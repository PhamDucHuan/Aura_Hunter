using UnityEngine;

public class ObjectMover : MonoBehaviour, IUpdateListener
{
    // Enum để chọn kiểu di chuyển trong Inspector
    public enum MovementType
    {
        Horizontal, // Di chuyển qua lại (trái - phải)
        Vertical    // Di chuyển lên xuống
    }

    [Header("Movement Settings")]
    [Tooltip("Chọn kiểu di chuyển cho object.")]
    [SerializeField] private MovementType moveType = MovementType.Horizontal;

    [Tooltip("Tốc độ di chuyển của object.")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Khoảng cách di chuyển từ điểm bắt đầu.")]
    [SerializeField] private float moveDistance = 5f;

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Lưu lại vị trí ban đầu của object
        startingPosition = transform.position;

        // Thiết lập mục tiêu ban đầu dựa trên kiểu di chuyển
        SetInitialTarget();
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

    public void OnUpdate(float deltaTime)
    {
        // Di chuyển object về phía mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Nếu object đã đến gần mục tiêu, đổi chiều di chuyển
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            SwitchDirection();
        }
    }

    private void SetInitialTarget()
    {
        if (moveType == MovementType.Horizontal)
        {
            targetPosition = startingPosition + new Vector3(moveDistance, 0, 0);
        }
        else // Vertical
        {
            targetPosition = startingPosition + new Vector3(0, moveDistance, 0);
        }
    }

    private void SwitchDirection()
    {
        // Tính toán 2 điểm cuối của quãng đường di chuyển
        Vector3 endPoint1, endPoint2;
        if (moveType == MovementType.Horizontal)
        {
            endPoint1 = startingPosition + new Vector3(moveDistance, 0, 0);
            endPoint2 = startingPosition - new Vector3(moveDistance, 0, 0);
        }
        else // Vertical
        {
            endPoint1 = startingPosition + new Vector3(0, moveDistance, 0);
            endPoint2 = startingPosition - new Vector3(0, moveDistance, 0);
        }

        // Nếu mục tiêu hiện tại là điểm 1, đổi sang điểm 2 và ngược lại
        if (targetPosition == endPoint1)
        {
            targetPosition = endPoint2;
        }
        else
        {
            targetPosition = endPoint1;
        }
    }

    // (Tùy chọn) Vẽ ra đường di chuyển trong Scene View để dễ hình dung
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 startPos = Application.isPlaying ? startingPosition : transform.position;
        Vector3 endPos;

        if (moveType == MovementType.Horizontal)
        {
            endPos = startPos + new Vector3(moveDistance * 2, 0, 0);
            startPos -= new Vector3(moveDistance, 0, 0);
            endPos -= new Vector3(moveDistance, 0, 0);
        }
        else // Vertical
        {
            endPos = startPos + new Vector3(0, moveDistance * 2, 0);
            startPos -= new Vector3(0, moveDistance, 0);
            endPos -= new Vector3(0, moveDistance, 0);
        }

        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(startPos, 0.3f);
        Gizmos.DrawWireSphere(endPos, 0.3f);
    }
}