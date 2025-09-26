using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IPoundable
{
    public void OnPounded()
    {
        // Logic khi bị dậm: ví dụ, phá hủy object
        Debug.Log(gameObject.name + " was broken by a ground pound!");
        // Thêm hiệu ứng, âm thanh...
        Destroy(gameObject);
    }
}