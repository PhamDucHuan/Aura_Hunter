using System.Collections;
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

    [Tooltip("Thời gian chờ (tính bằng giây) từ lúc bắt đầu animation đến lúc thực sự gây sát thương")]
    [SerializeField] private float damageDelay = 0.3f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI TỪ SCRIPT ENEMYMOVEMENT
    public void PerformAttack()
    {
        // 1. Kích hoạt animation tấn công
        _animator.SetTrigger("attack");

        // 2. Bắt đầu một Coroutine để xử lý việc gây sát thương sau một khoảng chờ
        StartCoroutine(DealDamageAfterDelay());
    }

    // <<< HÀM COROUTINE MỚI >>>
    private IEnumerator DealDamageAfterDelay()
    {
        // 1. Chờ một khoảng thời gian bằng giá trị của damageDelay
        yield return new WaitForSeconds(damageDelay);

        // 2. Sau khi chờ, kiểm tra xem người chơi CÓ CÒN trong vùng tấn công không
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        // 3. Gây sát thương cho những người chơi vẫn còn trong vùng
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("Enemy hit " + player.name + " after a delay.");
            CharacterManager playerHealth = player.GetComponent<CharacterManager>();
            if (playerHealth != null)
            {
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
