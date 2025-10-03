using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator : MonoBehaviour, IUpdateListener
{
    // Enum để chọn hướng xoay trong Inspector
    public enum RotationDirection
    {
        Clockwise,        // Theo chiều kim đồng hồ
        CounterClockwise  // Ngược chiều kim đồng hồ
    }

    [Header("Rotation Settings")]
    [Tooltip("Tốc độ xoay của vật thể (độ/giây).")]
    [SerializeField] private float rotationSpeed = 100f;

    [Tooltip("Hướng xoay của vật thể.")]
    [SerializeField] private RotationDirection direction = RotationDirection.Clockwise;


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
        // Xác định hướng xoay dựa trên giá trị của enum
        // Xoay theo chiều kim đồng hồ là xoay quanh trục Z theo chiều âm
        // Ngược chiều kim đồng hồ là xoay quanh trục Z theo chiều dương
        float directionMultiplier = (direction == RotationDirection.Clockwise) ? -1f : 1f;

        // Áp dụng phép xoay quanh trục Z
        // Nhân với Time.deltaTime để tốc độ xoay không phụ thuộc vào framerate
        transform.Rotate(0f, 0f, rotationSpeed * directionMultiplier * Time.deltaTime);
    }
}