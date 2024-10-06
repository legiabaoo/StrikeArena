using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class LoginAPI : MonoBehaviour
{
    public TMP_InputField edtEmail;
    public TMP_InputField edtPassword;
    public TMP_Text txtMessage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Login()
    {
        string email = edtEmail.text;
        string pass = edtPassword.text;
        LoginModel loginModel = new LoginModel(email, pass);
        StartCoroutine(CheckLogin(loginModel));
    }
    IEnumerator CheckLogin(LoginModel loginModel)
    {
        
        string jsonStringRequest = JsonConvert.SerializeObject(loginModel);

        var request = new UnityWebRequest("https://api-strikearena.onrender.com/login", "POST");
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

}
