using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterModel  
{
    public RegisterModel(string email)
    {
        this.email = email;
    }

    public RegisterModel(string username, string email, string password)
    {
        this.username = username;
        this.email = email;
        this.password = password;
    }

    public string username { get; set; }
    public string email {  get; set; }
    public string password { get; set; }
}
