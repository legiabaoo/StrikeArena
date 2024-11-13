using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BomKhoi : MonoBehaviourPun
{
    public GameObject explosionEffect;      // Hiệu ứng khói
    public float explosionDelay = 3f;       // Thời gian đếm ngược trước khi nổ

    private bool hasExploded = false;

    [PunRPC]
    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Hiển thị hiệu ứng khói trên tất cả các máy khách
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 5f); // Hủy hiệu ứng sau 5 giây

        // Hủy bom sau khi kích hoạt hiệu ứng
        PhotonNetwork.Destroy(gameObject);

        Debug.Log("Smoke bomb activated and destroyed.");
    }

    public void StartExplosionCountdown()
    {
        // Gọi hàm Explode trên tất cả các máy khách để kích hoạt hiệu ứng khói đồng bộ
        photonView.RPC("Explode", RpcTarget.All);
    }
}
