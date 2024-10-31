using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TimeManager instance;
    public TextMeshProUGUI diemTeamXanh; // Text hiển thị điểm của đội xanh
    private int scoreXanh = 0; // Điểm của đội xanh
    public TextMeshProUGUI timeText;
    private int minutes;
    private int seconds;

    // Thời gian cho hai giai đoạn
    private float buyPhaseTime = 2f; // Thời gian 30 giây cho mua vũ khí
    private float battlePhaseTime = 100f; // Thời gian 1 phút 40 giây cho chiến đấu
    private float currentTime;

    private bool isGameOver = false;
    public GameObject over;
    public Spawn spawnScript;

    private enum GamePhase { Buy, Battle }
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
        over.GetComponentInChildren<Text>().color = Color.green;

    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !isGameOver && startGame)
        {
         
            // Giảm thời gian còn lại
            currentTime -= Time.deltaTime;

            // Gửi thời gian đến tất cả người chơi
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);

            // Kiểm tra khi thời gian còn lại bằng 0
            if (currentTime <= 0)
            {
                if (currentPhase == GamePhase.Buy)
                {
                    StartBattlePhase(); // Bắt đầu giai đoạn chiến đấu
                }
                else if (currentPhase == GamePhase.Battle)
                {
                    EndGame(); // Kết thúc vòng đấu khi giai đoạn chiến đấu kết thúc
                }
            }
        }

        // Cập nhật thời gian hiển thị
        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
        RoomManager.instance.RemovePlayerInstances();

        // Spawn lại nhân vật mới và reset tag
        RoomManager.instance.HandleTeamSelection();

        

        StartBuyPhase(); // Bắt đầu lại giai đoạn mua vũ khí
        isGameOver = false;
        over.SetActive(false);
    }

    [PunRPC]
    public void UI()
    {
        over.SetActive(true);
    }
    public void EndGame()
    {
        isGameOver = true;
        scoreXanh++; // Cộng 1 điểm cho đội xanh
        photonView.RPC("UpdateScoreXanh", RpcTarget.All, scoreXanh);
        PhotonView.Get(this).RPC("UI", RpcTarget.AllBuffered);
        
        // Đặt lại thời gian sau 3 giây
        Invoke("ResetTimeDelay", 3f);
    }
    private void ResetTimeDelay()
    {
        PhotonView.Get(this).RPC("ResetTime", RpcTarget.All);
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
