using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class health : MonoBehaviour
{
    public int healths;
    public int armor;
    public bool isLocalPlayer;

    [Header("UI")]
    public TMP_Text healthText;
    public TMP_Text armorText;  // Hiển thị giáp

    private HealthBar healthBar; // Tham chiếu đến thanh sức khỏe của người chơi
    [HideInInspector] public BloodOverlay blood;

    menu menu;
    void Start()
    {

        menu = FindObjectOfType<menu>();


        if (healthText != null)
            healthText.text = healths.ToString();

        if (armorText != null)
            armorText.text = armor.ToString();

        // Lấy thanh sức khỏe từ prefab hoặc tạo mới nếu cần
        if (isLocalPlayer)
        {
            menu.manchet.SetActive(false);
            healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.SetPlayerHealth(healths); // Gán sức khỏe ban đầu
            }
            blood = FindObjectOfType<BloodOverlay>();
        }
        RoomManager.instance.UpdatePlayerStatus(true);

    }


    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (armor > 0)
        {
            int remainingDamage = damage - armor; // Phần sát thương còn lại sau khi trừ giáp
            armor -= damage; // Trừ giáp trước
            if (armor < 0) armor = 0; // Đảm bảo giáp không âm

            if (remainingDamage > 0)
            {
                healths -= remainingDamage; // Phần sát thương dư sẽ trừ vào máu
            }
        }
        else
        {
            healths -= damage; // Nếu không còn giáp, trừ trực tiếp vào máu
        }

        if (healths < 0) healths = 0; // Đảm bảo máu không âm

        // Cập nhật giao diện người dùng
        if (healthText != null)
            healthText.text = healths.ToString();

        if (armorText != null)
            armorText.text = armor.ToString();

        if (isLocalPlayer && healthBar != null)
        {
            healthBar.UpdateHealth(healths); // Cập nhật thanh sức khỏe
            blood.ShowBloodEffect();
        }

        if (healths <= 0 && armor <=0)
        {
            if (isLocalPlayer)
            {
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
                menu.manchet.SetActive(true);
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

            }
            else if (currentCamera == null)
            {
                Debug.Log("Camera bi null");
            }
            RoomManager.instance.UpdatePlayerStatus(false);
        }
    }
    [PunRPC]
    public void Heal(int amount, int ViewID)
    {
        health health = PhotonView.Find(ViewID).GetComponent<health>();
        healths = health.healths;
        healths += amount;
        if (healths > 100) // Giới hạn máu tối đa, tùy thuộc vào game của bạn
        {
            healths = 100;
        }

        // Cập nhật giao diện người dùng
        if (healthText != null)
            healthText.text = healths.ToString();

        if (isLocalPlayer && healthBar != null)
        {
            healthBar.UpdateHealth(healths); // Cập nhật thanh máu
        }
    }
    [PunRPC]
    public void AddArmor(int amount, int ViewID)
    {
        health health = PhotonView.Find(ViewID).GetComponent<health>();
        armor = health.armor;
        armor += amount;
        if (armor > 100) // Giới hạn giáp tối đa, tùy thuộc vào game của bạn
        {
            armor = 100;
        }

        // Cập nhật giao diện người dùng
        if (armorText != null)
            armorText.text = armor.ToString();
    }
}
