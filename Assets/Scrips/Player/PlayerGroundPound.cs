using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement))]
public class PlayerGroundPound : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rb;
    private PlayerMovement _movementScript;
    private PlayerInput _inputActions;
    private Animator _animator;

    [Header("Pound Settings")]
    [SerializeField] private float poundVelocity = -30f; // Tốc độ lao xuống
    [SerializeField] private float poundImpactRadius = 1.5f; // Bán kính ảnh hưởng khi va chạm
    [SerializeField] private LayerMask poundableLayers; // Các layer có thể bị ảnh hưởng (đất vỡ, kẻ thù)
    [SerializeField] private GameObject impactEffectPrefab; // Hiệu ứng va chạm (tùy chọn)

    private bool _isPounding = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movementScript = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _inputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        // Sử dụng Action "Crouch" (Ngồi) khi ở trên không để dậm đất
        // Bạn có thể tạo Action mới nếu muốn
        _inputActions.Player.Crouch.performed += OnPound;
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Crouch.performed -= OnPound;
        _inputActions.Player.Disable();
    }

    private void OnPound(InputAction.CallbackContext context)
    {
        // Chỉ cho phép dậm khi đang ở trên không và chưa bắt đầu dậm
        if (!_movementScript.IsGrounded && !_isPounding)
        {
            _isPounding = true;
            _rb.velocity = new Vector2(0, poundVelocity); // Lao thẳng xuống
            _animator.SetTrigger("groundPound"); // Kích hoạt animation
        }
    }

    // Chúng ta sẽ gọi hàm này từ PlayerMovement2D khi nhân vật tiếp đất
    public void OnLanding()
    {
        if (_isPounding)
        {
            HandlePoundImpact();
            _isPounding = false;
        }
    }

    private void HandlePoundImpact()
    {
        // Tạo hiệu ứng va chạm nếu có
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        // Phát hiện tất cả các đối tượng trong bán kính ảnh hưởng
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, poundImpactRadius, poundableLayers);

        foreach (var obj in hitObjects)
        {
            // Kiểm tra xem đối tượng có thể bị "dậm" không
            IPoundable poundable = obj.GetComponent<IPoundable>();
            if (poundable != null)
            {
                poundable.OnPounded();
            }
        }
    }

    // Vẽ vùng ảnh hưởng trong Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, poundImpactRadius);
    }
}