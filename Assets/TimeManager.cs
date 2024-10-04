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
    private float currentTime = 3f; // Th?i gian ban ??u (300 gi�y = 5 ph�t)
    private bool isGameOver = false;
    public Loaderboard loaderboard;
    public GameObject over;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // ??ng b? h�a th?i gian v?i m�y ch?
            photonView.RPC("SyncTime", RpcTarget.MasterClient, currentTime);
            loaderboard = FindObjectOfType<Loaderboard>();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            // Gi?m th?i gian c�n l?i
            currentTime -= Time.deltaTime;

            // G?i th?i gian ??n t?t c? c�c m�y kh�ch th�ng qua Photon
            photonView.RPC("SyncTime", RpcTarget.All, currentTime);

            // Ki?m tra khi th?i gian c�n l?i b?ng 0
            if (currentTime <= 0)
            {
                EndGame();
            }
        }

        // C?p nh?t th?i gian hi?n th? tr�n giao di?n ng??i ch?i
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
      
        // Th?c hi?n c�c h�nh ??ng khi k?t th�c m�n ch?i, nh? hi?n th? ?i?m s?, v.v.
    }

    [PunRPC]
    private void SyncTime(float time)
    {
        // Nh?n gi� tr? th?i gian t? m�y ch? v� c?p nh?t n�
        currentTime = time;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // G?i th?i gian t? m�y ch? ??n t?t c? c�c m�y kh�ch
            stream.SendNext(currentTime);
        }
        else
        {
            // Nh?n th?i gian t? m�y ch?
            currentTime = (float)stream.ReceiveNext();
        }
    }
  
}
