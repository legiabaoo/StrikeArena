using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RoomManager : MonoBehaviourPunCallbacks
{
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
    public GameObject attackTeamPrefab;  // Prefab cho ??i t?n công
    public GameObject defenseTeamPrefab; // Prefab cho ??i phòng th?

    [Space]
    public Transform[] attackSpawnPoints; // ?i?m spawn cho ??i t?n công
    public Transform[] defenseSpawnPoints; // ?i?m spawn cho ??i phòng th?

    private GameObject currentPlayer;    // L?u tr? tham chi?u ??n nhân v?t hi?n t?i

    private void Awake()
    {
        instance = this;
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
        else // N?u ??i phòng th? ???c ch?n
        {
            Debug.Log("??i phòng th? ???c ch?n.");
            spawnPoint = defenseSpawnPoints[Random.Range(0, defenseSpawnPoints.Length)];
            teamPrefab = defenseTeamPrefab; // Nhân v?t cho ??i phòng th?
        }

        // T?o nhân v?t t?i ?i?m spawn t??ng ?ng và ??ng b? hóa gi?a t?t c? ng??i ch?i
        GameObject _player = PhotonNetwork.Instantiate(teamPrefab.name, spawnPoint.position, spawnPoint.rotation);

        // Gán team cho ng??i ch?i trong Custom Properties
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["team"] = selectedTeam; // Gán team vào Custom Properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // ??t tên ng??i ch?i
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
}
