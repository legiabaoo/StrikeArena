﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using Unity.VisualScripting;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Intansce; // Singleton để truy cập dễ dàng đến Leaderboard
    public Transform leaderboardParent; // Parent chứa các mục xếp hạng trong giao diện
    public GameObject leaderboardItemPrefab; // Prefab để hiển thị thông tin từng người chơi
    public GameObject loading; // GameObject để hiển thị trạng thái loading
    public TMP_Text ranking;
    public TMP_Text score;
    public TMP_Text name;

    private string apiUrl = "https://api-strikearena.onrender.com/ranking"; // URL API để lấy dữ liệu xếp hạng
    //private string apiUrl = "http://localhost:3000/ranking"; // URL API bảng xếp hạng
    private void Awake()
    {
        Intansce = this;// Gán giá trị singleton khi đối tượng khởi tạo
    }
    private void Start()
    {
        FetchLeaderboard();// Gọi hàm để lấy dữ liệu bảng xếp hạng khi khởi động
        UserScore();// Gọi hàm để hiển thị điểm cá nhân của người dùng
    }
    public void FetchLeaderboard()
    {
        StartCoroutine(GetLeaderboardData()); // Kích hoạt Coroutine để lấy dữ liệu bảng xếp hạng
    }
    public void UserScore()
    {
        string id = PlayerPrefs.GetString("Id");// Lấy ID của người dùng từ PlayerPrefs
        StartCoroutine(SetScore(id));// Kích hoạt Coroutine để lấy điểm cá nhân từ API
    }
    IEnumerator SetScore(string id)
    {

        //string jsonStringRequest = JsonConvert.SerializeObject();

        //var request = new UnityWebRequest($"http://localhost:3000/user/ranking/{id}", "GET");
        var request = new UnityWebRequest($"https://api-strikearena.onrender.com/user/ranking/{id}", "GET");
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringRequest);
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");// Thiết lập header cho yêu cầu
        yield return request.SendWebRequest(); // Gửi yêu cầu và chờ phản hồi

        if (request.result != UnityWebRequest.Result.Success) // Kiểm tra nếu có lỗi
        {
            Debug.Log(request.error);// Log lỗi nếu yêu cầu thất bại
        }
        else
        {
            var jsonString = request.downloadHandler.text.ToString();// Lấy dữ liệu JSON từ phản hồi
            LoginCallBackModel message = JsonConvert.DeserializeObject<LoginCallBackModel>(jsonString); 
            ranking.text = "#" + message.rank.ToString();
            score.text = message.score.ToString();
            name.text = message.username;
        }
        request.Dispose();
    }
    private IEnumerator GetLeaderboardData()
    {
        loading.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonString = request.downloadHandler.text;

            // Parse JSON thành danh sách người chơi
            List<PlayerData> players = JsonUtility.FromJson<PlayerList>(jsonString).players;
            Debug.Log(players);
            // Xóa các mục cũ
            foreach (Transform child in leaderboardParent)
            {
                Destroy(child.gameObject);
            }

            // Tạo mục xếp hạng mới
            for (int i = 0; i < players.Count; i++)
            {
                GameObject item = Instantiate(leaderboardItemPrefab, leaderboardParent);
                if (i == 0)
                {
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (i + 1); 
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].username; 
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].score.ToString();
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                else if (i == 1)
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#FFA400", out Color newColor);
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (i + 1); 
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = newColor;
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].username; 
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = newColor;
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].score.ToString(); 
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = newColor;

                }
                else if (i == 2)
                {
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (i + 1); 
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].username; 
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].score.ToString(); 
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                }
                else
                {
                    // Gán dữ liệu vào TextMeshPro
                    item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (i + 1); 
                    item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].username; 
                    item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].score.ToString(); 
                }

            }

        }
        else
        {
            Debug.LogError("Lỗi khi lấy dữ liệu bảng xếp hạng: " + request.error);
        }
        loading.SetActive(false);
    }
}

// Lớp cho dữ liệu người chơi
[System.Serializable]
public class PlayerData
{
    public string username;
    public int score;
}

// Lớp để parse danh sách người chơi
[System.Serializable]
public class PlayerList
{
    public List<PlayerData> players;
}
