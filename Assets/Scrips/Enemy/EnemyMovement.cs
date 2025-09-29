using UnityEngine;

[RequireComponent(typeof(EnemyAttack))]
public class EnemyMovement : MonoBehaviour, IUpdateListener
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    // <<< THAY ĐỔI: Sử dụng một khoảng cách tuần tra thay vì các điểm Transform >>>
    [Tooltip("Quái vật sẽ đi qua lại trong khoảng cách này so với vị trí ban đầu.")]
    [SerializeField] private float patrolDistance = 5f;

    [Header("Patrol Settings")]
    [Tooltip("Điểm để bắn raycast kiểm tra tường phía trước.")]
    [SerializeField] private Transform wallCheckPoint;
    [Tooltip("Điểm để bắn raycast kiểm tra mép vực phía trước.")]
    [SerializeField] private Transform edgeCheckPoint;
    [Tooltip("Khoảng cách để phát hiện tường.")]
    [SerializeField] private float wallCheckDistance = 0.2f;
    [Tooltip("Layer của các vật cản (đất, tường...).")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float stopChaseDistance = 1.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Player Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer;

    // --- Biến nội bộ ---
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private float lastAttackTime = -999f;
    private EnemyAttack _enemyAttack;

    // <<< THAY ĐỔI: Các biến mới để quản lý tuần tra bằng khoảng cách >>>
    private Vector2 startingPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;
    private Vector2 currentTargetPosition;

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

        // <<< THAY ĐỔI: Tự động tính toán 2 điểm tuần tra dựa trên vị trí ban đầu >>>
        startingPosition = transform.position;
        leftPatrolPoint = new Vector2(startingPosition.x - patrolDistance, startingPosition.y);
        rightPatrolPoint = new Vector2(startingPosition.x + patrolDistance, startingPosition.y);

        // Bắt đầu bằng cách đi về phía bên phải
        currentTargetPosition = rightPatrolPoint;
    }

    // Các hàm OnEnable, OnDisable, OnUpdate, FindPlayer... giữ nguyên không đổi
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

    public void OnUpdate(float deltaTime)
    {
        FindPlayer();
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

    // ... các hàm khác không đổi ...
    private void FindPlayer()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, detectionRadius, Vector2.zero, 0f, playerLayer);
        if (hit.collider != null)
        {
            player = hit.transform;
            if (currentState == State.Patrolling)
            {
                currentState = State.Chasing;
            }
        }
        else
        {
            player = null;
            if (currentState == State.Chasing || currentState == State.Attacking)
            {
                currentState = State.Patrolling;
                rb.velocity = Vector2.zero;
                anim.SetBool("isWalking", false);
            }
        }
    }

    // <<< THAY ĐỔI: Viết lại hàm Patrol để di chuyển giữa 2 điểm đã tính toán >>>
    private void Patrol()
    {
        if (patrolDistance <= 0)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        // Điều kiện 1: Đã đến điểm cuối của patrolDistance
        bool reachedPatrolPoint = Vector2.Distance(transform.position, currentTargetPosition) < 0.5f;

        // Điều kiện 2: Phát hiện thấy vật cản (tường hoặc vực)
        bool detectedObstacle = ShouldTurn();

        // Nếu một trong hai điều kiện đúng, đổi hướng
        if (reachedPatrolPoint || detectedObstacle)
        {
            if (currentTargetPosition == rightPatrolPoint)
            {
                currentTargetPosition = leftPatrolPoint;
            }
            else
            {
                currentTargetPosition = rightPatrolPoint;
            }
        }

        // Di chuyển về phía mục tiêu
        anim.SetBool("isWalking", true);
        Vector2 direction = (currentTargetPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);
    }

    private bool ShouldTurn()
    {
        // Xác định hướng di chuyển hiện tại
        float moveDirection = Mathf.Sign(rb.velocity.x);
        // Nếu đang đứng yên, lấy hướng từ sprite
        if (moveDirection == 0)
        {
            // Giả sử scale.x < 0 là quay mặt sang phải
            moveDirection = -Mathf.Sign(transform.localScale.x);
        }

        // Bắn Raycast kiểm tra tường
        bool isHittingWall = Physics2D.Raycast(wallCheckPoint.position, new Vector2(moveDirection, 0), wallCheckDistance, groundLayer);

        // Bắn Raycast kiểm tra vực
        bool isAtEdge = !Physics2D.Raycast(edgeCheckPoint.position, Vector2.down, 2f, groundLayer);

        return isHittingWall || isAtEdge;
    }

    // ... các hàm còn lại giữ nguyên ...
    private void Chase()
    {
        if (player == null) { currentState = State.Patrolling; return; }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange) { currentState = State.Attacking; }
        else if (distanceToPlayer > stopChaseDistance)
        {
            anim.SetBool("isWalking", true);
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            float directionToPlayer = player.position.x - transform.position.x;
            FlipSprite(directionToPlayer);
        }
    }

    private void Attack()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("isWalking", false);
        if (player == null || Vector2.Distance(transform.position, player.position) > attackRange) { currentState = State.Chasing; return; }
        float directionToPlayer = player.position.x - transform.position.x;
        FlipSprite(directionToPlayer);
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("Attack");
            _enemyAttack.PerformAttack();
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // <<< THAY ĐỔI: Cập nhật Gizmos để vẽ ra vùng tuần tra >>>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ đường tuần tra TỐI ĐA
        Gizmos.color = Color.cyan;
        Vector2 startPos = Application.isPlaying ? startingPosition : (Vector2)transform.position;
        Vector2 leftPoint = startPos - new Vector2(patrolDistance, 0);
        Vector2 rightPoint = startPos + new Vector2(patrolDistance, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawWireSphere(leftPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPoint, 0.3f);

        // Vẽ các tia raycast
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            float direction = Application.isPlaying ? Mathf.Sign(rb.velocity.x) : 1;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + new Vector3(wallCheckDistance * direction, 0, 0));
        }
        if (edgeCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(edgeCheckPoint.position, edgeCheckPoint.position + new Vector3(0, -2f, 0));
        }
    }
}