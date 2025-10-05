using System.Collections;
using UnityEngine;

// <<< SỬA LỖI: Đưa enum ra ngoài để đúng chuẩn hơn >>>
public enum MovementType { Horizontal, Vertical }

[RequireComponent(typeof(Animator))]
public class BreakableMovingPlatform : MonoBehaviour, IUpdateListener
{
    public enum PlatformBehavior
    {
        Static, // Đứng yên
        Moving  // Di chuyển
    }

    [Header("Platform Behavior")]
    [Tooltip("Chọn hành vi cho nền tảng: đứng yên hoặc di chuyển.")]
    [SerializeField] private PlatformBehavior behavior = PlatformBehavior.Static;

    [Header("Movement Settings (If Moving)")]
    [SerializeField] private MovementType moveType = MovementType.Horizontal;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;

    [Header("Breaking Settings")]
    [Tooltip("Thời gian chờ trước khi nền tảng bắt đầu vỡ (tính bằng giây).")]
    [SerializeField] private float timeBeforeBreak = 2f;
    [Tooltip("Thời gian của animation phá hủy.")]
    [SerializeField] private float destructionAnimationTime = 0.5f;
    [SerializeField] private Animator animator;

    // Biến nội bộ
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Coroutine breakingCoroutine;

    private Transform playerOnPlatform = null;
    void Start()
    {
        if (behavior == PlatformBehavior.Moving)
        {
            startingPosition = transform.position;
            SetInitialTarget();
        }
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
        if (behavior == PlatformBehavior.Moving)
        {
            MovePlatform();
        }
    }

    // ... (Các hàm OnCollision và BreakPlatformRoutine giữ nguyên)

    // <<< THÊM MỚI: Hàm để vẽ đường di chuyển trong Editor >>>
    private void OnDrawGizmosSelected()
    {
        // Chỉ vẽ đường đi nếu hành vi được chọn là 'Moving'
        if (behavior == PlatformBehavior.Moving)
        {
            Gizmos.color = Color.cyan;
            // Dùng transform.position khi chưa chạy game để Gizmos đi theo vật thể
            Vector3 startForGizmo = Application.isPlaying ? startingPosition : transform.position;
            Vector3 point1, point2;

            if (moveType == MovementType.Horizontal)
            {
                point1 = startForGizmo + new Vector3(moveDistance, 0, 0);
                point2 = startForGizmo - new Vector3(moveDistance, 0, 0);
            }
            else // Vertical
            {
                point1 = startForGizmo + new Vector3(0, moveDistance, 0);
                point2 = startForGizmo - new Vector3(0, moveDistance, 0);
            }

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawWireSphere(point1, 0.3f);
            Gizmos.DrawWireSphere(point2, 0.3f);
        }
    }

    #region Các hàm không thay đổi (tôi thu gọn lại)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // <<< THÊM MỚI: Lưu lại transform của người chơi >>>
            playerOnPlatform = collision.transform;

            if (behavior == PlatformBehavior.Moving)
            {
                playerOnPlatform.SetParent(this.transform);
            }

            if (breakingCoroutine == null)
            {
                breakingCoroutine = StartCoroutine(BreakPlatformRoutine());
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Luôn tháo player ra khi họ rời đi
            collision.transform.SetParent(null);

            // <<< THÊM MỚI: Xóa tham chiếu đến người chơi >>>
            playerOnPlatform = null;

            // Dừng tiến trình phá hủy nếu người chơi rời đi sớm
            if (breakingCoroutine != null)
            {
                StopCoroutine(breakingCoroutine);
                breakingCoroutine = null;
            }
        }
    }
    private IEnumerator BreakPlatformRoutine()
    {
        yield return new WaitForSeconds(timeBeforeBreak);
        if (animator != null)
        {
            animator.SetTrigger("Break");
        }
        yield return new WaitForSeconds(destructionAnimationTime);

        if (playerOnPlatform != null)
        {
            playerOnPlatform.SetParent(null);
        }

        Destroy(gameObject);
    }

    private void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            SwitchDirection();
        }
    }

    private void SetInitialTarget()
    {
        if (moveType == MovementType.Horizontal)
            targetPosition = startingPosition + new Vector3(moveDistance, 0, 0);
        else
            targetPosition = startingPosition + new Vector3(0, moveDistance, 0);
    }

    private void SwitchDirection()
    {
        Vector3 endPoint1, endPoint2;
        if (moveType == MovementType.Horizontal)
        {
            endPoint1 = startingPosition + new Vector3(moveDistance, 0, 0);
            endPoint2 = startingPosition - new Vector3(moveDistance, 0, 0);
        }
        else
        {
            endPoint1 = startingPosition + new Vector3(0, moveDistance, 0);
            endPoint2 = startingPosition - new Vector3(0, moveDistance, 0);
        }
        if (targetPosition == endPoint1)
            targetPosition = endPoint2;
        else
            targetPosition = endPoint1;
    }
    #endregion
}
