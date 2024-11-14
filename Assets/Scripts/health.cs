using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class health : MonoBehaviour
{
    public int healths;
    public bool isLocalPlayer;

    [Header("UI")]
    public TMP_Text healthText;

    private HealthBar healthBar; // Tham chiếu đến thanh sức khỏe của người chơi

    void Start()
    {
        healthText.text = healths.ToString();

        // Lấy thanh sức khỏe từ prefab hoặc tạo mới nếu cần
        if (isLocalPlayer)
        {
            healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.SetPlayerHealth(healths); // Gán sức khỏe ban đầu
            }
        }
        RoomManager.instance.UpdatePlayerStatus(true);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        healths -= damage;
        if (healths < 0) healths = 0; // Đảm bảo sức khỏe không âm

        // Cập nhật giao diện người dùng
        healthText.text = healths.ToString();

        if (isLocalPlayer && healthBar != null)
        {
            healthBar.UpdateHealth(healths); // Cập nhật thanh sức khỏe của người chơi
        }

        if (healths <= 0)
        {
            if (isLocalPlayer)
            {
                //RoomManager.instance.HandleTeamSelection();
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }
            OnPlayerDeath();
        }
    }
    private void OnPlayerDeath()
    {
        // Gọi phương thức trên PlayerSetup để xử lý khi chết
        GetComponent<PlayerSetup>().OnPlayerDeath();

        // Chuyển camera sang đồng đội
        if (isLocalPlayer)
        {
            GameObject currentPlayerObject = gameObject; // Lấy GameObject của người chơi
            Camera currentCamera = GetComponentInChildren<Camera>();

            if (currentCamera != null)
            {
                
                CameraManager.instance.SwitchToTeammateCamera(currentPlayerObject);

            }else if(currentCamera == null)
            {
                Debug.Log("Camera bi null");
            }
            RoomManager.instance.UpdatePlayerStatus(false);
        }
    }

}
