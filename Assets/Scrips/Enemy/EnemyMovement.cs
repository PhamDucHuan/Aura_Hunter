using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private int patrolIndex = 0;

    private enum State { Patrolling, Chasing }
    private State currentState;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            return;
        }

        anim.SetBool("isWalking", true);

        Transform targetPoint = patrolPoints[patrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);

        // **FIXED:** Chỉ kiểm tra khoảng cách trên trục X
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

        anim.SetBool("isWalking", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);
    }

    private void FlipSprite(float directionX)
    {
        // Nếu di chuyển sang phải (directionX > 0)
        if (directionX > 0)
        {
            // Lật sprite về bên TRÁI (scale.x = -1) để nó quay mặt sang PHẢI
            // (Giả sử sprite gốc của bạn hướng về bên trái)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // Nếu di chuyển sang trái (directionX < 0)
        else if (directionX < 0)
        {
            // Giữ nguyên sprite về bên PHẢI (scale.x = 1) để nó quay mặt sang TRÁI
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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