using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("Danh sách tất cả các màn chơi.")]
    [SerializeField] private List<LevelData> levels;

    [Header("UI Setup")]
    [Tooltip("Prefab của một nút chọn màn chơi.")]
    [SerializeField] private GameObject levelButtonPrefab;
    [Tooltip("Đối tượng cha để chứa các nút được tạo ra.")]
    [SerializeField] private Transform buttonContainer;

    void Start()
    {
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        // Xóa các nút cũ đi nếu có (để tránh tạo trùng lặp)
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Duyệt qua danh sách dữ liệu và tạo nút cho mỗi màn chơi
        foreach (LevelData level in levels)
        {
            // Tạo một instance của prefab nút
            GameObject buttonInstance = Instantiate(levelButtonPrefab, buttonContainer);

            // Lấy script LevelButton từ instance đó
            LevelButton buttonScript = buttonInstance.GetComponent<LevelButton>();

            // Gửi dữ liệu của màn chơi hiện tại cho script của nút
            buttonScript.Setup(level);
        }
    }
}