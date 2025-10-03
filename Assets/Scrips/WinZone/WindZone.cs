using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone : MonoBehaviour
{
    // <<< THÊM MỚI: Enum để tạo dropdown chọn hướng trong Inspector >>>
    public enum WindDirection
    {
        Right,
        Left,
        Up,
        Down,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }

    [Header("Wind Settings")]
    [Tooltip("Hướng của luồng gió.")]
    [SerializeField] private WindDirection direction = WindDirection.Right;

    [Tooltip("Lực đẩy của gió.")]
    [SerializeField] private float windForce = 10f;

    // <<< THAY ĐỔI: Sử dụng GetDirectionVector() để lấy hướng >>>
    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Lấy vector hướng từ enum đã chọn
            Vector2 forceDirection = GetDirectionVector();

            // Thêm một lực đẩy liên tục vào đối tượng
            rb.AddForce(forceDirection * windForce, ForceMode2D.Force);
        }
    }

    // <<< THÊM MỚI: Hàm để "dịch" từ enum sang Vector2 >>>
    private Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case WindDirection.Right:
                return Vector2.right;
            case WindDirection.Left:
                return Vector2.left;
            case WindDirection.Up:
                return Vector2.up;
            case WindDirection.Down:
                return Vector2.down;
            case WindDirection.UpRight:
                return new Vector2(1, 1).normalized;
            case WindDirection.UpLeft:
                return new Vector2(-1, 1).normalized;
            case WindDirection.DownRight:
                return new Vector2(1, -1).normalized;
            case WindDirection.DownLeft:
                return new Vector2(-1, -1).normalized;
            default:
                return Vector2.zero;
        }
    }

    private void OnValidate()
    {
        // Hàm này giữ nguyên, không cần thay đổi
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                RotateTrapToDirection();
            }
        };
    }

    private void RotateTrapToDirection()
    {
        Vector2 directionVector = GetDirectionVector();
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        // <<< THÊM DÒNG NÀY: Trừ đi 90 độ để hiệu chỉnh cho sprite hướng lên >>>
        angle -= 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // <<< THAY ĐỔI: Gizmo cũng sẽ đọc hướng từ enum >>>
    private void OnDrawGizmosSelected()
    {
        Vector2 gizmoDirection = GetDirectionVector();
        if (gizmoDirection == Vector2.zero) return;

        Gizmos.color = Color.cyan;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + (Vector3)gizmoDirection * 2f;

        // Vẽ thân mũi tên
        Gizmos.DrawLine(startPosition, endPosition);

        // --- SỬA LẠI LOGIC VẼ ĐẦU MŨI TÊN ---

        // Kích thước và góc của đầu mũi tên
        float arrowheadAngle = 25.0f;
        float arrowheadLength = 0.3f;

        // Tính toán hướng của 2 cạnh đầu mũi tên bằng cách xoay vector hướng ngược lại
        // một góc nhỏ quanh trục Z (trục của màn hình 2D)
        Vector3 rightDir = Quaternion.Euler(0, 0, arrowheadAngle) * (-gizmoDirection.normalized);
        Vector3 leftDir = Quaternion.Euler(0, 0, -arrowheadAngle) * (-gizmoDirection.normalized);

        // Vẽ 2 cạnh của đầu mũi tên
        Gizmos.DrawRay(endPosition, rightDir * arrowheadLength);
        Gizmos.DrawRay(endPosition, leftDir * arrowheadLength);
    }
}