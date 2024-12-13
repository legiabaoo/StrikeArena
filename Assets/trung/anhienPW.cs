using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class anhienPW : MonoBehaviour
{
    public InputField passwordInputField; // � nh?p m?t kh?u
    public Button toggleButton;          // N�t chuy?n ??i ?n/hi?n m?t kh?u
    public Sprite showIcon;              // Icon hi?n th? m?t kh?u
    public Sprite hideIcon;              // Icon ?n m?t kh?u

    private bool isPasswordVisible = false;

    void Start()
    {
        // ??m b?o n�t c� s? ki?n l?ng nghe
        toggleButton.onClick.AddListener(TogglePasswordVisibility);
        UpdateToggleIcon();
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        // C?p nh?t ch? ?? nh?p c?a InputField
        passwordInputField.contentType = isPasswordVisible ? InputField.ContentType.Standard : InputField.ContentType.Password;

        // Y�u c?u InputField c?p nh?t l?i gi� tr? hi?n th?
        passwordInputField.ForceLabelUpdate();

        // C?p nh?t icon c?a n�t
        UpdateToggleIcon();
    }

    void UpdateToggleIcon()
    {
        // ??i icon c?a n�t d?a v�o tr?ng th�i hi?n t?i
        Image buttonImage = toggleButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isPasswordVisible ? hideIcon : showIcon;
        }
    }
}
