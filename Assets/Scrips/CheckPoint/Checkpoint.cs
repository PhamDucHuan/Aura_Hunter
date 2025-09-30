using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Mỗi checkpoint sẽ có một index riêng, bạn sẽ đặt số này trong Inspector
    public int checkpointIndex;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer; // Tùy chọn: để thay đổi màu sắc
    public Color activatedColor = Color.green; // Tùy chọn: màu khi kích hoạt

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            // Báo cáo cho Manager thay vì tự xử lý
            if (CheckpointManager.instance != null)
            {
                CheckpointManager.instance.ActivateCheckpoint(checkpointIndex, transform.position);

                // Sau khi báo cáo, có thể tự đánh dấu là đã kích hoạt
                // isActivated = true; // Bỏ ghi chú dòng này nếu bạn muốn mỗi checkpoint chỉ kích hoạt 1 lần duy nhất trong suốt game
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = activatedColor; // Thay đổi màu sắc để báo hiệu
                }
            }
            else
            {
                Debug.LogError("Không tìm thấy CheckpointManager trong màn chơi!");
            }
        }
    }
}