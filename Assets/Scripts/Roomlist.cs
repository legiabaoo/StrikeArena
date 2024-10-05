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

        // Ki?m tra xem tên phòng có h?p l? không
        if (!string.IsNullOrEmpty(roomName))
        {
            // G?i ph??ng th?c trong RoomManager ?? t?o phòng
           
            Debug.Log("Tao phong thanh cong");
        }
        else
        {
            Debug.LogError("Tên phòng không h?p l?!");
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
        // T?o m?t danh sách m?i ?? l?u tr? danh sách phòng ?ã c?p nh?t
        List<RoomInfo> updatedRoomList = new List<RoomInfo>(roomList);

        foreach (var room in roomList)
        {
            Debug.Log("Room Name: " + room.Name);
            // Tìm ki?m phòng trong danh sách cachedRoomList
            int index = cachedRoomList.FindIndex(r => r.Name == room.Name);
            if (index != -1)
            {
                // N?u phòng ?ã t?n t?i trong danh sách cachedRoomList
                if (room.RemovedFromList)
                {
                    // N?u phòng ?ã b? xóa kh?i danh sách, lo?i b? nó kh?i danh sách c?p nh?t
                    updatedRoomList.RemoveAt(index);
                }
                else
                {
                    // N?u không, c?p nh?t thông tin c?a phòng trong danh sách c?p nh?t
                    updatedRoomList[index] = room;
                }
            }
            else
            {
                // N?u phòng không t?n t?i trong danh sách cachedRoomList và không b? xóa kh?i danh sách,
                // thêm nó vào danh sách c?p nh?t
                if (!room.RemovedFromList)
                {
                    updatedRoomList.Add(room);
                }
            }
        }

        // C?p nh?t cachedRoomList v?i danh sách ?ã c?p nh?t
        cachedRoomList = updatedRoomList;

        // C?p nh?t giao di?n ng??i dùng
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
