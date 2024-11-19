using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.UI;
using UnityEngine.UI;


public class RegisterAPI : MonoBehaviour
{
    public TMP_InputField edtUsername;
    public TMP_InputField edtEmail;
    public TMP_InputField edtPassword;
    public TMP_InputField edtCPassword;
    public TMP_InputField edtOTP;
    public int otp;
    public TMP_Text txtMessage;
    public GameObject Loading;
    public TMP_Text send;
    public Button btnSend;
    public bool isSend = false;
    public float time ;
    // Start is called before the first frame update
    void Start()
    {
        //btnSend = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSend)
        {
            btnSend.interactable = false;
            time -= Time.deltaTime;
            send.text = Mathf.Floor(time).ToString();
            if (time < 0)
            {
                send.text = "Send";
                isSend = false;
            }
        }
        else if (!isSend)
        {
            btnSend.interactable = true;
            time = 60f;
            resetOTP();
        }
    }
    public void resetOTP()
    {
        otp = 01011;
    }
    public void Register()
    {
        string username = edtUsername.text;
        string email = edtEmail.text;
        string pass = edtPassword.text;
        string cpass = edtCPassword.text;
        string edtotp = edtOTP.text;
        if (!pass.Equals(cpass))
        {
            txtMessage.text = "Nhập lại mật khẩu chưa khớp";
        }
        else if (int.Parse(edtotp) != (otp))
        {
            txtMessage.text = "Nhập otp sai";
        }
        else
        {
            RegisterModel registerModel = new RegisterModel(username, email, pass);
            StartCoroutine(CheckLogin(registerModel));
        }

    }
    public void SendMail()
    {
        if (!isSend)
        {
            string email = edtEmail.text;
            RegisterModel registerModel = new RegisterModel(email);
            StartCoroutine(GetOTP(registerModel));
        }

    }
    IEnumerator CheckLogin(RegisterModel registerModel)
    {
        Loading.SetActive(true);
        string jsonStringRequest = JsonConvert.SerializeObject(registerModel);

        var request = new UnityWebRequest("https://api-strikearena.onrender.com/register", "POST");
        //var request = new UnityWebRequest("http://localhost:3000/register", "POST");
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

        }
        Loading.SetActive(false);
        request.Dispose();
    }
    IEnumerator GetOTP(RegisterModel registerModel)
    {
        Loading.SetActive(true);
        string jsonStringRequest = JsonConvert.SerializeObject(registerModel);

        //var request = new UnityWebRequest("https://api-strikearena.onrender.com/confirm-mail", "POST");
        var request = new UnityWebRequest("http://localhost:3000/confirm-mail", "POST");
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
            otp = message.otp;
            Debug.Log(otp);
        }
        Loading.SetActive(false);
        isSend = true;
        request.Dispose();
    }

}
