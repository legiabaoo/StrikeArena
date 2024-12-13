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

  

    // Phương thức mở rương và chỉ cập nhật giáp cho người nhặt rương
    void OpenChest()
    {

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (
                player.CustomProperties.TryGetValue("viewID", out var viewIDValue) &&
                viewIDValue is int viewID )
            {
                GameObject playerObject = PhotonView.Find(viewID)?.gameObject;
                health playerHealth = playerObject.GetComponent<health>();
                playerHealth.AddArmor(soLuongGiap);
                if (playerHealth != null)
                {
                    // Cộng giáp cho người chơi local

                    // Trừ tiền cho người chơi
                    GunShop.instance.playerMoney -= cost;
                    // Cập nhật lại UI tiền sau khi trừ

                    Debug.Log("Mở rương thành công! ");

                    // Đánh dấu rương đã được mở
                    isUsed = true;
                    handUI.SetActive(false);
                    OB.GetComponent<Animator>().SetBool("open", true); // Chạy animation mở rương
                }
            }
        }

        // Thực hiện kiểm tra chỉ người chơi nhặt rương thực sự nhận giáp
        //if (PhotonNetwork.LocalPlayer != null)
        //{
        //    // Lấy player từ PhotonNetwork
        //    health playerHealth = FindObjectOfType<health>();
        //    playerHealth.AddArmor(soLuongGiap);
        //    if (playerHealth != null)
        //    {
        //        // Cộng giáp cho người chơi local

        //        // Trừ tiền cho người chơi
        //        playerMoney -= cost;
        //        // Cập nhật lại UI tiền sau khi trừ
        
        //        Debug.Log("Mở rương thành công! Số tiền còn lại: " + playerMoney);

        //        // Đánh dấu rương đã được mở
        //        isUsed = true;
        //        handUI.SetActive(false);
        //        OB.GetComponent<Animator>().SetBool("open", true); // Chạy animation mở rương
        //    }
        //}
    }
}
