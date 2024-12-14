using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gamechat : MonoBehaviour
{
    public TextMeshProUGUI chatText; // Hi?n th? n?i dung chat
    public TMP_InputField inputField; // Tr??ng nh?p v?n b?n

    private bool isInputFieldToggled;

    void Start()
    {
        // K?t n?i t?i Photon n?u ch?a k?t n?i
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // K?t n?i t?i Photon Cloud
            Debug.Log("Connecting to Photon...");
        }
    }

    void Update()
    {
        // B?t input field khi nh?n phím Y
        if (Input.GetKeyDown(KeyCode.Y) && !isInputFieldToggled)
        {
            isInputFieldToggled = true;
            inputField.Select();
            inputField.ActivateInputField();
            Debug.Log("Toggled on");
        }

        // T?t input field khi nh?n phím Escape
        if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
        {
            isInputFieldToggled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Toggled off");
        }

        // G?i tin nh?n khi nh?n Enter/Return
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && isInputFieldToggled && !string.IsNullOrEmpty(inputField.text))
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Cannot send messages when not connected. Please connect to Photon.");
                return;
            }

            string messageToSend = $"{PhotonNetwork.LocalPlayer.NickName}: {inputField.text}";

            // G?i RPC t?i t?t c? ng??i ch?i
            GetComponent<PhotonView>().RPC("SendChatMessage", RpcTarget.All, messageToSend);

            // Xóa input field và t?t ch? ?? nh?p
            inputField.text = "";
            isInputFieldToggled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            Debug.Log("Message sent");
        }
    }

    [PunRPC]
    public void SendChatMessage(string _message)
    {
        // C?p nh?t n?i dung chat
        chatText.text += "\n" + _message;
    }

    // Callback khi k?t n?i t?i Photon thành công
    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby(); // Tham gia m?t lobby n?u c?n
    }
}
