using UnityEngine;

[RequireComponent(typeof(EnemyAttack))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Components")]
    private Animator _animator;

    [Header("Stats")]
    [Tooltip("Sử dụng ScriptableObject để quản lý chỉ số cho Enemy")]

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage = 20; // Sát thương mỗi đòn đánh
    [SerializeField] private float attackRange = 0.5f;
    [Tooltip("Layer của đối tượng mà Enemy sẽ tấn công (thường là Player)")]
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI TỪ SCRIPT ENEMYMOVEMENT
    public void PerformAttack()
    {
        // 1. Kích hoạt animation tấn công
        _animator.SetTrigger("attack");

        // 2. Phát hiện người chơi trong vùng tấn công
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        // 3. Gây sát thương cho người chơi
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("Enemy hit " + player.name);
            CharacterManager playerHealth = player.GetComponent<CharacterManager>();
            if (playerHealth != null)
            {
                // Lấy sát thương từ ScriptableObject
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Vẽ vùng tấn công trong Scene View
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
