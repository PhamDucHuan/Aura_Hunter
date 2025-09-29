using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button selectButton;

    [Tooltip("Kéo Panel chứa toàn bộ UI chọn nhân vật vào đây")]
    [SerializeField] private GameObject characterSelectPanel;

    private int _currentIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextCharacter);
        prevButton.onClick.AddListener(PreviousCharacter);
        selectButton.onClick.AddListener(ConfirmSelection);

        if (GameManager.Instance != null && GameManager.Instance.characterPrefabs.Count > 0)
        {
            DisplayCharacter(_currentIndex);
        }
        else
        {
            // Nếu không, báo lỗi và vô hiệu hóa UI để tránh lỗi
            Debug.LogError("GameManager chưa sẵn sàng hoặc danh sách nhân vật rỗng!");
            characterIcon.gameObject.SetActive(false);
            characterNameText.text = "Không có nhân vật";
            nextButton.interactable = false;
            prevButton.interactable = false;
            selectButton.interactable = false;
        }
    }

    private void DisplayCharacter(int index)
    {
        // <<< THAY ĐỔI: Thêm các bước kiểm tra an toàn >>>
        if (GameManager.Instance == null || GameManager.Instance.characterPrefabs.Count == 0)
        {
            Debug.LogError("Không thể hiển thị nhân vật vì GameManager chưa sẵn sàng hoặc danh sách rỗng.");
            return; // Dừng hàm tại đây
        }

        GameObject characterPrefab = GameManager.Instance.characterPrefabs[index];
        if (characterPrefab == null)
        {
            Debug.LogError($"Prefab tại vị trí {index} trong GameManager bị rỗng (None)!");
            return;
        }

        CharacterManager manager = characterPrefab.GetComponent<CharacterManager>();
        if (manager == null)
        {
            Debug.LogError($"Prefab '{characterPrefab.name}' bị thiếu component CharacterManager!");
            return;
        }

        CharacterStats stats = manager.GetCharacterData();
        if (stats == null)
        {
            Debug.LogError($"Prefab '{characterPrefab.name}' chưa được gán CharacterStats trong component CharacterManager!");
            return;
        }

        // Nếu mọi thứ đều ổn, cập nhật UI
        characterIcon.sprite = stats.characterIcon;
        characterNameText.text = stats.characterName;
    }

    // Các hàm NextCharacter, PreviousCharacter, StartGame giữ nguyên không đổi
    public void NextCharacter()
    {
        _currentIndex = (_currentIndex + 1) % GameManager.Instance.characterPrefabs.Count;
        DisplayCharacter(_currentIndex);
    }

    public void PreviousCharacter()
    {
        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = GameManager.Instance.characterPrefabs.Count - 1;
        }
        DisplayCharacter(_currentIndex);
    }

    public void ConfirmSelection()
    {
        // 1. Lấy prefab nhân vật đang được hiển thị
        GameObject selectedPrefab = GameManager.Instance.characterPrefabs[_currentIndex];

        // 2. "Chốt" lựa chọn này và báo cho GameManager
        GameManager.Instance.SelectCharacter(selectedPrefab);

        Debug.Log("Player confirmed selection: " + selectedPrefab.name);

        // 3. Ẩn giao diện chọn nhân vật đi
        if (characterSelectPanel != null)
        {
            characterSelectPanel.SetActive(false);
        }

        // 4. (Tùy chọn) Hiện lại menu chính hoặc một giao diện khác ở đây
        // mainMenuPanel.SetActive(true);
    }
}
