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
        // L?ng nghe s? thay ??i gi� tr? c?a dropdown
        teamDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // H�m n�y s? ch?y m?i khi gi� tr? c?a dropdown thay ??i
    void OnDropdownValueChanged(int index)
    {
        // L?y t�n c?a option hi?n t?i
        string selectedOption = teamDropdown.options[index].text;

        // In ra gi� tr? hi?n t?i (ch? s? v� t�n)
        Debug.Log("Ch? s?: " + index + ", Gi� tr?: " + selectedOption);
        int redTeamCount = 0;
        int blueTeamCount = 0;

        // L?y danh s�ch t?t c? ng??i ch?i trong ph?ng
        Player[] players = PhotonNetwork.PlayerList;

        // Duy?t qua t?ng ng??i ch?i
        foreach (Player player in players)
        {
            // Ki?m tra n?u ng??i ch?i c� Custom Properties ch?a key "team"
            if (player.CustomProperties.TryGetValue("team", out var teamValue))
            {
                int team = (int)teamValue; // Chuy?n ??i gi� tr? sang ki?u int

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
        // Ki?m tra gi� tr? v� th?c hi?n h�nh ??ng
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
            // Th?c hi?n h�nh ??ng cho ??i t?n c�ng
            Debug.Log("Ng??i ch?i ch?n ??i t?n c�ng");
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
            // Th?c hi?n h�nh ??ng cho ??i ph�ng th?
            Debug.Log("Ng??i ch?i ch?n ??i ph�ng th?");
        }
    }
}
