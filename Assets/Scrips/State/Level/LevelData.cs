using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level System/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    [Tooltip("Tên của file Scene cần load. Phải viết chính xác.")]
    public string sceneName;

    [Tooltip("Tên hiển thị trong menu (ví dụ: 'Màn 1: Rừng Rậm').")]
    public string displayName;

    [Tooltip("Hình ảnh đại diện cho màn chơi.")]
    public Sprite levelIcon;

    // Có thể mở rộng thêm sau này, ví dụ:
    public bool isLocked = true;
}