using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gamechat : MonoBehaviour
{
    public TextMeshProUGUI chatText; // Hiển thị nội dung chat
    public TMP_InputField inputField; // Trường nhập văn bản

    private bool isInputFieldToggled;

    void Start()
    {
        // Kết nối tới Photon nếu chưa kết nối
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Kết nối tới Photon Cloud
            Debug.Log("Connecting to Photon...");
        }
    }

    void Update()
    {
        // Bật/tắt input field khi nhấn Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!isInputFieldToggled) // Nếu input field chưa bật, bật nó
            {
                isInputFieldToggled = true;
                inputField.text = ""; // Đảm bảo input field luôn trống khi mở
                inputField.Select();
                inputField.ActivateInputField();
                Debug.Log("Chat input field activated.");
            }
            else if (!string.IsNullOrEmpty(inputField.text)) // Nếu input field đang bật và có nội dung, gửi tin nhắn
            {
                if (!PhotonNetwork.IsConnected)
                {
                    Debug.LogError("Cannot send messages when not connected. Please connect to Photon.");
                    return;
                }

                string messageToSend = $"{PhotonNetwork.LocalPlayer.NickName}: {inputField.text}";

                // Gửi RPC tới tất cả người chơi
                GetComponent<PhotonView>().RPC("SendChatMessage", RpcTarget.All, messageToSend);

                // Xóa input field và tắt chế độ nhập
                inputField.text = "";
                isInputFieldToggled = false;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

                Debug.Log("Message sent and chat closed.");
            }
            else // Nếu input field đang bật nhưng không có nội dung, tắt nó
            {
                isInputFieldToggled = false;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                Debug.Log("Chat input field deactivated.");
            }
        }

        // Tắt input field khi nhấn Escape
        if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
        {
            isInputFieldToggled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Chat input field deactivated.");
        }
    }

    [PunRPC]
    public void SendChatMessage(string _message)
    {
        // Cập nhật nội dung chat
        chatText.text += "\n" + _message;
    }

    // Callback khi kết nối tới Photon thành công
    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby(); // Tham gia một lobby nếu cần
    }
}
