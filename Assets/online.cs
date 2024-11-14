using Photon.Pun;
using UnityEngine;

public class online : MonoBehaviour
{
    public GameObject bombPrefab; // Prefab của bom

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Nhấn phím Space để ném bom
        {
            // Chỉ người chơi cục bộ mới có thể ném bom
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                // Xác định vị trí để tạo bom
                Vector3 spawnPosition = transform.position + transform.forward * 2;
                GameObject bomb = PhotonNetwork.Instantiate(bombPrefab.name, spawnPosition, Quaternion.identity);

                // Gọi hàm StartExplosionCountdown() để bắt đầu đếm ngược nổ
                bomb.GetComponent<BomKhoi>().StartExplosionCountdown();
            }
        }
    }
}
