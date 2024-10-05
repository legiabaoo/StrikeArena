using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Roomlist : MonoBehaviourPunCallbacks
{
    public static Roomlist Instance;

    public GameObject roomManagerGameobject;
    public RoomManager roomManager; 
    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    public TMP_InputField roomNameInputField;
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();


    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        // Ki?m tra xem t�n ph�ng c� h?p l? kh�ng
        if (!string.IsNullOrEmpty(roomName))
        {
            // G?i ph??ng th?c trong RoomManager ?? t?o ph�ng
           
            Debug.Log("Tao phong thanh cong");
        }
        else
        {
            Debug.LogError("T�n ph�ng kh�ng h?p l?!");
        }
    }
    public void ChangRoomToCreateName(string _roomName)
    {
        roomManager.roomNameToJoin = _roomName;
    }
    private void Awake()
    {
        Instance = this;
    }
    private IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();

        }
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // T?o m?t danh s�ch m?i ?? l?u tr? danh s�ch ph�ng ?� c?p nh?t
        List<RoomInfo> updatedRoomList = new List<RoomInfo>(roomList);

        foreach (var room in roomList)
        {
            Debug.Log("Room Name: " + room.Name);
            // T�m ki?m ph�ng trong danh s�ch cachedRoomList
            int index = cachedRoomList.FindIndex(r => r.Name == room.Name);
            if (index != -1)
            {
                // N?u ph�ng ?� t?n t?i trong danh s�ch cachedRoomList
                if (room.RemovedFromList)
                {
                    // N?u ph�ng ?� b? x�a kh?i danh s�ch, lo?i b? n� kh?i danh s�ch c?p nh?t
                    updatedRoomList.RemoveAt(index);
                }
                else
                {
                    // N?u kh�ng, c?p nh?t th�ng tin c?a ph�ng trong danh s�ch c?p nh?t
                    updatedRoomList[index] = room;
                }
            }
            else
            {
                // N?u ph�ng kh�ng t?n t?i trong danh s�ch cachedRoomList v� kh�ng b? x�a kh?i danh s�ch,
                // th�m n� v�o danh s�ch c?p nh?t
                if (!room.RemovedFromList)
                {
                    updatedRoomList.Add(room);
                }
            }
        }

        // C?p nh?t cachedRoomList v?i danh s�ch ?� c?p nh?t
        cachedRoomList = updatedRoomList;

        // C?p nh?t giao di?n ng??i d�ng
        UpdateUI();
    }

   /* public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach(var room in roomList)
            {
                Debug.Log("Room Name: " + room.Name);
                for (int i = 0; i< cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i]= room;
                        }
                        cachedRoomList = newList;


                    }
                }
            }
        }
        UpdateUI();
    }
*/
    private void UpdateUI()
    {
        Debug.Log("Number of rooms in cached list: " + cachedRoomList.Count);
      
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }
        foreach(var room in cachedRoomList)
        {
           GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            Debug.Log("Room Name: " + room.Name);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount+"/16";

            roomItem.GetComponent<RoomItemButton>().Roomname = room.Name;
        }
    }
    public void JoinRoomByName(string _name)
    {
        roomManager.roomNameToJoin = _name;
        roomManagerGameobject.SetActive(true);
        gameObject.SetActive(false);
    }
}
