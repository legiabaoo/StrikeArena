using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class anhienPW : MonoBehaviour
{
    public InputField passwordInputField; // Ô nh?p m?t kh?u
    public Button toggleButton;          // Nút chuy?n ??i ?n/hi?n m?t kh?u
    public Sprite showIcon;              // Icon hi?n th? m?t kh?u
    public Sprite hideIcon;              // Icon ?n m?t kh?u

    private bool isPasswordVisible = false;

    void Start()
    {
        // ??m b?o nút có s? ki?n l?ng nghe
        toggleButton.onClick.AddListener(TogglePasswordVisibility);
        UpdateToggleIcon();
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        // C?p nh?t ch? ?? nh?p c?a InputField
        passwordInputField.contentType = isPasswordVisible ? InputField.ContentType.Standard : InputField.ContentType.Password;

        // Yêu c?u InputField c?p nh?t l?i giá tr? hi?n th?
        passwordInputField.ForceLabelUpdate();

        // C?p nh?t icon c?a nút
        UpdateToggleIcon();
    }

    void UpdateToggleIcon()
    {
        // ??i icon c?a nút d?a vào tr?ng thái hi?n t?i
        Image buttonImage = toggleButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isPasswordVisible ? hideIcon : showIcon;
        }
    }
}
