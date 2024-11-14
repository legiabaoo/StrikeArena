using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Unity.VisualScripting;
using TMPro;

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

    public TMP_Text username;

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

    health health;
    private void Awake()
    {
        instance = this;

    }
    void Update()
    {
        CountPlayersInTeams();
        //if (TimeManager.instance.startGame && !TimeManager.instance.isGameOver)
        //{
        //CheckRedTeamStatus();
        //}
    }
    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }
    public void JoinRoomButtonPressed()
    {

        Debug.Log("Ket Noi ...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
        thoigian.SetActive(true);
    }

    public override void OnJoinedRoom()
    {

        base.OnJoinedRoom();

        camRoom.SetActive(false);

        HandleTeamSelection();

        Debug.Log("Number of players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Debug.Log("You are currently in the dev region: " + PhotonNetwork.CloudRegion);
        CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        // Gọi lại để cập nhật danh sách camera khi có người chơi mới
        CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);
    }

    public void HandleTeamSelection()
    {
        // L?y giá tr? team ???c ch?n t? DropdownManager
        int selectedTeam = dropdownManager.teamDropdown.value; // L?y ch? s? c?a team ???c ch?n

        Transform spawnPoint;
        GameObject teamPrefab;

        if (selectedTeam == 0) // N?u ??i t?n công ???c ch?n
        {
            Debug.Log("??i t?n công ???c ch?n.");
            spawnPoint = attackSpawnPoints[Random.Range(0, attackSpawnPoints.Length)];
            teamPrefab = attackTeamPrefab; // Nhân v?t cho ??i t?n công
        }
        else // N?u ??i pḥng th? ???c ch?n
        {
            Debug.Log("??i pḥng th? ???c ch?n.");
            spawnPoint = defenseSpawnPoints[Random.Range(0, defenseSpawnPoints.Length)];
            teamPrefab = defenseTeamPrefab; // Nhân v?t cho ??i pḥng th?
        }

        // T?o nhân v?t t?i ?i?m spawn t??ng ?ng và ??ng b? hóa gi?a t?t c? ng??i ch?i
        GameObject _player = PhotonNetwork.Instantiate(teamPrefab.name, spawnPoint.position, spawnPoint.rotation);

        Hashtable hash = new Hashtable
        {
            { "isAlive", true },
            { "team", selectedTeam },
            { "spawnPoint", spawnPoint.position }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // ??t tên ng??i ch?i
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<health>().isLocalPlayer = true;


        PhotonNetwork.LocalPlayer.NickName = nickname;
        GameObject currentPlayerObject = gameObject;
        CameraManager.instance.RespawnPlayerCamera(currentPlayerObject);
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
        //else
        //{
        //    TimeManager.instance.startGame = false;
        //}

    }
    public void UpdatePlayerStatus(bool isAlive)
    {
        Hashtable properties = new Hashtable
            {
                { "isAlive", isAlive }
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
      /*  Debug.Log("Custom Properties updated: " + targetPlayer.CustomProperties.ToStringFull());*/

        if (targetPlayer.CustomProperties.ContainsKey("isAlive"))
        {
            if (PhotonNetwork.IsMasterClient)  // Chỉ gọi từ Master Client để tránh việc gọi trùng lặp từ các máy khách
            {
                CheckRedTeamStatus();
            }
            CameraManager.instance.photonView.RPC("GetAllPlayerCameras", RpcTarget.AllBuffered);
        }
    }

    public void CheckRedTeamStatus()
    {
        int aliveRedCount = 0;
        int aliveBlueCount = 0;
        bool allDeadRed = true;  // Biến này sẽ xác nhận nếu tất cả đội đỏ đã chết
        bool allDeadBlue = true;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Kiểm tra nếu người chơi thuộc đội đỏ
            if (player.CustomProperties.TryGetValue("team", out var teamValueRed) && (int)teamValueRed == 0)
            {
                // Kiểm tra nếu người chơi còn sống
                if (player.CustomProperties.TryGetValue("isAlive", out var isAliveValue) && (bool)isAliveValue)
                {
                    aliveRedCount++;
                    allDeadRed = false;  // Có ít nhất một người chơi còn sống
                }
            }
            if (player.CustomProperties.TryGetValue("team", out var teamValueBlue) && (int)teamValueBlue == 1)
            {
                // Kiểm tra nếu người chơi còn sống
                if (player.CustomProperties.TryGetValue("isAlive", out var isAliveValue) && (bool)isAliveValue)
                {
                    aliveBlueCount++;
                    allDeadBlue = false;  // Có ít nhất một người chơi còn sống
                }
            }
        }
        Debug.Log("allDeadRed: " + allDeadRed);
        Debug.Log("allDeadBlue: " + allDeadBlue);
        // Kiểm tra nếu tất cả đội đỏ đã chết
        if (allDeadRed && TimeManager.instance.startGame && !TimeManager.instance.isGameOver && !hasCalledEndGame)
        {
            hasCalledEndGame = true;
            Debug.Log("Đội đỏ đã chết hết");
            TimeManager.instance.isAllDeathRed = true;
        }
        // Kiểm tra nếu tất cả đội xanh đã chết
        if (allDeadBlue && TimeManager.instance.startGame && !TimeManager.instance.isGameOver && !hasCalledEndGame)
        {
            hasCalledEndGame = true;
            Debug.Log("Đội xanh đã chết hết");
            TimeManager.instance.isAllDeathBlue = true;
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
