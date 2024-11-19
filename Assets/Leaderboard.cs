using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public Transform leaderboardParent; // Parent chứa các mục xếp hạng
    public GameObject leaderboardItemPrefab; // Prefab cho mỗi người chơi

    private string apiUrl = "http://localhost:3000/players/ranking"; // URL API bảng xếp hạng
    private void Start()
    {
        FetchLeaderboard();
    }
    public void FetchLeaderboard()
    {
        StartCoroutine(GetLeaderboardData());
    }

    private IEnumerator GetLeaderboardData()
    {
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
