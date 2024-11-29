using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BomKhoi : MonoBehaviourPun
{
    public GameObject explosionEffect;      // Hiệu ứng khói
    public float explosionDelay = 5f;       // Thời gian đếm ngược trước khi nổ (5 giây)
    public AudioClip amthanhkhoi;           // Âm thanh khói
    private bool hasExploded = false;

    // Coroutine xử lý việc phát nổ sau 5 giây
    [PunRPC]
    void ExplodeEffectAndSound()
    {
        if (hasExploded) return;  // Nếu bom đã nổ rồi thì không làm gì

        hasExploded = true;

        // Hiển thị hiệu ứng khói trên tất cả các máy khách
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 5f); // Hủy hiệu ứng sau 5 giây

        // Phát âm thanh nếu có
        if (amthanhkhoi != null)
        {
            AudioSource.PlayClipAtPoint(amthanhkhoi, transform.position);
        }

        // Hủy bom sau khi kích hoạt hiệu ứng
        PhotonNetwork.Destroy(gameObject);

        Debug.Log("Smoke bomb activated and destroyed.");
    }

    // Chạy đếm ngược và gọi RPC đồng bộ sau 5 giây
    public void StartExplosionCountdown()
    {
        // Bắt đầu coroutine để đếm ngược 5 giây
        StartCoroutine(ExplosionCountdown());
    }

    // Coroutine để đếm ngược và gọi RPC sau 5 giây
    IEnumerator ExplosionCountdown()
    {
        // Chờ 5 giây
        yield return new WaitForSeconds(explosionDelay);

        // Gọi RPC trên tất cả các máy khách để kích hoạt hiệu ứng khói và âm thanh đồng bộ
        photonView.RPC("ExplodeEffectAndSound", RpcTarget.All);
    }
}
