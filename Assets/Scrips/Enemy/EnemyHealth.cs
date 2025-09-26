using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoundable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " damage.");

        //(Tùy chọn) Thêm hiệu ứng bị đánh, ví dụ: đổi màu trong chốc lát
        StartCoroutine(ResetColorCoroutine());
    }

    private IEnumerator ResetColorCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void Die()
    {
        Debug.Log(transform.name + " died.");
        // (Tùy chọn) Kích hoạt animation chết, tạo hiệu ứng nổ, rơi vật phẩm...
        Destroy(gameObject);
    }

    public void OnPounded()
    {
        Debug.Log(transform.name + " was hit by a ground pound!");
        // Gây sát thương cực lớn hoặc giết ngay lập tức
        TakeDamage(9999);
    }
}