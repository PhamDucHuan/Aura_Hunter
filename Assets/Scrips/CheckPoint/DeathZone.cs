using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Hàm này phải là OnTriggerEnter2D với tham số là Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Dòng debug để kiểm tra va chạm
        Debug.Log("Va chạm 2D được phát hiện với đối tượng: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Đó là Player! Chuẩn bị Respawn.");

            // Lấy script và gọi hàm
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
            }
        }
    }
}