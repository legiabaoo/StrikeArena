using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    public string NhaTanCong; // T�n GameObject m� b?n mu?n di chuy?n ng??i ch?i ??n

    private Transform targetSpawnPosition;

    void Start()
    {
        // T�m GameObject d?a tr�n t�n
        GameObject targetObject = GameObject.Find(NhaTanCong);
        if (targetObject != null)
        {
            Debug.LogWarning("T�m th?y GameObject v?i t�n: " + NhaTanCong);
            targetSpawnPosition = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Kh�ng t�m th?y GameObject v?i t�n: " + NhaTanCong);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && PhotonView.Get(this).IsMine) // Ki?m tra n?u l� nh�n v?t c?a ng??i ch?i
        {
            TeleportToSpawnPoint();
        }
        // T�m GameObject d?a tr�n t�n
        GameObject targetObject = GameObject.Find(NhaTanCong);
        if (targetObject != null)
        {
         
            targetSpawnPosition = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Kh�ng t�m th?y GameObject v?i t�n: " + NhaTanCong);
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
            Debug.LogWarning("Ch?a g�n v? tr� spawn!");
        }
    }
}
