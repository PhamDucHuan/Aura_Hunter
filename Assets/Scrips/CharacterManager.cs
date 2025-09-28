using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    // Kéo file ScriptableObject Stats (ví dụ: PlayerStats) vào đây trong Inspector
    [SerializeField] private CharacterStats characterData;
    [SerializeField] private Slider healthSlider; // Thanh hiển thị máu 

    // Các biến trạng thái hiện tại của nhân vật
    [SerializeField] private int _currentHealth;

    public void Initialize(CharacterStats newData)
    {
        characterData = newData; // Gán dữ liệu mới

        if (characterData != null)
        {
            gameObject.name = "Player (" + characterData.characterName + ")";
            _currentHealth = characterData.maxHealth;

            if (healthSlider != null)
            {
                healthSlider.maxValue = characterData.maxHealth;
                healthSlider.value = characterData.maxHealth;
            }
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
        healthSlider.value = _currentHealth;
    }

    private void Die()
    {
        Debug.Log(characterData.characterName + " has died.");
        // Xử lý logic khi chết
        Destroy(gameObject);
    }

    public CharacterStats GetCharacterData()
    {
        return characterData;
    }
}