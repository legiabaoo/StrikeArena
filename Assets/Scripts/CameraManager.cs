using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviourPunCallbacks
{
    private List<GameObject> playerCameras = new List<GameObject>();
    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
    }
    [PunRPC]
    public void GetAllPlayerCameras()
    {
        playerCameras.Clear();
        //Debug.Log("Tổng số người chơi: " + PhotonNetwork.PlayerList.Length);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //Debug.Log("Checking player: " + player.NickName + ", ActorNumber: " + player.ActorNumber);

            // Thay vì sử dụng ActorNumber, hãy lấy ViewID của đối tượng
            int viewID = player.ActorNumber * 1000 + 1; // Giả sử bạn gán ViewID trùng với ActorNumber trong lúc tạo

            GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

            if (playerObject != null)
            {
                //Debug.Log($"Found player object for ViewID {viewID}: {playerObject.name}");

                // Giả sử bạn đã có camera trong đối tượng người chơi
                //Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
                //if (playerCamera != null)
                //{
                playerCameras.Add(playerObject);
                //Debug.Log($"Camera found for {playerObject.name}");
                //}
                //else
                //{
                //    Debug.Log($"No camera found in {playerObject.name}");
                //}
            }
            else
            {
                //Debug.Log($"No player object found for ViewID {viewID}");
            }
        }

        Debug.Log("Tổng số người chơi(gameObject): " + playerCameras.Count);
    }




    public void SwitchToTeammateCamera(GameObject currentPlayer)
    {
        foreach (GameObject obj in playerCameras)
        {
            if (obj != currentPlayer) // Tìm camera đồng đội
            {
                // Tắt camera hiện tại
                Camera currentPlayerCamera = currentPlayer.GetComponentInChildren<Camera>();
                if (currentPlayerCamera != null)
                {
                    currentPlayerCamera.enabled = false; // Tắt camera người chơi hiện tại
                    currentPlayerCamera.GetComponent<MouseLook>().enabled = false;
                }

                // Bật camera của đồng đội
                Camera teammateCamera = obj.GetComponentInChildren<Camera>();
                if (teammateCamera != null)
                {
                    teammateCamera.enabled = true; // Bật camera đồng đội
                    teammateCamera.GetComponent<AudioListener>().enabled = false;
                    currentPlayer.transform.rotation = teammateCamera.transform.rotation;
                }

                break;
            }
        }
    }



    public List<GameObject> GetPlayerCameras()
    {
        return playerCameras;
    }
}
