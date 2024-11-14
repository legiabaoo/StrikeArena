using Photon.Pun;
using Photon.Realtime;
using scgFullBodyController;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private List<GameObject> playerObjects = new List<GameObject>();
    public static CameraManager instance;
    private int currentTeammateIndex = 0;
    private Quaternion currentTeammateRotation; // Để lưu rotation từ camera đồng đội

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwitchToNextTeammateCamera(); // Chuyển camera qua đồng đội
        }

    }

    [PunRPC]
    public void GetAllPlayerCameras()
    {
        playerObjects.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int viewID = player.ActorNumber * 1000 + 1;
            GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

            // Kiểm tra nếu player cùng đội với người chơi hiện tại
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("team", out var localTeamValue) &&
            player.CustomProperties.TryGetValue("team", out var teamValue) &&
            (int)teamValue == (int)localTeamValue && player.CustomProperties.TryGetValue("isAlive", out var isAliveValue) && (bool)isAliveValue)
            {
                if (playerObject != null)
                {
                    playerObjects.Add(playerObject);
                }
            }
        }

        Debug.Log("Tổng số người chơi thuộc đội đỏ (gameObject): " + playerObjects.Count);
    }

    public void SwitchToTeammateCamera(GameObject currentPlayer)
    {
        foreach (GameObject obj in playerObjects)
        {
            if (obj != currentPlayer)
            {
                Camera currentPlayerCamera = currentPlayer.GetComponentInChildren<Camera>();
                Canvas currentPlayerCanvas = currentPlayer.GetComponentInChildren<Canvas>();
               ThirdPersonCharacter currentPlayerController = currentPlayer.GetComponent<ThirdPersonCharacter>();
                if (currentPlayerCamera != null)
                {
                    //currentPlayer.transform.position 
                    currentPlayerCamera.enabled = false;
                    currentPlayerCamera.GetComponent<MouseLook>().enabled = false;
                    currentPlayerCamera.GetComponent<AudioListener>().enabled = false;
                    currentPlayerCanvas.enabled = false;
                    currentPlayerController.enabled = false;
                }

                Camera teammateCamera = obj.GetComponentInChildren<Camera>();
                Canvas currentTeammateCanvas = obj.GetComponentInChildren<Canvas>();
                if (teammateCamera != null)
                {
                    teammateCamera.enabled = true;
                    teammateCamera.GetComponent<AudioListener>().enabled = false;
                    currentTeammateRotation = teammateCamera.transform.rotation;
                    currentTeammateCanvas.enabled = true;
                }

                break;
            }
        }
    }
    public void SwitchToNextTeammateCamera()
    {
        if (playerObjects.Count == 0) return;

        // Tắt camera của đồng đội hiện tại
        GameObject currentPlayer = playerObjects[currentTeammateIndex];
        Camera currentPlayerCamera = currentPlayer.GetComponentInChildren<Camera>();
        Canvas currentPlayerCanvas = currentPlayer.GetComponentInChildren<Canvas>();

        if (currentPlayerCamera != null)
        {
            currentPlayerCamera.enabled = false;
            currentPlayerCamera.GetComponent<AudioListener>().enabled = false;
            currentPlayerCanvas.enabled = false;
        }

        // Tăng chỉ mục và đảm bảo vòng lại đầu nếu đạt cuối danh sách
        currentTeammateIndex = (currentTeammateIndex + 1) % playerObjects.Count;

        // Bật camera của đồng đội mới
        GameObject nextTeammate = playerObjects[currentTeammateIndex];
        Camera teammateCamera = nextTeammate.GetComponentInChildren<Camera>();
        Canvas teammateCanvas = nextTeammate.GetComponentInChildren<Canvas>();

        if (teammateCamera != null)
        {
            teammateCamera.enabled = true;
            teammateCamera.GetComponent<AudioListener>().enabled = true;
            currentTeammateRotation = teammateCamera.transform.rotation;
            teammateCanvas.enabled = true;
        }
    }
    public void RespawnPlayerCamera(GameObject respawnedPlayer)
    {
        Camera playerCamera = respawnedPlayer.GetComponentInChildren<Camera>();
        Canvas playerCanvas = respawnedPlayer.GetComponentInChildren<Canvas>();
        dichuyen PlayerController = respawnedPlayer.GetComponent<dichuyen>();
        if (playerCanvas != null)
        {
            playerCanvas.enabled = true;
        }
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.GetComponent<AudioListener>().enabled = true;
            PlayerController.enabled = true;

            // Nếu bạn có MouseLook hay các thành phần khác, hãy kích hoạt chúng ở đây
            var mouseLook = playerCamera.GetComponent<MouseLook>();
            if (mouseLook != null)
            {
                mouseLook.enabled = true;
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
