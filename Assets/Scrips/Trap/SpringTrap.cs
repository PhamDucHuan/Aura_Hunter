using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpringTrap : MonoBehaviour
{
    public enum LaunchDirection
    {
        Up,
        UpRight,
        UpLeft,
        Right,
        Left,
        Down,
        DownRight,
        DownLeft
    }

    [Header("Launch Settings")]
    [Tooltip("Hướng hất tung người chơi.")]
    [SerializeField] private LaunchDirection direction = LaunchDirection.Up;

    [Tooltip("Lực hất tung người chơi.")]
    [SerializeField] private float launchPower = 20f;

    [Header("Components")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // <<< THÊM MỚI: Xoay bẫy về đúng hướng khi game bắt đầu >>>
        RotateTrapToDirection();
    }

    // <<< THÊM MỚI: Hàm này chạy trong Editor mỗi khi bạn thay đổi giá trị Inspector >>>
    private void OnValidate()
    {
        // Sử dụng delayCall để tránh lỗi và đảm bảo cập nhật mượt mà trong Editor
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                RotateTrapToDirection();
            }
        };
    }

    // <<< THÊM MỚI: Hàm xử lý logic xoay >>>
    private void RotateTrapToDirection()
    {
        // Lấy vector hướng từ enum
        Vector2 directionVector = GetDirectionVector();

        // Tính toán góc xoay (tính bằng độ)
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        // Trừ đi 90 độ để hiệu chỉnh cho sprite có hướng mặc định là hướng LÊN
        angle -= 90f;

        // Áp dụng góc xoay cho vật thể bẫy
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            LaunchPlayer(collision.gameObject);
        }

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // Logic va chạm giữ nguyên không đổi
    //    if (collision.gameObject.CompareTag("Player") && collision.contacts[0].normal.y < -0.5)
    //    {
    //        LaunchPlayer(collision.gameObject);
    //    }
    //}

    private void LaunchPlayer(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        // <<< THÊM MỚI: Lấy component PlayerMovement >>>
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        if (playerRb != null && playerMovement != null)
        {
            animator.SetTrigger("Launch");

            // <<< THÊM MỚI: Vô hiệu hóa điều khiển của người chơi trong 0.5 giây >>>
            playerMovement.DisableMovement(0.5f);

            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

            Vector2 finalForce = GetDirectionVector() * launchPower;
            playerRb.AddForce(finalForce, ForceMode2D.Impulse);

            Debug.Log("Player launched by spring trap!");
        }
    }

    private Vector2 GetDirectionVector()
    {
        // Logic lấy vector hướng giữ nguyên không đổi
        switch (direction)
        {
            case LaunchDirection.Up: return Vector2.up;
            case LaunchDirection.UpRight: return new Vector2(1, 1).normalized;
            case LaunchDirection.UpLeft: return new Vector2(-1, 1).normalized;
            case LaunchDirection.Right: return Vector2.right;
            case LaunchDirection.Left: return Vector2.left;
            case LaunchDirection.Down: return Vector2.down;
            case LaunchDirection.DownRight: return new Vector2(1, -1).normalized;
            case LaunchDirection.DownLeft: return new Vector2(-1, -1).normalized;
            default: return Vector2.up;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Logic vẽ Gizmos giữ nguyên không đổi
        Gizmos.color = Color.green;
        Vector3 startPosition = transform.position;
        Vector2 gizmoDirection = GetDirectionVector();
        Vector3 endPosition = startPosition + (Vector3)gizmoDirection * (launchPower * 0.1f);
        Gizmos.DrawLine(startPosition, endPosition);

        float arrowheadAngle = 25.0f;
        float arrowheadLength = 0.3f;
        Vector3 rightDir = Quaternion.Euler(0, 0, arrowheadAngle) * (-gizmoDirection);
        Vector3 leftDir = Quaternion.Euler(0, 0, -arrowheadAngle) * (-gizmoDirection);
        Gizmos.DrawRay(endPosition, rightDir * arrowheadLength);
        Gizmos.DrawRay(endPosition, leftDir * arrowheadLength);
    }
}