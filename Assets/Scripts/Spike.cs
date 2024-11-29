using DevionGames;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spike : MonoBehaviour
{
    
    //Đặt tag Spike
    [PunRPC]
    public void SetSpikeTag(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);

        if (spikeView != null)
        {
            GameObject spike = spikeView.gameObject;
            spike.tag = "Spike"; // Đặt tag cho Spike trên tất cả các máy
            Debug.Log("Tag set successfully on all clients");
        }
        else
        {
            Debug.LogError("Spike object not found with the given ViewID: " + spikeViewID);
        }
    }
    [PunRPC]
    public void SetSpike0Tag(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);

        if (spikeView != null)
        {
            GameObject spike = spikeView.gameObject;
            spike.tag = "Spike0"; // Đặt tag cho Spike trên tất cả các máy
            Debug.Log("Tag set successfully on all clients");
        }
        else
        {
            Debug.LogError("Spike object not found with the given ViewID: " + spikeViewID);
        }
    }
    //Gỡ spike 
    [PunRPC]
    private void RemoveSpike(int spikeViewID)
    {
        GameObject spike = PhotonView.Find(spikeViewID).gameObject;
        if (spike != null)
        {
            Destroy(spike); 
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
