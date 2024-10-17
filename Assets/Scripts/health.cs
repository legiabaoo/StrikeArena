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
    public TextMeshProUGUI healthText;



    [PunRPC]
    public void TakeDamege(int dame)
    {
        // L?y team c?a ng??i b?n v� ng??i b? b?n
        int shooterTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        int targetTeam = (int)GetComponent<PhotonView>().Owner.CustomProperties["team"];

        // Ch? tr? m�u n?u team kh�c nhau
        if (shooterTeam != targetTeam)
        {
            healths -= dame;
            healthText.text = healths.ToString();
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
        else
        {
            Debug.Log("Kh�ng th? g�y s�t th??ng cho ??ng ??i.");
        }
    }


}
