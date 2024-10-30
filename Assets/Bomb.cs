using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect; // Hi?u ?ng n?
    public float explosionDelay = 3f; // Th?i gian ??m ng??c tr??c khi n?

    private bool hasExploded = false;

    void Start()
    {
        // B?t ??u ??m ng??c
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Hi?n th? hi?u ?ng n?
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        // H?y ??i t??ng hi?u ?ng n? sau m?t th?i gian (ví d?: 2 giây)
        Destroy(explosion, 2f);

     
    }
}
