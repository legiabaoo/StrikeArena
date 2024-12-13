using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class ThamGiaLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
      
    
}
    public void lobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // K?t n?i l?i n?u không k?t n?i
            PhotonNetwork.ConnectUsingSettings();
        }

        if (PhotonNetwork.InLobby)
        {
            Debug.LogError("tham gia lai lobby nè");
            // Tham gia l?i lobby ?? nh?n danh sách phòng
           
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
