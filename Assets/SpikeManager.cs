using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpikeManager : MonoBehaviour
{
    public static SpikeManager instance1;
    private void Awake()
    {
        instance1 = this;
    }
    public void PrintPlayerProperties()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;

            // Kiểm tra nếu hashtable không rỗng
            if (properties != null && properties.Count > 0)
            {
                string playerInfo = $"Player {player.NickName} Properties:";

                foreach (DictionaryEntry entry in properties)
                {
                    playerInfo += $"\n{entry.Key}: {entry.Value}";
                }

                Debug.Log(playerInfo);
            }
            else
            {
                Debug.Log($"Player {player.NickName} has no custom properties.");
            }
        }
    }

}
