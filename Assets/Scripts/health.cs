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
                RoomManager.instance.HandleTeamSelection();
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }
            Destroy(gameObject);
        }
    }
}
