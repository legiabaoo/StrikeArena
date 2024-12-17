using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UseChest : MonoBehaviourPunCallbacks
{
    public static UseChest Instance;
    public GameObject OB;
    public GameObject handUI;
    public int soLuongGiap = 30; // Số lượng giáp cộng thêm
    public int cost = 50; // Chi phí mở rương
    public bool isUsed = false;
    private bool inReach;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        OB = this.gameObject;
        handUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !isUsed)
        {
            inReach = true;
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && !isUsed && Input.GetKey(KeyCode.E))
        {
            if (GunShop.instance.playerMoney >= cost)
            {
                OpenChest();
            }
            else
            {
                Debug.Log("Không đủ tiền để mở rương!");
            }
        }
    }

    void OpenChest()
    {

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (
                player.CustomProperties.TryGetValue("viewID", out var viewIDValue) && viewIDValue is int viewID &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("viewID", out var viewIDLocal) && viewIDValue == viewIDLocal)
            {
                GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                if (playerObject != null)
                {
                    PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
                    playerPhotonView.RPC("AddArmor", RpcTarget.AllBuffered, soLuongGiap, viewID);
                    GunShop.instance.playerMoney -= cost;
                    Debug.Log("Mở rương thành công! ");
                    // Đánh dấu rương đã được mở
                    isUsed = true;
                    handUI.SetActive(false);
                    OB.GetComponent<Animator>().SetBool("open", true); // Chạy animation mở rương
                }
            }
        }
    }
}
