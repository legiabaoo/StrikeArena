using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class RegisterAPI : MonoBehaviour
{
    public TMP_InputField edtUsername;
    public TMP_InputField edtEmail;
    public TMP_InputField edtPassword;
    public TMP_InputField edtCPassword;
    public TMP_InputField edtOTP;
    public int otp;
    public TMP_Text txtMessage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        else if (int.Parse(edtotp)!=(otp))
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
        string email = edtEmail.text;
        RegisterModel registerModel = new RegisterModel(email);
        StartCoroutine(GetOTP(registerModel));
    }
    IEnumerator CheckLogin(RegisterModel registerModel)
    {

        string jsonStringRequest = JsonConvert.SerializeObject(registerModel);

        var request = new UnityWebRequest("https://api-strikearena.onrender.com/register", "POST");
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
        request.Dispose();
    }
    IEnumerator GetOTP(RegisterModel registerModel)
    {
       
        string jsonStringRequest = JsonConvert.SerializeObject(registerModel);

        var request = new UnityWebRequest("https://api-strikearena.onrender.com/confirm-mail", "POST");
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
        request.Dispose();
    }

}
