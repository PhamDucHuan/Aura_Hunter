using UnityEngine;

[RequireComponent(typeof(EnemyAttack))]
public class EnemyMovement : MonoBehaviour, IUpdateListener
{
    // --- Các biến có thể chỉnh trong Inspector ---
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float stopChaseDistance = 1.5f; // **BIẾN MỚI:** Khoảng cách dừng lại khi đuổi theo
    [SerializeField] private float attackCooldown = 2f;

    // <<< BIẾN MỚI CHO VIỆC DÒ TÌM BẰNG RAYCAST >>>
    [Header("Player Detection")]
    [SerializeField] private float detectionRadius = 5f; // Bán kính "radar" để phát hiện người chơi
    [SerializeField] private LayerMask playerLayer;      // Layer của người chơi để CircleCast chỉ tìm kiếm đối tượng này

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

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.RegisterUpdateListener(this);
        }
    }
    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UnregisterUpdateListener(this);
        }
    }

    void Start()
    {
        currentState = State.Patrolling;
    }

    public void OnUpdate(float deltaTime)
    {
        switch (currentState)
        {
            case State.Patrolling:
                FindPlayer();

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

    // <<< HÀM MỚI: SỬ DỤNG CIRCLECAST ĐỂ TÌM NGƯỜI CHƠI >>>
    private void FindPlayer()
    {
        // Bắn một "radar" hình tròn từ vị trí của quái vật với bán kính detectionRadius,
        // chỉ tìm kiếm các đối tượng trên playerLayer.
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, detectionRadius, Vector2.zero, 0f, playerLayer);

        if (hit.collider != null) // Nếu "radar" trúng một thứ gì đó (chính là người chơi)
        {
            player = hit.transform; // Lưu lại transform của người chơi
            if (currentState == State.Patrolling)
            {
                currentState = State.Chasing; // Nếu đang đi tuần thì chuyển sang đuổi theo
            }
        }
        else // Nếu không tìm thấy người chơi trong bán kính
        {
            player = null; // Bỏ tham chiếu đến người chơi
            if (currentState == State.Chasing || currentState == State.Attacking)
            {
                currentState = State.Patrolling; // Nếu đang đuổi hoặc tấn công, quay về đi tuần
                rb.velocity = Vector2.zero; // Dừng di chuyển ngay lập tức
                anim.SetBool("isWalking", false);
            }
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
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // NẾU VÀO TẦM ĐÁNH -> TẤN CÔNG
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
        // NẾU NGOÀI KHOẢNG DỪNG -> TIẾP TỤC ĐUỔI
        else if (distanceToPlayer > stopChaseDistance)
        {
            anim.SetBool("isWalking", true);
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
        // NẾU Ở GIỮA KHOẢNG DỪNG VÀ TẦM ĐÁNH -> ĐỨNG YÊN CHỜ
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            // Quay mặt về phía người chơi khi chờ
            float directionToPlayer = player.position.x - transform.position.x;
            FlipSprite(directionToPlayer);
        }
    }

    private void Attack()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("isWalking", false);

        // Nếu người chơi đã chạy ra khỏi tầm đánh, quay lại đuổi theo
        if (player == null || Vector2.Distance(transform.position, player.position) > attackRange)
        {
            currentState = State.Chasing;
            return; // Thoát khỏi hàm Attack
        }

        // Quay mặt về phía người chơi trước khi tấn công
        float directionToPlayer = player.position.x - transform.position.x;
        FlipSprite(directionToPlayer);

        // Kiểm tra cooldown và tấn công
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // Dòng này gọi animation tấn công, bạn cần tạo trigger "Attack" trong Animator
            anim.SetTrigger("Attack");

            // Gọi hàm gây sát thương từ script EnemyAttack
            _enemyAttack.PerformAttack();
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

    private void OnDrawGizmosSelected()
    {
        // --- Vẽ bán kính dò tìm ---
        Gizmos.color = Color.yellow; // Màu vàng cho dò tìm
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // --- Vẽ tầm đánh ---
        Gizmos.color = Color.red; // Màu đỏ cho tầm đánh
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}