using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Kéo file ScriptableObject Stats (ví dụ: PlayerStats) vào đây trong Inspector
    public CharacterStats characterData;

    // Các biến trạng thái hiện tại của nhân vật
    private int _currentHealth;

    void Awake()
    {
        // Khởi tạo các giá trị ban đầu từ ScriptableObject
        if (characterData != null)
        {
            gameObject.name = characterData.characterName;
            _currentHealth = characterData.maxHealth;
        }
        else
        {
            Debug.LogError("Character Stats data is not assigned on " + gameObject.name);
        }
    }

    public void TakeDamage(int damage)
    {
        // Tính toán sát thương thực tế sau khi trừ giáp
        int damageTaken = Mathf.Max(damage - characterData.armor, 1);
        _currentHealth -= damageTaken;

        Debug.Log(characterData.characterName + " took " + damageTaken + " damage. Health is now " + _currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(characterData.characterName + " has died.");
        // Xử lý logic khi chết
        Destroy(gameObject);
    }
}