using UnityEngine;

[RequireComponent(typeof(EnemyAttack))]
public class EnemyMovement : MonoBehaviour
{
    // --- Các biến có thể chỉnh trong Inspector ---
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float stopChaseDistance = 1.5f; // **BIẾN MỚI:** Khoảng cách dừng lại khi đuổi theo
    [SerializeField] private float attackCooldown = 2f;

    // --- Biến nội bộ ---
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private int patrolIndex = 0;
    private float lastAttackTime = -999f;
    private EnemyAttack _enemyAttack;

    // --- State Machine ---
    private enum State { Patrolling, Chasing, Attacking }
    private State currentState;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        _enemyAttack = GetComponent<EnemyAttack>();
    }

    void Start()
    {
        currentState = State.Patrolling;
    }

    void Update()
    {
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // **LOGIC ĐÃ CẬP NHẬT**
        if (distanceToPlayer < attackRange)
        {
            // Nếu đủ gần, chuyển sang tấn công
            currentState = State.Attacking;
        }
        else if (distanceToPlayer > stopChaseDistance)
        {
            // Nếu còn xa, tiếp tục đuổi theo
            anim.SetBool("isWalking", true);
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
        else
        {
            // Nếu ở giữa khoảng cách tấn công và khoảng cách dừng, thì đứng yênrb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);
        }
    }

    private void Attack()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("isWalking", false);

        // Lật mặt về phía người chơi
        if (player != null)
        {
            float directionToPlayer = player.position.x - transform.position.x;
            FlipSprite(directionToPlayer);
        }

        // Kiểm tra cooldown
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // GỌI HÀM TẤN CÔNG TỪ SCRIPT MỚI
            _enemyAttack.PerformAttack();
        }
        else
        {
            // Nếu đang cooldown, quay lại đuổi theo
            currentState = State.Chasing;
        }
    }

    private void FlipSprite(float directionX)
    {
        // Giả sử sprite gốc của bạn hướng về bên trái
        if (directionX > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

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
        }
    }
}