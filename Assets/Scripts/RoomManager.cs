using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

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

    public Dropdown dropdownManager;
    public GameObject attackTeamPrefab;  // Prefab cho ??i t?n c�ng
    public GameObject defenseTeamPrefab; // Prefab cho ??i ph�ng th?

    [Space]
    public Transform[] attackSpawnPoints; // ?i?m spawn cho ??i t?n c�ng
    public Transform[] defenseSpawnPoints; // ?i?m spawn cho ??i ph�ng th?

    private GameObject currentPlayer;
    public TimeManager timeManager;// L?u tr? tham chi?u ??n nh�n v?t hi?n t?i

    public GameObject TeamDoThang;
    public GameObject TeamXanhThang;

    health health;
    private void Awake()
    {
        instance = this;
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

        Debug.Log("Ket Noi ...");
        PhotonNetwork.ConnectUsingSettings();
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
        thoigian.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Dang Ket Noi Sever...");
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        Debug.Log("Dang ket noi va o trong phong ngay bay gio");

    }
    public override void OnJoinedRoom()
    {

        base.OnJoinedRoom();

        camRoom.SetActive(false);

        HandleTeamSelection();

        Debug.Log("Number of players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Debug.Log("You are currently in the dev region: " + PhotonNetwork.CloudRegion);

    }

    public void ResPawnPlayer()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<health>().isLocalPlayer = true;
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;

    }


    public void HandleTeamSelection()
    {
        // L?y gi� tr? team ???c ch?n t? DropdownManager
        int selectedTeam = dropdownManager.teamDropdown.value; // L?y ch? s? c?a team ???c ch?n

        Transform spawnPoint;
        GameObject teamPrefab;

        if (selectedTeam == 0) // N?u ??i t?n c�ng ???c ch?n
        {
            Debug.Log("??i t?n c�ng ???c ch?n.");
            spawnPoint = attackSpawnPoints[Random.Range(0, attackSpawnPoints.Length)];
            teamPrefab = attackTeamPrefab; // Nh�n v?t cho ??i t?n c�ng
        }
        else // N?u ??i ph�ng th? ???c ch?n
        {
            Debug.Log("??i ph�ng th? ???c ch?n.");
            spawnPoint = defenseSpawnPoints[Random.Range(0, defenseSpawnPoints.Length)];
            teamPrefab = defenseTeamPrefab; // Nh�n v?t cho ??i ph�ng th?
        }

        // T?o nh�n v?t t?i ?i?m spawn t??ng ?ng v� ??ng b? h�a gi?a t?t c? ng??i ch?i
        GameObject _player = PhotonNetwork.Instantiate(teamPrefab.name, spawnPoint.position, spawnPoint.rotation);

        // G�n team cho ng??i ch?i trong Custom Properties
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["isAlive"] = true;
        hash["team"] = selectedTeam;
        hash["spawnPoint"] = spawnPoint.position;// G�n team v�o Custom Properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // ??t t�n ng??i ch?i
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<health>().isLocalPlayer = true;

        PhotonNetwork.LocalPlayer.NickName = nickname;
        
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

        // L?y danh s�ch t?t c? ng??i ch?i trong ph�ng
        Player[] players = PhotonNetwork.PlayerList;

        // Duy?t qua t?ng ng??i ch?i
        foreach (Player player in players)
        {
            // Ki?m tra n?u ng??i ch?i c� Custom Properties ch?a key "team"
            if (player.CustomProperties.TryGetValue("team", out var teamValue))
            {
                int team = (int)teamValue; // Chuy?n ??i gi� tr? sang ki?u int

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
        if(redTeamCount > 0 && blueTeamCount > 0)
        {
            if(redTeamCount == 0)
            {
                TeamXanhThang.SetActive(true);
            }
            if(blueTeamCount == 0)
            {
                TeamDoThang.SetActive(true);
            }
        }
      
       
    }
    public void UpdatePlayerStatus(bool isAlive)
    {
        Hashtable hash = new Hashtable();
        hash["isAlive"] = isAlive;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        CheckRedTeamStatus();
    }
   
    public void CheckRedTeamStatus()
    {
        int aliveRedCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("team", out var teamValue) && (int)teamValue == 0)
            {
                if (player.CustomProperties.TryGetValue("isAlive", out var isAliveValue) && (bool)isAliveValue)
                {
                    aliveRedCount++;
                }
            }
        }

        if (aliveRedCount == 0)
        {
            // N?u t?t c? th�nh vi�n ??i ?? ?� ch?t, reset th?i gian
           
            timeManager.EndGame();
        }
    }
}
