using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSelectedLevel()
    {
        // Kiểm tra xem người chơi đã chọn nhân vật VÀ màn chơi chưa
        if (GameManager.Instance != null &&
            GameManager.Instance.SelectedCharacterPrefab != null &&
            GameManager.Instance.SelectedLevel != null)
        {
            // Lấy tên scene từ màn chơi đã chọn và load nó
            string sceneToLoad = GameManager.Instance.SelectedLevel.sceneName;
            Debug.Log("Loading selected scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // Thông báo nếu người chơi chưa chọn đủ
            Debug.LogWarning("Vui lòng chọn nhân vật và màn chơi trước khi bắt đầu!");
            // (Tùy chọn) Hiển thị một thông báo trên UI ở đây
        }
    }
}