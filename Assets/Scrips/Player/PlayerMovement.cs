using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour, IFixedUpdateListener, IUpdateListener
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private PlayerGroundPound _groundPoundScript;
    private PlayerInput _inputActions;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // Một object con đặt ở chân nhân vật
    [SerializeField] private LayerMask groundLayer; // Layer của các đối tượng được coi là mặt đất
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Jump Settings")]
    [Tooltip("Số lần nhảy tối đa (1 = nhảy đơn, 2 = nhảy đôi).")]
    [SerializeField] private int maxJumps = 2; // MỚI: Số lần nhảy tối đa
    private int _jumpsRemaining; // MỚI: Biến đếm số lần nhảy còn lại

    // Biến nội bộ
    private Vector2 _moveInput;
    private bool _isSprinting = false;
    private bool _isGrounded;
    private bool _isFacingRight = true;

    public bool IsGrounded => _isGrounded;
    private void Awake()
    {
        _inputActions = new PlayerInput();

        // Đăng ký các sự kiện input
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
        _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;

        _inputActions.Player.Jump.performed += OnJump;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.RegisterFixedUpdateListener(this);
            UpdateManager.Instance.RegisterUpdateListener(this);
        }
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UnregisterFixedUpdateListener(this);
            UpdateManager.Instance.UnregisterUpdateListener(this);
        }
    }
 
    public void OnUpdate(float deltaTime)
    {
        // Xử lý hoạt ảnh dựa trên trạng thái hiện tại
        HandleAnimations();
    }

    public void OnFixedUpdate(float deltaTime)
    {
        // Kiểm tra xem nhân vật có đang trên mặt đất không
        CheckIfGrounded();

        // Xử lý di chuyển ngang
        HandleMovement();

        // Xử lý lật hình nhân vật
        FlipCharacter();
    }

    private void CheckIfGrounded()
    {
        bool wasGrounded = _isGrounded; // Lưu lại trạng thái của frame trước
        // Tạo một vòng tròn vô hình ở vị trí groundCheck để phát hiện va chạm với groundLayer
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // ----- THAY ĐỔI CHO DOUBLE JUMP -----
        // Nếu nhân vật đang trên mặt đất, hồi lại toàn bộ số lần nhảy
        if (_isGrounded)
        {
            _jumpsRemaining = maxJumps;
        }
        // ------------------------------------
        if (!wasGrounded && _isGrounded)
        {
            // Gọi hàm xử lý va chạm của script dậm đất
            _groundPoundScript?.OnLanding();
        }
    }

    private void HandleAnimations()
    {
        // Lấy tốc độ di chuyển ngang tuyệt đối để quyết định giữa idle và run
        float horizontalSpeed = Mathf.Abs(_rb.velocity.x);
        _animator.SetFloat("speed", horizontalSpeed);

        // Cập nhật trạng thái isGrounded
        _animator.SetBool("isGrounded", _isGrounded);

        // Cập nhật vận tốc trục Y để quyết định giữa jump và fall
        _animator.SetFloat("yVelocity", _rb.velocity.y);
    }

    private void HandleMovement()
    {
        // Xác định tốc độ hiện tại (đi bộ hoặc chạy nhanh)
        float currentSpeed = _isSprinting ? characterStats.runSpeed : characterStats.walkSpeed;

        // Di chuyển nhân vật bằng cách thay đổi vận tốc của Rigidbody2D
        // Chúng ta giữ nguyên vận tốc theo trục Y để không ảnh hưởng đến trọng lực và lực nhảy
        _rb.velocity = new Vector2(_moveInput.x * currentSpeed, _rb.velocity.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // THAY ĐỔI: Kiểm tra xem có còn lượt nhảy nào không, thay vì chỉ kiểm tra isGrounded
        if (_jumpsRemaining > 0)
        {
            // Trừ đi một lượt nhảy
            _jumpsRemaining--;

            // Reset vận tốc y để lực nhảy luôn nhất quán
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(new Vector2(0f, characterStats.jumpForce), ForceMode2D.Impulse);
        }
    }

    private void FlipCharacter()
    {
        // Nếu nhân vật đang di chuyển sang trái và đang quay mặt sang phải
        if (_moveInput.x < 0 && _isFacingRight)
        {
            // Lật nhân vật
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _isFacingRight = false;
        }
        // Nếu nhân vật đang di chuyển sang phải và đang quay mặt sang trái
        else if (_moveInput.x > 0 && !_isFacingRight)
        {
            // Lật nhân vật lại
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _isFacingRight = true;
        }
    }

    // (Tùy chọn) Vẽ ra vòng tròn kiểm tra mặt đất trong Scene view để dễ debug
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
