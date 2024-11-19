using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCallBackModel 
{
    public LoginCallBackModel()
    {
        
    }
    public LoginCallBackModel(int status, string message)
    {
        this.status = status;
        this.message = message;
    }

    public LoginCallBackModel(int status, string message, int otp)
    {
        this.status = status;
        this.message = message;
        this.otp = otp;
    }

    public LoginCallBackModel(int status, string message, string username, int score, int rank, string id)
    {
        this.status = status;
        this.message = message;
        this.username = username;
        this.score = score;
        this.rank = rank;
        this.id = id;
    }

    public int status { get; set; }
    public string message { get; set; }
    public int otp {  get; set; }
    public string username {  get; set; }
    public int score { get; set; }
    public int rank { get; set; }
    public string id { get; set; }
}
