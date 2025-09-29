using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image levelIconImage;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button button;

    private LevelData currentLevelData;

    public void Setup(LevelData levelData)
    {
        currentLevelData = levelData; // Lưu lại dữ liệu của màn chơi này

        levelIconImage.sprite = levelData.levelIcon;
        levelNameText.text = levelData.displayName;

        // <<< THAY ĐỔI: Sự kiện OnClick giờ sẽ gọi hàm OnSelectButton >>>
        button.onClick.AddListener(OnSelectButton);
    }

    // <<< THAY ĐỔI: Đổi tên và chức năng của hàm >>>
    private void OnSelectButton()
    {
        // Báo cho GameManager biết màn chơi này đã được chọn
        GameManager.Instance.SelectLevel(currentLevelData);

        // (Tùy chọn) Thêm phản hồi trực quan, ví dụ đổi màu nút được chọn
        // Bạn có thể mở rộng phần này sau.
        Debug.Log("Button clicked for level: " + currentLevelData.displayName);
    }
}