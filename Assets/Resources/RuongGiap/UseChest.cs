using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UseChest : MonoBehaviourPunCallbacks
{
    private GameObject OB;
    public GameObject handUI;
    public int soLuongGiap = 30; // Số lượng giáp cộng thêm
    public int cost = 50; // Chi phí mở rương
    private bool isUsed = false;
    private bool inReach;


    private int playerMoney = 100;

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
            if (playerMoney >= cost)
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
        health playerHealth = FindObjectOfType<health>();
        playerHealth.AddArmor(soLuongGiap);
        // Thực hiện kiểm tra chỉ người chơi nhặt rương thực sự nhận giáp
        if (PhotonNetwork.LocalPlayer != null)
        {
            // Lấy player từ PhotonNetwork

            if (playerHealth != null)
            {
                // Cộng giáp cho người chơi local

                // Trừ tiền cho người chơi
                playerMoney -= cost;
                // Cập nhật lại UI tiền sau khi trừ
        
                Debug.Log("Mở rương thành công! Số tiền còn lại: " + playerMoney);

                // Đánh dấu rương đã được mở
                isUsed = true;
                handUI.SetActive(false);
                OB.GetComponent<Animator>().SetBool("open", true); // Chạy animation mở rương
            }
        }
    }
}
