using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    private Animator _animator;
    [SerializeField] private CharacterStats characterStats;
    private PlayerInput _inputActions;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint; // Vị trí để tạo ra vùng tấn công
    [SerializeField] private float attackRange = 0.5f; // Bán kính của vùng tấn công
    //[SerializeField] private int attackDamage = 40; // Sát thương mỗi đòn đánh
    [SerializeField] private LayerMask enemyLayers; // Layer của các đối tượng là kẻ địch

    [Header("Attack Cooldown")]
    [SerializeField] private float attackRate = 2f; // Tần suất tấn công (số lần mỗi giây)
    private float _nextAttackTime = 0f; // Thời điểm có thể tấn công lần tiếp theo

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputActions = new PlayerInput(); // Giả sử bạn đã có class PlayerInput từ Input Actions
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện tấn công với action "Fire" (hoặc tên bạn đặt)
        _inputActions.Player.Fire.performed += OnAttack;
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Fire.performed -= OnAttack;
        _inputActions.Player.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        // Kiểm tra xem đã đến lúc được tấn công chưa
        if (Time.time >= _nextAttackTime)
        {
            Attack();
            // Cập nhật lại thời điểm cho lần tấn công tiếp theo
            _nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void Attack()
    {
        // 1. Kích hoạt animation tấn công
        _animator.SetTrigger("attack");

        // 2. Phát hiện kẻ địch trong vùng tấn công
        // Tạo một vòng tròn vô hình tại attackPoint để kiểm tra va chạm
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Gây sát thương cho những kẻ địch bị trúng đòn
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            // Thử lấy component EnemyHealth từ đối tượng va chạm
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(characterStats.baseDamage);
            }
        }
    }

    // (Tùy chọn) Vẽ ra vùng tấn công trong Scene View để dễ dàng căn chỉnh
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}