using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // RPC để đặt tag Spike
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
    // RPC để hủy spike
    [PunRPC]
    private void DestroySpike(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);
        if (spikeView != null)
        {
            Destroy(spikeView.gameObject); // Hủy spike trên tất cả các client
            Debug.Log("Spike defused on all clients!");
        }
        else
        {
            Debug.LogError("Spike not found with ViewID: " + spikeViewID);
        }
        //holdTime = 0.0f; // Reset thời gian
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
