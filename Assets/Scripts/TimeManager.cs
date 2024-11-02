using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon.StructWrapping;

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
    private float buyPhaseTime = 2f; // Thời gian 30 giây cho mua vũ khí
    private float battlePhaseTime = 15f; // Thời gian 1 phút 40 giây cho chiến đấu
    private float plantPhaseTime = 10f;
    private float currentTime;

    public bool isGameOver = false;
    public bool isPlantSpike = false;
    private bool isSpikeTime = false;
    private bool isTextBuy = false;
    public GameObject over;
    public Spawn spawnScript;

    private enum GamePhase { Buy, Battle, Plant }
    private enum Team { red, blue };
    private Team winner;
    private GamePhase currentPhase;
    public bool startGame = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);
        }
        else
        {
            currentTime = buyPhaseTime;
            
        }
    }

    private void Update()
    {
        if (!isGameOver && startGame)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (currentPhase == GamePhase.Buy && !isTextBuy)
                {
                    isTextBuy = true;
                    over.GetComponentInChildren<Text>().color = Color.white;
                    over.GetComponentInChildren<Text>().text = "BẮT ĐẦU MUA TRANG BỊ";
                    PhotonView.Get(this).RPC("onText", RpcTarget.AllBuffered);
                    Invoke("InvokeOffText", 1f);
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
                        over.GetComponentInChildren<Text>().text = "BẮT ĐẦU CHIẾN ĐẤU";
                        over.GetComponentInChildren<Text>().color = Color.green;
                        PhotonView.Get(this).RPC("onText", RpcTarget.AllBuffered);
                        Invoke("InvokeOffText", 1f);
                        StartBattlePhase(); // Bắt đầu giai đoạn chiến đấu
                    }
                    else if (currentPhase == GamePhase.Battle)
                    {
                        over.GetComponentInChildren<Text>().text = "ĐỘI PHÒNG THỦ \n" +
                            "CHIÊN THẮNG";
                        over.GetComponentInChildren<Text>().color = Color.blue;
                        winner = Team.blue;
                        EndGame(); // Kết thúc vòng đấu khi giai đoạn chiến đấu kết thúc
                    }
                    else if (currentPhase == GamePhase.Plant)
                    {
                        over.GetComponentInChildren<Text>().text = "ĐỘI TẤN CÔNG \n" +
                            "CHIÊN THẮNG";
                        over.GetComponentInChildren<Text>().color = Color.red;
                        winner = Team.red;
                        EndGame();
                    }
                }
                if (isPlantSpike && !isSpikeTime)
                {
                    PlantSpikePhase();
                }
                if (!isPlantSpike && isSpikeTime)
                {
                    over.GetComponentInChildren<Text>().text = "ĐỘI PHÒNG THỦ \n" +
                            "CHIÊN THẮNG";
                    over.GetComponentInChildren<Text>().color = Color.blue;
                    isSpikeTime = false;
                    winner = Team.blue;
                    EndGame();
                }
            }

            // Cập nhật thời gian hiển thị
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    private void StartBuyPhase()
    {
        currentPhase = GamePhase.Buy;
        currentTime = buyPhaseTime;
    }

    private void StartBattlePhase()
    {
        currentPhase = GamePhase.Battle;
        currentTime = battlePhaseTime;
        photonView.RPC("SetTagForPlayers", RpcTarget.All); // Gọi RPC đặt tag cho người chơi
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
        //if (isGameOver)
        //{

        isGameOver = false;
        RoomManager.instance.hasCalledEndGame = false;
        //}
        SpikeManager.instance.photonView.RPC("SetIsSpikeExists", RpcTarget.AllBuffered, false);
        StartBuyPhase(); // Bắt đầu lại giai đoạn mua vũ khí
        over.SetActive(false);
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
        offText();
    }
    public void EndGame()
    {
        isTextBuy = false;
        isGameOver = true;
        startGame = false;
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


        PhotonView.Get(this).RPC("onText", RpcTarget.AllBuffered);

        // Đặt lại thời gian sau 3 giây
        Invoke("ResetTimeDelay", 3f);
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
