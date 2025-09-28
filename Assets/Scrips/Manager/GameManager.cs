using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // THAY ĐỔI: Từ List<CharacterStats> thành List<GameObject>
    [Header("Character Prefabs")]
    public List<GameObject> characterPrefabs;

    // THAY ĐỔI: Lưu trữ GameObject prefab thay vì CharacterStats
    public GameObject SelectedCharacterPrefab { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // THAY ĐỔI: Hàm này nhận vào một GameObject
    public void SelectCharacter(GameObject characterPrefab)
    {
        SelectedCharacterPrefab = characterPrefab;
        // Lấy tên từ component để Debug
        if (characterPrefab.TryGetComponent<CharacterManager>(out var manager))
        {
            Debug.Log("Character selected: " + manager.GetCharacterData().characterName);
        }
    }
}
