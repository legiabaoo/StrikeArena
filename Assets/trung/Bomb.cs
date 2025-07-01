using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect; // Hiệu ứng nổ
    public float explosionDelay = 3f; // Thời gian đếm ngược trước khi nổ
    public float explosionRadius = 5f; // Bán kính sát thương
    public float explosionDamage = 50f; // Lượng sát thương

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

        // Gây sát thương lên tất cả đối tượng trong bán kính sát thương
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            // Kiểm tra xem đối tượng có thành phần Health hay không
            bomgaydau bomgaydau = nearbyObject.GetComponent<bomgaydau>();
            if (bomgaydau != null)
            {
                bomgaydau.TakeDamage(explosionDamage); // Gây sát thương
                Debug.Log("Damage dealt to: " + nearbyObject.name); // Log đối tượng đã bị gây sát thương
            }
        }

        // Hủy hiệu ứng nổ sau 5 giây
        Destroy(explosion, 5f);

        // Hủy bom sau khi nổ
        Destroy(gameObject);

        // Log để xác nhận rằng bom đã bị hủy
        Debug.Log("Bomb destroyed after explosion.");
    }
}
