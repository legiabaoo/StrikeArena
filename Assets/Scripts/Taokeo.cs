using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taokeo : MonoBehaviour
{
    public GameObject stickyBombPrefab;
    public Transform spawnPoint;
    public float destroyTime = 10f;

    // Bi?n ?? gi?i h?n s? bom t?i ?a và ??m s? bom hi?n t?i
    public int maxBombs = 3;
    private int currentBombCount = 0;

    void Update()
    {
        if (GetComponent<PhotonView>().IsMine && Input.GetKeyDown(KeyCode.E))
        {
            if (currentBombCount < maxBombs) // Ki?m tra n?u s? bom ch?a ??t gi?i h?n
            {
                GetComponent<PhotonView>().RPC("CreateStickyBomb", RpcTarget.All);
            }
            else
            {
                Debug.Log("?ã ??t gi?i h?n s? bom keo.");
            }
        }
    }

    [PunRPC]
    void CreateStickyBomb()
    {
        // T?o bom và t?ng bi?n ??m s? bom
        GameObject newBomb = Instantiate(stickyBombPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(newBomb, destroyTime);
        currentBombCount++; // T?ng s? l??ng bom hi?n t?i

        // G?i ID c?a bom keo t?i t?t c? client ?? ??ng b? hóa vi?c destroy
        int bombID = newBomb.GetInstanceID();
        GetComponent<PhotonView>().RPC("DestroyStickyBomb", RpcTarget.All, bombID);
    }

    [PunRPC]
    void DestroyStickyBomb(int bombID)
    {
        // Tìm và h?y bom khi h?t th?i gian
        GameObject bombToDestroy = GameObject.Find(bombID.ToString());
        if (bombToDestroy != null)
        {
            Destroy(bombToDestroy);
            currentBombCount--; // Gi?m s? l??ng bom khi bom b? h?y
        }
    }
}
