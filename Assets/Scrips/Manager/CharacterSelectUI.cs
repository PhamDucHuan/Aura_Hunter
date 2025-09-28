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
    [SerializeField] private Button startButton;

    private int _currentIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextCharacter);
        prevButton.onClick.AddListener(PreviousCharacter);
        startButton.onClick.AddListener(StartGame);

        DisplayCharacter(_currentIndex);
    }

    private void DisplayCharacter(int index)
    {
        // Lấy prefab từ GameManager
        GameObject characterPrefab = GameManager.Instance.characterPrefabs[index];

        // Lấy component CharacterManager từ prefab để đọc dữ liệu
        CharacterManager manager = characterPrefab.GetComponent<CharacterManager>();
        if (manager != null)
        {
            CharacterStats stats = manager.GetCharacterData();
            // Cập nhật UI
            characterIcon.sprite = stats.characterIcon;
            characterNameText.text = stats.characterName;
        }

        // Lưu prefab đã chọn vào GameManager
        GameManager.Instance.SelectCharacter(characterPrefab);
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

    public void StartGame()
    {
        // Thay "GameScene" bằng tên Scene game của bạn
        SceneManager.LoadScene("SampleScene");
    }
}
