using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // --- Các biến có thể chỉnh trong Inspector ---
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;      // Khoảng cách để tấn công
    [SerializeField] private float attackCooldown = 2f;   // Thời gian nghỉ giữa các đòn đánh
    [SerializeField] private GameObject attackHitbox;     // Vùng gây sát thương

    // --- Biến nội bộ ---
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private int patrolIndex = 0;
    private float lastAttackTime = -999f; // Đặt giá trị âm để có thể tấn công ngay lần đầu

    // --- State Machine ---
    private enum State { Patrolling, Chasing, Attacking }
    private State currentState;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentState = State.Patrolling;
        attackHitbox.SetActive(false); // Đảm bảo hitbox đã tắt khi bắt đầu
    }

    void Update()
    {
        // "Bộ não" của kẻ thù
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        anim.SetBool("isWalking", true);
        Transform targetPoint = patrolPoints[patrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void Chase()
    {
        if (player == null)
        {
            currentState = State.Patrolling;
            anim.SetBool("isWalking", false);
            return;
        }

        // Kiểm tra khoảng cách để quyết định đuổi hay tấn công
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            // Nếu đủ gần, chuyển sang tấn công
            currentState = State.Attacking;
        }
        else
        {
            // Nếu chưa đủ gần, tiếp tục đuổi theo
            anim.SetBool("isWalking", true);
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
    }

    private void Attack()
    {
        // Đứng yên khi tấn công
        rb.velocity = Vector2.zero;
        anim.SetBool("isWalking", false);

        // Kiểm tra xem đã hết thời gian nghỉ chưa
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối
            anim.SetTrigger("attack");    // Kích hoạt animation tấn công
        }
        else
        {
            // Nếu đang trong thời gian nghỉ, quay lại đuổi theo
            // (Điều này ngăn kẻ thù bị "đơ" nếu người chơi lùi ra khỏi tầm đánh)
            currentState = State.Chasing;
        }
    }

    // --- Các hàm hỗ trợ ---

    private void FlipSprite(float directionX)
    {
        // Đảo ngược logic quay mặt nếu sprite gốc của bạn hướng về bên trái
        if (directionX > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // --- CÁC HÀM NÀY SẼ ĐƯỢC GỌI BẰNG ANIMATION EVENT ---
    public void EnableAttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
    }

    // --- Xử lý Trigger va chạm của DetectionZone ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            currentState = State.Chasing;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
            // Khi người chơi rời đi, trạng thái Chase sẽ tự động chuyển về Patrol
        }
    }
}