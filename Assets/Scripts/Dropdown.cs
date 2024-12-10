using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Dropdown : MonoBehaviour
{
    public TMP_Dropdown teamDropdown; // Tham chi?u ??n TMP_Dropdown
    public GameObject thongBao;
    public GameObject batdauao;
    public GameObject batdauthat;
    void Start()
    {
        // L?ng nghe s? thay ??i giá tr? c?a dropdown
        teamDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Hàm này s? ch?y m?i khi giá tr? c?a dropdown thay ??i
    void OnDropdownValueChanged(int index)
    {
        // L?y tên c?a option hi?n t?i
        string selectedOption = teamDropdown.options[index].text;

        // In ra giá tr? hi?n t?i (ch? s? và tên)
        Debug.Log("Ch? s?: " + index + ", Giá tr?: " + selectedOption);
        int redTeamCount = 0;
        int blueTeamCount = 0;

        // L?y danh sách t?t c? ng??i ch?i trong ph?ng
        Player[] players = PhotonNetwork.PlayerList;

        // Duy?t qua t?ng ng??i ch?i
        foreach (Player player in players)
        {
            // Ki?m tra n?u ng??i ch?i có Custom Properties ch?a key "team"
            if (player.CustomProperties.TryGetValue("team", out var teamValue))
            {
                int team = (int)teamValue; // Chuy?n ??i giá tr? sang ki?u int

                // ??m s? l??ng ng??i ch?i thu?c team ?? ho?c xanh
                if (team == 0) // ??i ??
                {
                    redTeamCount++;
                }
                else if (team == 1) // ??i xanh
                {
                    blueTeamCount++;
                }
            }
        }
        // Ki?m tra giá tr? và th?c hi?n hành ??ng
        if (index == 0)
        {
            thongBao.SetActive(false);
            batdauthat.SetActive(true);
            batdauao.SetActive(false);
            if (redTeamCount >= 1)
            {
                thongBao.SetActive(true);
                batdauthat.SetActive(false);
                batdauao.SetActive(true);
            }
            // Th?c hi?n hành ??ng cho ??i t?n công
            Debug.Log("Ng??i ch?i ch?n ??i t?n công");
        }
        else if (index == 1)
        {
            thongBao.SetActive(false);
            batdauthat.SetActive(true);
            batdauao.SetActive(false);
            if (blueTeamCount >= 1)
            {
                thongBao.SetActive(true);
                batdauthat.SetActive(false);
                batdauao.SetActive(true);

            }
            // Th?c hi?n hành ??ng cho ??i phòng th?
            Debug.Log("Ng??i ch?i ch?n ??i phòng th?");
        }
    }
}
