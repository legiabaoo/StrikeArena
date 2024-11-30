using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using System.Text;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Intansce;
    public Transform leaderboardParent; // Parent chứa các mục xếp hạng
    public GameObject leaderboardItemPrefab; // Prefab cho mỗi người chơi
    public GameObject loading;
    public TMP_Text ranking;
    public TMP_Text score;
    public TMP_Text name;

    private string apiUrl = "https://api-strikearena.onrender.com/ranking";
    //private string apiUrl = "http://localhost:3000/ranking"; // URL API bảng xếp hạng
    private void Awake()
    {
        Intansce = this;
    }
    private void Start()
    {
        FetchLeaderboard();
        UserScore();
    }
    public void FetchLeaderboard()
    {
        StartCoroutine(GetLeaderboardData());
    }
    public void UserScore()
    {
        string id = PlayerPrefs.GetString("Id");
        StartCoroutine(SetScore(id));
    }
    IEnumerator SetScore(string id)
    {

        //string jsonStringRequest = JsonConvert.SerializeObject();

        //var request = new UnityWebRequest($"http://localhost:3000/user/ranking/{id}", "GET");
        var request = new UnityWebRequest($"https://api-strikearena.onrender.com/user/ranking/{id}", "GET");
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringRequest);
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            var jsonString = request.downloadHandler.text.ToString();
            LoginCallBackModel message = JsonConvert.DeserializeObject<LoginCallBackModel>(jsonString);
            ranking.text = "#"+message.rank.ToString();
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

                // Gán dữ liệu vào TextMeshPro
                item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#"+(i + 1); // Thứ hạng
                item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i].username; // Tên
                item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = players[i].score.ToString(); // Điểm
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
