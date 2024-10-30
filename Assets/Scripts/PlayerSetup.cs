using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerSetup : MonoBehaviour
{
    public dichuyen movemnet;
    public GameObject camera;

    public string nickname;
    public TextMeshPro nickNameText;

    public void IsLocalPlayer()
    {
        movemnet.enabled = true;
        camera.SetActive(true);
    }
    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nickNameText.text = nickname;   
    }
    [PunRPC]
    public void RequestDestroyPlayer(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            if (photonView.IsMine )
            {
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }
}
