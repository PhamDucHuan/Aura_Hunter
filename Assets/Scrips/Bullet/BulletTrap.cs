using UnityEngine;

public class BulletTrap : MonoBehaviour
{
    [Header("Settings")]
    // [SerializeField] private float speed = 20f; // Tốc độ sẽ được truyền từ PlayerAttack
    [SerializeField] public int damage = 15;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float lifeTime = 3f;

    // Xóa hoàn toàn hàm Start()

    // Hàm public mới để nhận lệnh từ bên ngoài
    public void Launch(Vector2 direction, float speed)
    {
        // 1. Đặt vận tốc bay theo hướng được chỉ định
        rb.velocity = direction.normalized * speed;

        // 2. (Tùy chọn) Xoay sprite của viên đạn cho đúng hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Tự hủy sau một khoảng thời gian
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            // 2. L?y component CharacterManager t? ??i t??ng Player
            CharacterManager playerManager = hitInfo.GetComponent<CharacterManager>();

            // 3. N?u tìm th?y, g?i hàm TakeDamage ?? tr? máu
            if (playerManager != null)
            {
                playerManager.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
