using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // <<< THÊM MỚI: Enum để chọn kiểu tấn công >>>
    public enum AttackType { Melee, Ranged }
    [Header("Attack Mode")]
    [SerializeField] private AttackType currentAttackType = AttackType.Melee;

    [Header("Components")]
    private Animator _animator;
    [SerializeField] private CharacterStats characterStats;
    private PlayerInput _inputActions;

    // <<< Cài đặt cũ cho tầm gần được giữ nguyên >>>
    [Header("Melee Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    // <<< THÊM MỚI: Cài đặt cho tấn công tầm xa >>>
    [Header("Ranged Attack Settings")]
    [SerializeField] private GameObject projectilePrefab; // Prefab của viên đạn
    [SerializeField] private Transform firePoint; // Vị trí đạn được bắn ra
    [SerializeField] private float projectileSpeed = 20f;

    [Header("Attack Cooldown")]
    [SerializeField] private float attackRate = 2f;
    private float _nextAttackTime = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        _inputActions.Player.Fire.performed += OnAttack;
        // (Tùy chọn) Thêm một nút để đổi vũ khí
        // _inputActions.Player.SwitchWeapon.performed += OnSwitchWeapon;
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Fire.performed -= OnAttack;
        // _inputActions.Player.SwitchWeapon.performed -= OnSwitchWeapon;
        _inputActions.Player.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Time.time >= _nextAttackTime)
        {
            // <<< THAY ĐỔI: Gọi hàm Attack() thay vì thực hiện logic trực tiếp >>>
            Attack();
            _nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // <<< THAY ĐỔI: Hàm Attack giờ sẽ phân loại kiểu tấn công >>>
    private void Attack()
    {
        // Kích hoạt animation tấn công chung
        _animator.SetTrigger("attack");

        // Kiểm tra kiểu tấn công hiện tại
        if (currentAttackType == AttackType.Melee)
        {
            PerformMeleeAttack();
        }
        else // if (currentAttackType == AttackType.Ranged)
        {
            StartCoroutine(PerformRangedAttack());
        }
    }

    private void PerformMeleeAttack()
    {
        // Logic tấn công tầm gần cũ
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(characterStats.baseDamage);
            }
        }
    }

    IEnumerator PerformRangedAttack()
    {
        // Logic tấn công tầm xa mới
        yield return new WaitForSeconds(0.8f); // Đợi một chút để đồng bộ với animation nếu cần
        if (projectilePrefab != null && firePoint != null)
        {
            // 1. Tạo ra viên đạn
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // 2. Lấy script Projectile từ viên đạn vừa tạo
            Bullet projectileScript = projectileGO.GetComponent<Bullet>();

            if (projectileScript != null)
            {
                // 3. Xác định hướng bắn dựa trên hướng của Player (chính là transform.right)
                // transform.right sẽ tự động lật khi scale.x của Player thay đổi
                Vector2 launchDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0f);

                // 4. Gọi hàm Launch và truyền vào hướng và tốc độ
                projectileScript.Launch(launchDirection, projectileSpeed);
            }
        }
        else
        {
            Debug.LogWarning("Projectile Prefab hoặc Fire Point chưa được gán!");
        }
    }

    // (Tùy chọn) Hàm để đổi vũ khí
    /*
    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (currentAttackType == AttackType.Melee)
        {
            currentAttackType = AttackType.Ranged;
        }
        else
        {
            currentAttackType = AttackType.Melee;
        }
        Debug.Log("Switched to " + currentAttackType.ToString() + " mode.");
    }
    */

    private void OnDrawGizmosSelected()
    {
        // Vẽ vùng cho tầm gần
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        // Vẽ điểm cho tầm xa
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
}