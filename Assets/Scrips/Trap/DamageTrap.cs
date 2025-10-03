using UnityEngine;

public class DamageTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("L??ng sát th??ng b?y gây ra.")]
    [SerializeField] private int damageAmount = 10;

    // Hàm này s? t? ??ng ???c g?i khi có m?t ??i t??ng khác ?i vào trigger c?a b?y
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Ki?m tra xem ??i t??ng va ch?m có ph?i là "Player" không
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the trap!");

            // 2. L?y component CharacterManager t? ??i t??ng Player
            CharacterManager playerManager = other.GetComponent<CharacterManager>();

            // 3. N?u tìm th?y, g?i hàm TakeDamage ?? tr? máu
            if (playerManager != null)
            {
                playerManager.TakeDamage(damageAmount);
            }
        }
    }
}