using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public TextMeshProUGUI diemTeamXanh; // Text ?? hi?n th? ?i?m c?a ??i xanh
    private int scoreXanh = 0; // ?i?m c?a ??i xanh
    public TextMeshProUGUI timeText;
    private int minutes;
    private int seconds;
    private float currentTime = 3f; // Th?i gian ban ??u (300 giây = 5 phút)
    private bool isGameOver = false;
    public GameObject over;
    public Spawn spawnScript;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Ch? Master Client m?i b?t ??u ??ng h?
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            // Gi?m th?i gian còn l?i
            currentTime -= Time.deltaTime;

            // G?i th?i gian ??n t?t c? ng??i ch?i
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);

            // Ki?m tra khi th?i gian còn l?i b?ng 0
            if (currentTime <= 0)
            {
                EndGame();
            }
        }

        // C?p nh?t th?i gian hi?n th?
        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTime()
    {
        currentTime = 30f; // ??t l?i th?i gian v? giá tr? ban ??u (5 phút)
        isGameOver = false;
        over.SetActive(false);
    }

    public void EndGame()
    {
        spawnScript.TeleportToSpawnPoint();
        isGameOver = true;
        scoreXanh++; // C?ng 1 ?i?m cho ??i xanh
        photonView.RPC("UpdateScoreXanh", RpcTarget.All, scoreXanh);
        over.SetActive(true);
      
        Invoke("ResetTime", 3f); // ??t l?i th?i gian sau 3 giây
    }

    [PunRPC]
    private void SyncTime(float time)
    {
        // Nh?n giá tr? th?i gian t? Master Client và c?p nh?t
        currentTime = time;
    }
    [PunRPC]
    public void UpdateScoreXanh(int score)
    {
        scoreXanh = score;
        diemTeamXanh.text = scoreXanh.ToString(); // C?p nh?t ?i?m lên Text
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // G?i th?i gian t? Master Client ??n t?t c? khách
            stream.SendNext(currentTime);
        }
        else
        {
            // Nh?n th?i gian t? Master Client
            currentTime = (float)stream.ReceiveNext();
        }
    }
}
