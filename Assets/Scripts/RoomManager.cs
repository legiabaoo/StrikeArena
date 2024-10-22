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
    public GameObject attackTeamPrefab;  // Prefab cho ??i t?n c�ng
    public GameObject defenseTeamPrefab; // Prefab cho ??i ph�ng th?

    [Space]
    public Transform[] attackSpawnPoints; // ?i?m spawn cho ??i t?n c�ng
    public Transform[] defenseSpawnPoints; // ?i?m spawn cho ??i ph�ng th?

    private GameObject currentPlayer;    // L?u tr? tham chi?u ??n nh�n v?t hi?n t?i

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
        hash["team"] = selectedTeam; // G�n team v�o Custom Properties
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
}
