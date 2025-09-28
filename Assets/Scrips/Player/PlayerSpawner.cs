using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform spawnPoint; // Kéo một vị trí làm điểm xuất hiện vào đây

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.SelectedCharacterPrefab != null)
        {
            // Lấy prefab đã chọn từ GameManager
            GameObject playerPrefab = GameManager.Instance.SelectedCharacterPrefab;

            // Tạo một instance (bản sao) của prefab đó tại vị trí spawnPoint
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log(playerPrefab.name + " has been spawned.");
        }
        else
        {
            Debug.LogError("Could not find selected character prefab! Starting from Menu scene?");
            // Có thể tạo một nhân vật mặc định ở đây để test
        }
    }
}
