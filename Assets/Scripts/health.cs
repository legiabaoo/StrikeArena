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
        healthText.text = healths.ToString();
        healths -= dame;
        if(healths <= 0)
        {

            if (isLocalPlayer)
            {

                RoomManager.instance.ResPawnPlayer();

                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }
            Destroy(gameObject);    
        }
    }

}
