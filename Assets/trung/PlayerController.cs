using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public GameObject smokeBombPrefab;           // Prefab của bom khói
    public GameObject explosionBombPrefab;       // Prefab của bom nổ
    public Transform handPosition;               // Vị trí tay người chơi để cầm bom

    private GameObject heldBomb;                 // Biến để giữ bom hiện tại đang cầm

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Nhấn phím T để tạo bom khói trên tay
            if (Input.GetKeyDown(KeyCode.T))
            {
                CreateBomb(smokeBombPrefab);
            }
            // Nhấn phím Y để tạo bom nổ trên tay
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                CreateBomb(explosionBombPrefab);
            }
            // Nhấn chuột trái để ném bom và kích hoạt hiệu ứng
            else if (Input.GetKeyDown(KeyCode.Mouse0) && heldBomb != null)
            {
                ThrowBomb();
            }
        }
    }

    // Hàm tạo bom trên tay người chơi
    void CreateBomb(GameObject bombPrefab)
    {
        // Xóa bom đang cầm nếu có
        if (heldBomb != null)
        {
            PhotonNetwork.Destroy(heldBomb);
        }

        if (bombPrefab != null)
        {
            // Tạo bom tại vị trí tay của người chơi mà không kích hoạt hiệu ứng
            heldBomb = PhotonNetwork.Instantiate(bombPrefab.name, handPosition.position, handPosition.rotation);
            heldBomb.transform.SetParent(handPosition); // Gắn bom vào tay người chơi
            DeactivateBombEffect(heldBomb);            // Tắt hiệu ứng bom
        }
        else
        {
            Debug.LogError("Bomb prefab not assigned in the Inspector!");
        }
    }

    // Hàm tắt hiệu ứng của bom
    void DeactivateBombEffect(GameObject bomb)
    {
        var explosionScript = bomb.GetComponent<BomKhoi>(); // Giả sử bom có script hiệu ứng
        if (explosionScript != null)
        {
            explosionScript.enabled = false; // Tắt script hiệu ứng
        }
    }

    // Hàm ném bom
    void ThrowBomb()
    {
        // Bỏ gắn bom với tay và đặt vị trí cho nó
        heldBomb.transform.SetParent(null);
        heldBomb.transform.position = handPosition.position;

        // Kích hoạt hiệu ứng nổ
        var explosionScript = heldBomb.GetComponent<BomKhoi>();
        if (explosionScript != null)
        {
            explosionScript.enabled = true;
            explosionScript.StartExplosionCountdown(); // Kích hoạt nổ khi ném
        }

        // Thêm lực để ném bom
        Rigidbody bombRigidbody = heldBomb.GetComponent<Rigidbody>();
        if (bombRigidbody != null)
        {
            bombRigidbody.isKinematic = false;
            bombRigidbody.AddForce(handPosition.forward * 10f, ForceMode.Impulse); // Tùy chỉnh lực ném
        }

        heldBomb = null; // Xóa tham chiếu tới bom đã ném
    }
}
