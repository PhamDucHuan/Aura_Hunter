using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Biến lưu vị trí checkpoint đã được chuyển sang CheckpointManager

    void Start()
    {
        // Khi bắt đầu game, yêu cầu Manager đặt điểm hồi sinh tại vị trí người chơi
        if (CheckpointManager.instance != null)
        {
            CheckpointManager.instance.currentRespawnPosition = transform.position;
        }
    }

    // Hàm này vẫn được gọi bởi DeathZone
    public void Respawn()
    {
        Debug.Log("Người chơi quay trở lại checkpoint!");
        // Lấy vị trí hồi sinh từ Manager
        if (CheckpointManager.instance != null)
        {
            transform.position = CheckpointManager.instance.currentRespawnPosition;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Hàm UpdateCheckpoint có thể xóa đi vì không còn dùng nữa
}