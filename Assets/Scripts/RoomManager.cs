using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Unity.VisualScripting;
using TMPro;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject thoigian;
    public static RoomManager instance;
    public GameObject player;
    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject camRoom;
    [Space]
    private string nickname = "unnamed";
    public GameObject nameUI;
    public GameObject connectingUI;
    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;

    public Renderer playerRender;
    public string roomNameToJoin = "test";

    public TMP_InputField username;

    public Dropdown dropdownManager;
    public GameObject attackTeamPrefab;  // Prefab cho ??i t?n công
    public GameObject defenseTeamPrefab; // Prefab cho ??i pḥng th?

    [Space]
    public Transform[] attackSpawnPoints; // ?i?m spawn cho ??i t?n công
    public Transform[] defenseSpawnPoints; // ?i?m spawn cho ??i pḥng th?

    private GameObject currentPlayer;
    public TimeManager timeManager;// L?u tr? tham chi?u ??n nhân v?t hi?n t?i

    public GameObject TeamDoThang;
    public GameObject TeamXanhThang;

    public bool hasCalledEndGame = false;
    public bool isCountTeam = false;
    public GameObject gunshop;
    public bool isSpike = false;

    health health;
    private void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("Username"))
        {
            username.text = PlayerPrefs.GetString("Username");
            Debug.Log(username.text);
            Debug.Log(PlayerPrefs.GetString("Username"));
        }
    }
    void Update()
    {
        CountPlayersInTeams();
    }
    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }
    public void JoinRoomButtonPressed()
    {
        Debug.Log("Kết nối ...");

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
            nameUI.SetActive(false);
            connectingUI.SetActive(true);
            thoigian.SetActive(true);
            gunshop.GetComponent<GunShop>().enabled = true;
        }
        else
        {
            Debug.LogWarning("Chưa kết nối được với Photon!");
        }
    }


    public override void OnJoinedRoom()
    {

        base.OnJoinedRoom();

        camRoom.SetActive(false);

        HandleTeamSelection();
        //if (countSpike == 0)
        //{
        //    TimeManager.instance.isSpawnSpike = false;
        //    photonView.RPC("CountSpike", RpcTarget.AllBuffered);
        //}

        Debug.Log("Number of players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Debug.Log("You are currently in the dev region: " + PhotonNetwork.CloudRegion);
        CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);

    }
    //[PunRPC]
    //public void CountSpike()
    //{
    //    countSpike++;
    //}
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        // Gọi lại để cập nhật danh sách camera khi có người chơi mới
        CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);
    }

    public void HandleTeamSelection()
    {
        int selectedTeam = dropdownManager.teamDropdown.value; // Lấy chỉ số của team được chọn

        // Lấy các điểm spawn tương ứng với team
        Transform[] teamSpawnPoints = selectedTeam == 0 ? attackSpawnPoints : defenseSpawnPoints;
        GameObject teamPrefab = selectedTeam == 0 ? attackTeamPrefab : defenseTeamPrefab;

        Transform spawnPoint;

        // Kiểm tra nếu người chơi đã có vị trí spawn ban đầu
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("initialSpawnPoint", out var savedPosition))
        {
            spawnPoint = new GameObject("SavedSpawnPoint").transform;
            spawnPoint.position = (Vector3)savedPosition;
        }
        else
        {
            spawnPoint = GetAvailableSpawnPoint(teamSpawnPoints);
            if (spawnPoint == null)
            {
                Debug.LogError("Tất cả các điểm spawn đã bị chiếm.");
                return;
            }

            // Lưu vị trí spawn ban đầu
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "initialSpawnPoint", spawnPoint.position } });
        }

        // Tạo nhân vật tại điểm spawn và đồng bộ hóa với tất cả người chơi
        GameObject _player = PhotonNetwork.Instantiate(teamPrefab.name, spawnPoint.position, spawnPoint.rotation);
        int playerViewID = _player.GetComponent<PhotonView>().ViewID;
        // Thiết lập thuộc tính cho người chơi hiện tại
        Hashtable hash = new Hashtable
    {
        { "isAlive", true },
        { "team", selectedTeam },
        { "spawnPoint", spawnPoint.position },
        { "viewID", playerViewID }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // Đặt tên cho người chơi và thiết lập camera, vị trí vũ khí
        //_player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<health>().isLocalPlayer = true;

        PhotonNetwork.LocalPlayer.NickName = nickname;
        GameObject currentPlayerObject = gameObject;
        CameraManager.instance.RespawnPlayerCamera(currentPlayerObject);
        GunShop.instance.ResetGunPosition();
    }

    private Transform GetAvailableSpawnPoint(Transform[] spawnPoints)
    {
        List<Transform> availableSpawnPoints = new List<Transform>();

        // Lọc danh sách các điểm spawn chưa có ai sử dụng
        foreach (Transform point in spawnPoints)
        {
            bool isOccupied = false;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("spawnPoint", out var spawnPosition) &&
                    (Vector3)spawnPosition == point.position)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                availableSpawnPoints.Add(point);
            }
        }

        // Chọn ngẫu nhiên một điểm từ danh sách các điểm spawn trống
        if (availableSpawnPoints.Count > 0)
        {
            return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        }

        // Trả về null nếu không còn điểm trống nào
        return null;
    }


    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {

        }
    }

    public void CountPlayersInTeams()
    {
        int redTeamCount = 0;
        int blueTeamCount = 0;

        // L?y danh sách t?t c? ng??i ch?i trong pḥng
        Player[] players = PhotonNetwork.PlayerList;

        // Duy?t qua t?ng ng??i ch?i
        foreach (Player player in players)
        {
            // Ki?m tra n?u ng??i ch?i có Custom Properties ch?a key "team"
            if (player.CustomProperties.TryGetValue("team", out var teamValue))
            {
                int team = (int)teamValue; // Chuy?n ??i giá tr? sang ki?u int

                // ??m s? l??ng ng??i ch?i thu?c team ?? ho?c xanh
                if (team == 0) // ??i ??
                {
                    redTeamCount++;
                }
                else if (team == 1) // ??i xanh
                {
                    blueTeamCount++;
                }
            }
        }
        if (redTeamCount >= 1 && blueTeamCount >= 1)
        {
            TimeManager.instance.startGame = true;
        }
    }
    public void UpdatePlayerStatus(bool isAlive)
    {
        Hashtable properties = new Hashtable
            {
                { "isAlive", isAlive }
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

    }
    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //{
    //    if (PhotonNetwork.IsMasterClient) // Chỉ cho MasterClient kiểm tra
    //    {
    //        if (changedProps.ContainsKey("isAlive"))
    //        {
    //            CheckRedTeamStatus();
    //        }
    //    }
    //    else
    //    {
    //        // Máy khách khác chỉ cần cập nhật camera mà không gọi kiểm tra
    //        CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);
    //    }
    //}
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Custom Properties updated: " + targetPlayer.CustomProperties.ToStringFull());

        if (targetPlayer.CustomProperties.ContainsKey("isAlive"))
        {
            if (PhotonNetwork.IsMasterClient) // Chỉ gọi từ Master Client và kiểm tra cờ
            {
                CheckRedTeamStatus();
            }
            CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);
        }
    }



    public void CheckRedTeamStatus()
    {
        if (hasCalledEndGame) return; // Dừng nếu hàm đã được gọi

        bool allDeadRed = true;
        bool allDeadBlue = true;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("team", out var teamValue) &&
                player.CustomProperties.TryGetValue("isAlive", out var isAliveValue) &&
                (bool)isAliveValue)
            {
                if ((int)teamValue == 0) allDeadRed = false;
                if ((int)teamValue == 1) allDeadBlue = false;
            }
        }

        if ((allDeadRed || allDeadBlue) && !hasCalledEndGame && PhotonNetwork.IsMasterClient && !TimeManager.instance.isGameOver && TimeManager.instance.startGame)
        {
            hasCalledEndGame = true;
            if (allDeadRed)
            {
                Debug.Log("Đội đỏ đã chết hết");
                TimeManager.instance.isAllDeathRed = true;
            }
            else if (allDeadBlue)
            {
                Debug.Log("Đội xanh đã chết hết");
                TimeManager.instance.isAllDeathBlue = true;
            }
        }
    }


    public void RemovePlayerInstances()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("OldPlayer");

        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("RequestDestroyPlayer", RpcTarget.AllBuffered, photonView.ViewID);
            }
        }
    }
}
