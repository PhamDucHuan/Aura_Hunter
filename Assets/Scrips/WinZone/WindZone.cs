using UnityEngine;
using UnityEngine.InputSystem;

// <<< THAY ĐỔI: Không yêu cầu BoxCollider2D nữa >>>
public class WindZone : MonoBehaviour, IFixedUpdateListener
{
    public enum WindDirection
    {
        Right, Left, Up, Down,
        UpRight, UpLeft, DownRight, DownLeft
    }

    [Header("Wind Settings")]
    [Tooltip("Hướng của luồng gió.")]
    [SerializeField] private WindDirection direction = WindDirection.Right;
    [Tooltip("Lực đẩy của gió.")]
    [SerializeField] private float windForce = 10f;

    // <<< THÊM MỚI: Các biến cho Raycast >>>
    [Tooltip("Độ dài của luồng gió.")]
    [SerializeField] private float windDistance = 10f;
    [Tooltip("Chỉ tác động lên các đối tượng thuộc những layer này.")]
    [SerializeField] private LayerMask affectedLayers;

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.RegisterFixedUpdateListener(this);
        }
    }

    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UnregisterFixedUpdateListener(this);
        }
    }

    // <<< THAY ĐỔI: Logic được chuyển sang FixedUpdate() để xử lý vật lý >>>
    public void OnFixedUpdate(float deltaTime)
    {
        // Lấy hướng gió từ enum
        Vector2 castDirection = GetDirectionVector();

        // Bắn một tia Raycast, thu thập TẤT CẢ các đối tượng nó va chạm
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, castDirection, windDistance, affectedLayers);

        // Duyệt qua tất cả các đối tượng bị trúng tia
        foreach (RaycastHit2D hit in hits)
        {
            // Lấy Rigidbody2D của đối tượng đó
            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Tác động lực lên nó
                rb.AddForce(castDirection * windForce, ForceMode2D.Force);
            }
        }
    }

    // <<< THAY ĐỔI: Xóa hoàn toàn hàm OnTriggerStay2D() >>>

    // Các hàm GetDirectionVector, OnValidate, RotateTrapToDirection giữ nguyên
    private Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case WindDirection.Right: return Vector2.right;
            case WindDirection.Left: return Vector2.left;
            case WindDirection.Up: return Vector2.up;
            case WindDirection.Down: return Vector2.down;
            case WindDirection.UpRight: return new Vector2(1, 1).normalized;
            case WindDirection.UpLeft: return new Vector2(-1, 1).normalized;
            case WindDirection.DownRight: return new Vector2(1, -1).normalized;
            case WindDirection.DownLeft: return new Vector2(-1, -1).normalized;
            default: return Vector2.zero;
        }
    }

    // OnValidate và RotateTrapToDirection giúp xoay object trong Editor
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () => { if (this != null) { RotateTrapToDirection(); } };
    }
    private void RotateTrapToDirection()
    {
        Vector2 directionVector = GetDirectionVector();
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    // <<< THAY ĐỔI: Gizmo sẽ vẽ theo windDistance >>>
    private void OnDrawGizmosSelected()
    {
        Vector2 gizmoDirection = GetDirectionVector();
        if (gizmoDirection == Vector2.zero) return;

        Gizmos.color = Color.cyan;
        Vector3 startPosition = transform.position;
        // Điểm cuối giờ sẽ được tính bằng windDistance
        Vector3 endPosition = startPosition + (Vector3)gizmoDirection * windDistance;

        Gizmos.DrawLine(startPosition, endPosition);

        // Vẽ đầu mũi tên
        float arrowheadAngle = 25.0f;
        float arrowheadLength = 0.3f;
        Vector3 rightDir = Quaternion.Euler(0, 0, arrowheadAngle) * (-gizmoDirection.normalized);
        Vector3 leftDir = Quaternion.Euler(0, 0, -arrowheadAngle) * (-gizmoDirection.normalized);
        Gizmos.DrawRay(endPosition, rightDir * arrowheadLength);
        Gizmos.DrawRay(endPosition, leftDir * arrowheadLength);
    }
}