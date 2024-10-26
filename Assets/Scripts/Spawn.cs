using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public string NhaTanCong; // Tên GameObject mà b?n mu?n di chuy?n ng??i ch?i ??n

    private Transform targetSpawnPosition;

    void Start()
    {
        // Tìm GameObject d?a trên tên
        GameObject targetObject = GameObject.Find(NhaTanCong);
        if (targetObject != null)
        {
            Debug.LogWarning("Tìm th?y GameObject v?i tên: " + NhaTanCong);
            targetSpawnPosition = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm th?y GameObject v?i tên: " + NhaTanCong);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && PhotonView.Get(this).IsMine) // Ki?m tra n?u là nhân v?t c?a ng??i ch?i
        {
            TeleportToSpawnPoint();
        }
        // Tìm GameObject d?a trên tên
        GameObject targetObject = GameObject.Find(NhaTanCong);
        if (targetObject != null)
        {
         
            targetSpawnPosition = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm th?y GameObject v?i tên: " + NhaTanCong);
        }
    }

    public void TeleportToSpawnPoint()
    {
        if (targetSpawnPosition != null)
        {
            transform.position = targetSpawnPosition.position;
            transform.rotation = targetSpawnPosition.rotation;
        }
        else
        {
            Debug.LogWarning("Ch?a gán v? trí spawn!");
        }
    }
}
