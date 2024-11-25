using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TimeManager instance;
    public TextMeshProUGUI diemTeamXanh;
    public TextMeshProUGUI diemTeamDo;// Text hiển thị điểm của đội xanh
    private int scoreXanh = 0;// Điểm của đội xanh
    private int scoreDo = 0;//Diem cua doi do
    public TextMeshProUGUI timeText;
    private int minutes;
    private int seconds;

    // Thời gian cho hai giai đoạn
    private float buyPhaseTime = 5f; // Thời gian 30 giây cho mua vũ khí
    private float battlePhaseTime = 100f; // Thời gian 1 phút 40 giây cho chiến đấu
    private float plantPhaseTime = 20f;
    private float currentTime;

    public bool startGame = false;
    public bool isGameOver = false;
    public bool isPlantSpike = false;
    public bool isAllDeathRed = false;
    public bool isAllDeathBlue = false;
    private bool isSpikeTime = false;
    private bool isTextBuy = false;
    public GameObject over;
    public GameObject win;
    public GameObject lose;
    public Spawn spawnScript;
    public GameObject[] listshield;

    private enum GamePhase { Buy, Battle, Plant }
    private enum Team { red, blue };
    private Team winner;
    private GamePhase currentPhase;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC("SyncTime", RpcTarget.All, currentTime);
        //}
        StartBuyPhase();
        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Update()
    {
        if (!isGameOver && startGame)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (currentPhase == GamePhase.Buy)
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SpikeExists", false } });
                    if (!isTextBuy)
                    {
                        isTextBuy = true;
                        //over.GetComponentInChildren<Text>().color = Color.white;
                        //over.GetComponentInChildren<Text>().text = "BẮT ĐẦU MUA TRANG BỊ";
                        Color color = Color.white;
                        string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                        photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "BẮT ĐẦU MUA TRANG BỊ");
                        PhotonView.Get(this).RPC("onText", RpcTarget.AllBuffered);
                        photonView.RPC("SetTagForPlayers", RpcTarget.All); // Gọi RPC đặt tag cho người chơi
                        Invoke("InvokeOffText", 1f);
                    }

                }
                // Giảm thời gian còn lại
                currentTime -= Time.deltaTime;

                // Gửi thời gian đến tất cả người chơi
                photonView.RPC("SyncTime", RpcTarget.All, currentTime);

                // Kiểm tra khi thời gian còn lại bằng 0
                if (currentTime <= 0)
                {
                    if (currentPhase == GamePhase.Buy)
                    {
                        Color color = Color.green;
                        string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                        photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "BẮT ĐẦU CHIẾN ĐẤU");
                        photonView.RPC("onText", RpcTarget.AllBuffered);
                        Invoke("InvokeOffText", 1f);
                        StartBattlePhase(); // Bắt đầu giai đoạn chiến đấu
                    }
                    else if (currentPhase == GamePhase.Battle)
                    {
                        Color color = Color.blue;
                        string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                        photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "ĐỘI PHÒNG THỦ \n CHIÊN THẮNG");
                        winner = Team.blue;
                        Debug.LogError("B1");
                        EndGame(); // Kết thúc vòng đấu khi giai đoạn chiến đấu kết thúc
                    }
                    else if (currentPhase == GamePhase.Plant)
                    {
                        Color color = Color.red;
                        string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                        photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "ĐỘI TẤN CÔNG \n CHIÊN THẮNG");
                        winner = Team.red;
                        Debug.LogError("R1");
                        EndGame();
                    }
                }
                if (isPlantSpike && !isSpikeTime)
                {
                    PlantSpikePhase();
                }
                if (!isPlantSpike && isSpikeTime)
                {
                    Color color = Color.blue;
                    string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                    photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "ĐỘI PHÒNG THỦ \n CHIÊN THẮNG");
                    isSpikeTime = false;
                    winner = Team.blue;
                    Debug.LogError("B2");
                    EndGame();
                }
                if (isAllDeathRed)
                {
                    winner = Team.blue;
                    Color color = Color.blue;
                    string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                    photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "ĐỘI PHÒNG THỦ \n CHIÊN THẮNG");
                    Debug.LogError("B3");
                    EndGame();
                    //if (scoreXanh == 4)
                    //{
                    //    Debug.Log("Xanh thắng");
                    //    photonView.RPC("DisplayEndGameResult", RpcTarget.All, 1); // Truyền 1 cho team Xanh là đội chiến thắng
                    //}
                }
                if (isAllDeathBlue)
                {
                    winner = Team.red;
                    Color color = Color.red;
                    string colorString = $"{color.r},{color.g},{color.b},{color.a}";
                    photonView.RPC("SetNotify", RpcTarget.AllBuffered, colorString, "ĐỘI TẤN CÔNG \n CHIÊN THẮNG");
                    Debug.LogError("R2");
                    EndGame();
                    //if (scoreDo == 4)
                    //{
                    //    Debug.Log("Xanh thắng");
                    //    photonView.RPC("DisplayEndGameResult", RpcTarget.All, 0); // Truyền 1 cho team Xanh là đội chiến thắng
                    //}
                }
            }

            // Cập nhật thời gian hiển thị
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    [PunRPC]
    public void DisplayEndGameResult(int winningTeam)
    {
        // Kiểm tra xem người chơi hiện tại thuộc team nào và hiển thị kết quả phù hợp
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("team", out var teamValue) && (int)teamValue == winningTeam)
        {
            win.SetActive(true); // Hiển thị chiến thắng nếu là team thắng
            PlayerPrefs.SetInt("Result", 1);
        }
        else
        {
            lose.SetActive(true); // Hiển thị thua nếu không phải là team thắng
            PlayerPrefs.SetInt("Result", -1);
        }
    }
    private void StartBuyPhase()
    {
        currentPhase = GamePhase.Buy;
        currentTime = buyPhaseTime;
    }

    private void StartBattlePhase()
    {
        RoomManager.instance.hasCalledEndGame = false;
        currentPhase = GamePhase.Battle;
        currentTime = battlePhaseTime;
        photonView.RPC("ShieldDown", RpcTarget.AllBuffered, false);
    }

    private void PlantSpikePhase()
    {
        isSpikeTime = true;
        currentPhase = GamePhase.Plant;
        currentTime = plantPhaseTime;
    }

    [PunRPC]
    private void SetTagForPlayers()
    {
        GameObject[] newPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject newPlayer in newPlayers)
        {
            newPlayer.tag = "OldPlayer";
        }
    }

    [PunRPC]
    public void ResetTime()
    {
        RoomManager.instance.HandleTeamSelection();
        RoomManager.instance.RemovePlayerInstances();
        GunShop.instance.playerMoney = 800;
        //if (isGameOver)
        //{
        photonView.RPC("ShieldDown", RpcTarget.AllBuffered, true);
        isGameOver = false;
        //startGame = true;
        isSpikeTime = false;
        isPlantSpike = false;
        //set lai SpikeExists la false


        //}
        RemoveSpikeFromScene();
        StartBuyPhase(); // Bắt đầu lại giai đoạn mua vũ khí
        Invoke("InvokeOffText", 1f);
    }
    [PunRPC]
    public void ShieldDown(bool enabledShield)
    {
        foreach (GameObject shield in listshield)
        {
            MeshRenderer meshRenderer = shield.GetComponent<MeshRenderer>();
            MeshCollider collider = shield.GetComponent<MeshCollider>();
            meshRenderer.enabled = enabledShield;
            collider.enabled = enabledShield;

        }
    }
    private void RemoveSpikeFromScene()
    {
        GameObject spike = GameObject.FindWithTag("Spike");
        if (spike != null) Destroy(spike);
    }

    [PunRPC]
    public void onText()
    {
        over.SetActive(true);
    }
    [PunRPC]
    public void offText()
    {
        over.SetActive(false);
    }
    public void InvokeOffText()
    {
        photonView.RPC("offText", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void SetNotify(string colorString, string text)
    {
        string[] rgba = colorString.Split(',');
        Color color = new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
        over.GetComponentInChildren<Text>().color = color;
        over.GetComponentInChildren<Text>().text = text;
    }
    [PunRPC]
    public void EndGame()
    {
        isTextBuy = false;
        isGameOver = true;
        startGame = false;
        isAllDeathRed = false;
        isAllDeathBlue = false;
        if (winner == Team.red)
        {
            scoreDo++;
            photonView.RPC("UpdateScoreDo", RpcTarget.All, scoreDo);
        }
        else if (winner == Team.blue)
        {
            scoreXanh++;// Cộng 1 điểm cho đội xanh
            photonView.RPC("UpdateScoreXanh", RpcTarget.All, scoreXanh);
        }
        if (scoreXanh == 4)
        {
            Debug.Log("Xanh thắng");
            photonView.RPC("DisplayEndGameResult", RpcTarget.All, 1); // Truyền 1 cho team Xanh là đội chiến thắng
            Invoke("BackHomeDelay", 1f);
        }
        else if (scoreDo == 4)
        {
            Debug.Log("Xanh thắng");
            photonView.RPC("DisplayEndGameResult", RpcTarget.All, 0); // Truyền 1 cho team Xanh là đội chiến thắng
            Invoke("BackHomeDelay", 1f);
        }
        else
        {
            PhotonView.Get(this).RPC("onText", RpcTarget.AllBuffered);
            // Đặt lại thời gian sau 3 giây
            Invoke("ResetTimeDelay", 3f);
        }

    }
    public void BackHomeDelay()
    {
        photonView.RPC("BackHome", RpcTarget.All);
    }
    [PunRPC]
    private void BackHome()
    {
        SceneManager.LoadScene("LoginScene");
    }
    private void ResetTimeDelay()
    {
        photonView.RPC("ResetTime", RpcTarget.All);
    }

    [PunRPC]
    private void SyncTime(float time)
    {
        currentTime = time;
    }

    [PunRPC]
    public void UpdateScoreXanh(int score)
    {
        scoreXanh = score;
        diemTeamXanh.text = scoreXanh.ToString(); // Cập nhật điểm lên Text
    }
    [PunRPC]
    public void UpdateScoreDo(int score)
    {
        scoreDo = score;
        diemTeamDo.text = scoreDo.ToString(); // Cập nhật điểm lên Text
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentTime);
        }
        else
        {
            currentTime = (float)stream.ReceiveNext();
        }
    }
}
