using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private List<GameObject> playerObjects = new List<GameObject>();
    public static CameraManager instance;
    private Quaternion currentTeammateRotation; // Để lưu rotation từ camera đồng đội

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void GetAllPlayerCameras()
    {
        playerObjects.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int viewID = player.ActorNumber * 1000 + 1;
            GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

            if (playerObject != null)
            {
                playerObjects.Add(playerObject);
            }
        }

        Debug.Log("Tổng số người chơi(gameObject): " + playerObjects.Count);
    }

    public void SwitchToTeammateCamera(GameObject currentPlayer)
    {
        foreach (GameObject obj in playerObjects)
        {
            if (obj != currentPlayer)
            {
                Camera currentPlayerCamera = currentPlayer.GetComponentInChildren<Camera>();
                if (currentPlayerCamera != null)
                {
                    currentPlayerCamera.enabled = false;
                    currentPlayerCamera.GetComponent<MouseLook>().enabled = false;
                    currentPlayerCamera.GetComponent<AudioListener>().enabled = false;
                }

                Camera teammateCamera = obj.GetComponentInChildren<Camera>();
                if (teammateCamera != null)
                {
                    teammateCamera.enabled = true;
                    teammateCamera.GetComponent<AudioListener>().enabled = true;
                    currentTeammateRotation = teammateCamera.transform.rotation;
                }

                break;
            }
        }
    }

    // Liên tục gửi rotation qua mạng khi SerializeView được gọi
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Nếu đây là máy chủ của camera đồng đội, gửi rotation của camera
            stream.SendNext(currentTeammateRotation);
        }
        else
        {
            // Nhận rotation từ máy chủ và cập nhật cho camera của người chơi đã chết
            currentTeammateRotation = (Quaternion)stream.ReceiveNext();

            foreach (GameObject obj in playerObjects)
            {
                Camera teammateCamera = obj.GetComponentInChildren<Camera>();
                if (teammateCamera != null && !teammateCamera.enabled) // Chỉ cập nhật cho camera của người chơi đã chết
                {
                    teammateCamera.transform.rotation = currentTeammateRotation;
                }
            }
        }
    }

    public List<GameObject> GetPlayerCameras()
    {
        return playerObjects;
    }
}
