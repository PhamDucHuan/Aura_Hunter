using UnityEngine;

// Dòng này cho phép bạn tạo các asset từ script này trong menu của Unity
[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character/Character Stats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [Header("Character Info")]
    public string characterName = "New Character";
    public Sprite characterIcon; // Dùng cho UI

    [Header("Primary Stats")]
    [Tooltip("Lượng máu tối đa của nhân vật.")]
    public int maxHealth = 100;

    [Tooltip("Sát thương cơ bản của nhân vật.")]
    public int baseDamage = 10;

    [Tooltip("Giáp hoặc khả năng phòng thủ, dùng để giảm sát thương nhận vào.")]
    public int armor = 5;

    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển bình thường.")]
    public float walkSpeed = 5f;

    [Tooltip("Tốc độ khi chạy nhanh.")]
    public float runSpeed = 8f;

    [Tooltip("Lực nhảy của nhân vật.")]
    public float jumpForce = 12f;

    // Bạn có thể thêm bất kỳ thông số nào khác ở đây
    // Ví dụ: Tốc độ tấn công, tỉ lệ chí mạng, kháng phép, v.v.
}