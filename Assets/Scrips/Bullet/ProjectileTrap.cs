using System.Collections;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    public enum ShootingDirection
    {
        Up, Down, Left, Right,
        UpRight, UpLeft, DownRight, DownLeft
    }

    [Header("Trap Settings")]
    [Tooltip("Hướng bắn của bẫy.")]
    [SerializeField] private ShootingDirection direction = ShootingDirection.Right;
    [Tooltip("Prefab của vật thể được bắn ra (mũi tên, viên đạn...).")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Vị trí nơi vật thể được bắn ra.")]
    [SerializeField] private Transform firePoint;

    [Header("Shooting Mechanics")]
    [Tooltip("Tốc độ bay của vật thể.")]
    [SerializeField] private float projectileSpeed = 10f;
    [Tooltip("Thời gian chờ giữa mỗi lần bắn (tính bằng giây).")]
    [SerializeField] private float fireCooldown = 2f;

    // <<< THAY ĐỔI: Không cần biến timer trong Update nữa >>>

    // <<< THAY ĐỔI: Hàm Start giờ sẽ khởi động Coroutine >>>
    private void Start()
    {
        // Bắt đầu vòng lặp bắn đạn
        StartCoroutine(ShootingRoutine());
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
        angle -= -90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // <<< THAY ĐỔI: Logic chờ được chuyển vào Coroutine >>>
    private IEnumerator ShootingRoutine()
    {
        // Đảm bảo fireCooldown lớn hơn 0 để tránh vòng lặp vô tận
        if (fireCooldown <= 0)
        {
            Debug.LogWarning("Fire Cooldown phải lớn hơn 0. Bẫy sẽ chỉ bắn một lần.");
            Shoot();
            yield break; // Kết thúc Coroutine
        }

        // Vòng lặp vô tận để bắn liên tục
        while (true)
        {
            // Chờ cho lần bắn đầu tiên (hoặc lần bắn tiếp theo)
            yield return new WaitForSeconds(fireCooldown);

            // Bắn
            Shoot();
        }
    }

    private void Shoot()
    {
        //Debug.Log("Bẫy bắn!", this);
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Chưa gán Projectile Prefab hoặc Fire Point cho bẫy!", this);
            return;
        }

        Vector2 shotDirection = GetDirectionVector();
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BulletTrap projectileScript = projectileGO.GetComponent<BulletTrap>();
        if (projectileScript != null)
        {
            projectileScript.Launch(shotDirection, projectileSpeed);
        }
        else
        {
            Debug.LogError("Prefab được bắn ra thiếu script Projectile!", projectilePrefab);
        }
    }

    // Hàm GetDirectionVector và OnDrawGizmosSelected giữ nguyên không đổi
    private Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case ShootingDirection.Up: return Vector2.up;
            case ShootingDirection.Down: return Vector2.down;
            case ShootingDirection.Left: return Vector2.left;
            case ShootingDirection.Right: return Vector2.right;
            case ShootingDirection.UpRight: return new Vector2(1, 1).normalized;
            case ShootingDirection.UpLeft: return new Vector2(-1, 1).normalized;
            case ShootingDirection.DownRight: return new Vector2(1, -1).normalized;
            case ShootingDirection.DownLeft: return new Vector2(-1, -1).normalized;
            default: return Vector2.right;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = GetDirectionVector();
            Vector3 startPosition = firePoint.position;
            Vector3 endPosition = startPosition + (Vector3)direction * 2f;

            // Vẽ thân mũi tên
            Gizmos.DrawLine(startPosition, endPosition);

            // Vẽ đầu mũi tên
            float arrowheadAngle = 25.0f;
            float arrowheadLength = 0.3f;
            Vector3 rightDir = Quaternion.Euler(0, 0, arrowheadAngle) * (-direction.normalized);
            Vector3 leftDir = Quaternion.Euler(0, 0, -arrowheadAngle) * (-direction.normalized);
            Gizmos.DrawRay(endPosition, rightDir * arrowheadLength);
            Gizmos.DrawRay(endPosition, leftDir * arrowheadLength);
        }
    }
}