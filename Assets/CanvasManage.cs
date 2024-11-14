using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManage : MonoBehaviour
{
    public GameObject login;
    public GameObject register;
    public GameObject mainHall;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("login") == 1)
        {
            login.SetActive(false);
            mainHall.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("login") == 0)
        {
            login.SetActive(true);
            mainHall.SetActive(false);
        }
    }
    public void logout()
    {
        PlayerPrefs.SetInt("login", 0);
    }
}
