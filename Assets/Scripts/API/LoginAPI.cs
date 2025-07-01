using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class LoginAPI : MonoBehaviour
{
    public static LoginAPI instance;
    public TMP_InputField edtEmail;
    public TMP_InputField edtPassword;
    public TMP_Text txtMessage;
    public TMP_Text txtUsername;
    public GameObject Loading;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("Result") == 1)
        {
            Win();
            PlayerPrefs.SetInt("Result", 0);
        }else if (PlayerPrefs.GetInt("Result") == -1)
        {
            Lose();
            PlayerPrefs.SetInt("Result", 0);
        }else if(PlayerPrefs.GetInt("Result") == -2)
        {
            OutGame();
            PlayerPrefs.SetInt("Result", 0);
        }
    }
    
    public void Login()
    {
        string email = edtEmail.text;
        string pass = edtPassword.text;
        LoginModel loginModel = new LoginModel(email, pass);
        StartCoroutine(CheckLogin(loginModel));
    }
    public void Logout()
    {
        StartCoroutine(SetStatus(0, PlayerPrefs.GetString("Id")));
    }
    public void Win()
    {
        int score = PlayerPrefs.GetInt("Rank")+30;
        StartCoroutine(SetScore(score, PlayerPrefs.GetString("Id")));
    }
    public void Lose()
    {
        int score = PlayerPrefs.GetInt("Rank")-25;
        StartCoroutine(SetScore(score, PlayerPrefs.GetString("Id")));
    }
    public void OutGame()
    {
        int score = PlayerPrefs.GetInt("Rank") - 45;
        StartCoroutine(SetScore(score, PlayerPrefs.GetString("Id")));
    }
    IEnumerator CheckLogin(LoginModel loginModel)
    {
        Loading.SetActive(true);
        string jsonStringRequest = JsonConvert.SerializeObject(loginModel);

        var request = new UnityWebRequest("https://api-strikearena.onrender.com/login", "POST");
        //var request = new UnityWebRequest("http://localhost:3000/login", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringRequest);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
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
            txtMessage.text = message.message;
            if (message.status == 0)
            {
                // Lưu username tạm thời
                string username = message.username;
                Debug.Log(message.id);
                txtUsername.text = username;
                PlayerPrefs.SetString("Username", username);
                PlayerPrefs.SetInt("login", 1);
                PlayerPrefs.SetInt("Rank", message.score);
                PlayerPrefs.SetString("Id", message.id);
                StartCoroutine(SetStatus(1, message.id));
                edtEmail.text = "";
                edtPassword.text = "";
                txtMessage.text = "";
                //Leaderboard.Intansce.UserScore();
            }
        }
        Loading.SetActive(false);
        request.Dispose();
    }
    IEnumerator SetStatus(int statusValue, string id)
    {

        string jsonStringRequest = JsonConvert.SerializeObject(new { status = statusValue });

        var request = new UnityWebRequest($"http://localhost:3000/players/{id}/status", "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringRequest);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
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
            //LoginCallBackModel message = JsonConvert.DeserializeObject<LoginCallBackModel>(jsonString);
            //txtMessage.text = message.message;
            //if (message.status == 0)
            //{
            //    // Lưu username tạm thời
            //    string username = message.username;
            //    Debug.Log(message.id);
            //    txtUsername.text = username;
            //    PlayerPrefs.SetString("Username", username);
            //    PlayerPrefs.SetInt("login", 1);
            //    PlayerPrefs.SetInt("Rank", message.score);
            //    edtEmail.text = "";
            //    edtPassword.text = "";
            //    txtMessage.text = "";
            //}
        }
        request.Dispose();
    }
    IEnumerator SetScore(int scoreValue, string id)
    {

        string jsonStringRequest = JsonConvert.SerializeObject(new { score = scoreValue });

        //var request = new UnityWebRequest($"http://localhost:3000/players/{id}/score", "PUT");
        var request = new UnityWebRequest($"https://api-strikearena.onrender.com/players/{id}/score", "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStringRequest);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
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
            PlayerPrefs.SetInt("Rank", scoreValue);
        }
        request.Dispose();
    }
}
