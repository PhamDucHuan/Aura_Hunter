using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // Dùng Singleton pattern để dễ dàng truy cập từ mọi nơi
    public static CheckpointManager instance;

    private int lastCheckpointIndex = -1; // Bắt đầu bằng -1, tức chưa có checkpoint nào được kích hoạt
    public Vector3 currentRespawnPosition; // Vị trí hồi sinh hiện tại

    private void Awake()
    {
        // Thiết lập Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm này được các Checkpoint gọi
    public void ActivateCheckpoint(int checkpointIndex, Vector3 checkpointPosition)
    {
        // Chỉ cập nhật nếu đây là một checkpoint mới (có index lớn hơn)
        if (checkpointIndex > lastCheckpointIndex)
        {
            lastCheckpointIndex = checkpointIndex;
            currentRespawnPosition = checkpointPosition;
            Debug.Log("Đã kích hoạt Checkpoint mới! Index: " + checkpointIndex);
        }
        else
        {
            Debug.Log("Chạm vào checkpoint cũ (Index: " + checkpointIndex + "). Bỏ qua.");
        }
    }
}