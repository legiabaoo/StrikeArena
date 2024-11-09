using System.Collections;
using UnityEngine;

public class bomkhoi : MonoBehaviour
{
    public GameObject explosionEffect; // Hiệu ứng nổ
    public float explosionDelay = 3f; // Thời gian đếm ngược trước khi nổ

    private bool hasExploded = false;

    void Start()
    {
        // Kiểm tra xem hiệu ứng nổ có được gán không
        if (explosionEffect == null)
        {
            Debug.LogError("Explosion effect not assigned in the Inspector!");
            return;
        }

        // Bắt đầu đếm ngược
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Hiển thị hiệu ứng nổ
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Debug.Log("Explosion instantiated at: " + transform.position); // Log vị trí để kiểm tra

        // Hủy hiệu ứng nổ sau 5 giây
        Destroy(explosion, 5f);

        // Hủy bom sau khi nổ
        Destroy(gameObject);

        // Log để xác nhận rằng bom đã bị hủy
        Debug.Log("Bomb destroyed after explosion.");
    }
}
