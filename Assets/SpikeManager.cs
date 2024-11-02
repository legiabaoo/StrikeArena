using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeManager : MonoBehaviourPunCallbacks
{
    public static SpikeManager instance;
    public bool spikeExists = false;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (!spikeExists) RemoveSpikeFromScene();
        }
    }
    private void RemoveSpikeFromScene()
    {
        GameObject spike = GameObject.FindWithTag("Spike");
        if (spike != null) Destroy(spike);
    }
    [PunRPC]
    public void SetIsSpikeExists(bool isSpikeExists)
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            { "SpikeExists", isSpikeExists }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    [PunRPC]
    public void ReateIsSpikeExists(bool isSpikeExists)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SpikeExists", isSpikeExists } });
    }
    
}
