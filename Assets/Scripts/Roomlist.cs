using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class Roomlist : MonoBehaviourPunCallbacks
{
    public static Roomlist Instance;

    public GameObject roomManagerGameobject;
    public RoomManager roomManager;

    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    public TMP_InputField roomNameInputField;
    public TMP_Dropdown[] dropDownSL;

    [Header("Control")]
    public string AttackText="0";
    public string DefText="0";

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    public GameObject taoPhong;
    public GameObject chondoi;
    public GameObject thongbao;
    public GameObject thongbao1;
    public RoomItemButton roomItem;
    private void Update()
    {
        roomNameInputField.onValueChanged.AddListener(thongbaoUI);

    }
    public int GetSelectedMaxPlayers()
    {
        if (dropDownSL != null && dropDownSL[0].options.Count > 0 && dropDownSL[1].options.Count > 0)
        {
            //AttackText = dropDownSL[0].options[dropDownSL[0].value].text;
            //DefText = dropDownSL[1].options[dropDownSL[1].value].text;
            //if (byte.TryParse(selectedText, out byte maxPlayers))
            //{

            //}
            //if (
            //PhotonNetwork.IsMasterClient) {
            //photonView.RPC("SyncDropDown", RpcTarget.AllBuffered, AttackText, DefText);
            //}
            
            int maxPlayers = int.Parse(dropDownSL[0].options[dropDownSL[0].value].text);
            Debug.Log(maxPlayers);
            return maxPlayers;
        }
        Debug.LogError("Failed to get max players. Check TMP_Dropdown setup.");
        return 0; // Trả về giá trị mặc định nếu có lỗi
    }
    [PunRPC]
    public void SyncDropDown(string A, string D)
    {
        AttackText = A;
        DefText = D;
    }
    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        foreach (var room in cachedRoomList)
        {
            if (room.Name == roomName)
            {
                Debug.Log("Phong da ton tai");
                thongbao1.SetActive(true);

                return;

            }
        }

        // Ki?m tra xem tên pḥng có h?p l? không
        if (!string.IsNullOrEmpty(roomName))
        {
            // G?i ph??ng th?c trong RoomManager ?? t?o pḥng
            Debug.Log("Tao phong thanh cong");
            RoomManager.instance.JoinRoomButtonPressed();
            chondoi.SetActive(true);
            taoPhong.SetActive(false);

        }
        else
        {
            thongbao.SetActive(true);
            Debug.LogError("Tên phong không hop li");
            return;
        }
    }
    public void thongbaoUI(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            thongbao.SetActive(false);
            thongbao1.SetActive(false);
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
    private void OnMaxPlayersChanged(int index)
    {

    }
    private IEnumerator Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // Kết nối lại nếu không kết nối

            PhotonNetwork.ConnectUsingSettings();
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.LogError("tham gia lai lobby");
            // Tham gia lại lobby để nhận danh sách phòng
            PhotonNetwork.JoinLobby();
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();

        }
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia"; // Chỉ định vùng
        PhotonNetwork.ConnectUsingSettings(); // Kết nối với vùng đã chọn
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }
    /*  public override void OnRoomListUpdate(List<RoomInfo> roomList)
      {
          // T?o m?t danh sách m?i ?? l?u tr? danh sách pḥng ?ă c?p nh?t
          List<RoomInfo> updatedRoomList = new List<RoomInfo>(roomList);

          foreach (var room in roomList)
          {
              Debug.Log("Room Name: " + room.Name);
              // T́m ki?m pḥng trong danh sách cachedRoomList
              int index = cachedRoomList.FindIndex(r => r.Name == room.Name);
              if (index != -1)
              {
                  // N?u pḥng ?ă t?n t?i trong danh sách cachedRoomList
                  if (room.RemovedFromList)
                  {
                      // N?u pḥng ?ă b? xóa kh?i danh sách, lo?i b? nó kh?i danh sách c?p nh?t
                      updatedRoomList.RemoveAt(index);
                  }
                  else
                  {
                      // N?u không, c?p nh?t thông tin c?a pḥng trong danh sách c?p nh?t
                      updatedRoomList[index] = room;
                  }
              }
              else
              {
                  // N?u pḥng không t?n t?i trong danh sách cachedRoomList và không b? xóa kh?i danh sách,
                  // thêm nó vào danh sách c?p nh?t
                  if (!room.RemovedFromList)
                  {
                      updatedRoomList.Add(room);
                  }
              }
          }

          // C?p nh?t cachedRoomList v?i danh sách ?ă c?p nh?t
          cachedRoomList = updatedRoomList;

          // C?p nh?t giao di?n ng??i dùng
          UpdateUI();
      }*/
    public void LoadSceen1()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                Debug.Log("Room Name: " + room.Name);
                for (int i = 0; i < cachedRoomList.Count; i++)
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
                            newList[i] = room;
                        }
                        cachedRoomList = newList;


                    }
                }
            }
        }


        UpdateUI();

    }

    private void UpdateUI()
    {
        Debug.Log("Number of rooms in cached list: " + cachedRoomList.Count);

        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }
        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            Debug.Log("Room Name: " + room.Name);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;

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
