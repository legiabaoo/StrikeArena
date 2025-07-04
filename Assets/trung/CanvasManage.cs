﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManage : MonoBehaviour
{
    public GameObject login;
    public GameObject register;
    public GameObject mainHall;
    public TMP_InputField[] inputField; // Tham chiếu đến Input Field

    private bool isPasswordMode = true; // Trạng thái hiện tại
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
            register.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("login") == 0)
        {
            login.SetActive(true);
            register.SetActive(false);
            mainHall.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("login") == 2)
        {
            register.SetActive(true);
            mainHall.SetActive(false);
            login.SetActive(false);
        }
    }
    public void logout()
    {
        PlayerPrefs.SetInt("login", 0);
    }
    public void btnRegister()
    {
        PlayerPrefs.SetInt("login", 2);
    }
    public void ToggleContentType(int i)
    {
        if (isPasswordMode)
        {
            inputField[i].contentType = TMP_InputField.ContentType.Standard; // Chuyển sang chế độ văn bản thường
        }
        else
        {
            inputField[i].contentType = TMP_InputField.ContentType.Password; // Chuyển sang chế độ mật khẩu
        }

        isPasswordMode = !isPasswordMode; // Đảo trạng thái
        inputField[i].ForceLabelUpdate(); // Cập nhật giao diện
    }
}
