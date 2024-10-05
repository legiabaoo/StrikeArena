using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public TextMeshProUGUI timeText;
    private int minutes;
    private int seconds;
    private float currentTime = 3f; // Th?i gian ban ??u (300 giây = 5 phút)
    private bool isGameOver = false;
    public Loaderboard loaderboard;
    public GameObject over;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // ??ng b? hóa th?i gian v?i máy ch?
            photonView.RPC("SyncTime", RpcTarget.MasterClient, currentTime);
            loaderboard = FindObjectOfType<Loaderboard>();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            // Gi?m th?i gian còn l?i
            currentTime -= Time.deltaTime;

            // G?i th?i gian ??n t?t c? các máy khách thông qua Photon
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);

            // Ki?m tra khi th?i gian còn l?i b?ng 0
            if (currentTime <= 0)
            {
                EndGame();
            }
        }

        // C?p nh?t th?i gian hi?n th? trên giao di?n ng??i ch?i
        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void EndGame()
    {
        over.SetActive(true);
        if (loaderboard != null)
        {
            loaderboard.SetPlayersHolderActive(true);
        }
        isGameOver = true;
      
        // Th?c hi?n các hành ??ng khi k?t thúc màn ch?i, nh? hi?n th? ?i?m s?, v.v.
    }

    [PunRPC]
    private void SyncTime(float time)
    {
        // Nh?n giá tr? th?i gian t? máy ch? và c?p nh?t nó
        currentTime = time;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // G?i th?i gian t? máy ch? ??n t?t c? các máy khách
            stream.SendNext(currentTime);
        }
        else
        {
            // Nh?n th?i gian t? máy ch?
            currentTime = (float)stream.ReceiveNext();
        }
    }
  
}
