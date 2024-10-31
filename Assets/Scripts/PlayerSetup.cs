using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerSetup : MonoBehaviour
{
    public dichuyen movemnet;
    public Camera cameraPlayer;
    public Canvas canvas;
    public string nickname;
    public TextMeshPro nickNameText;
    private PhotonView photonView;
    [SerializeField]
    private int actorNumber; // Biến này sẽ hiển thị trong Inspector

    void Awake()
    {
        photonView = GetComponent<PhotonView>(); // Gán photonView ở đây
        cameraPlayer.enabled = false;
        cameraPlayer.GetComponent<AudioListener>().enabled = false;
    }
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer != null)
        {
            actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    public void IsLocalPlayer()
    {
        movemnet.enabled = true;
        cameraPlayer.enabled = true;
        canvas.gameObject.SetActive(true);
        cameraPlayer.GetComponent<AudioListener>().enabled = false;
    }
    public void OnPlayerDeath()
    {
        // Tắt nhân vật
        gameObject.SetActive(false);

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
