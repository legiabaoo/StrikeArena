using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Dropdown : MonoBehaviour
{
    public TMP_Dropdown teamDropdown; // Tham chi?u ??n TMP_Dropdown

    void Start()
    {
        // L?ng nghe s? thay ??i giá tr? c?a dropdown
        teamDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Hàm này s? ch?y m?i khi giá tr? c?a dropdown thay ??i
    void OnDropdownValueChanged(int index)
    {
        // L?y tên c?a option hi?n t?i
        string selectedOption = teamDropdown.options[index].text;

        // In ra giá tr? hi?n t?i (ch? s? và tên)
        Debug.Log("Ch? s?: " + index + ", Giá tr?: " + selectedOption);

        // Ki?m tra giá tr? và th?c hi?n hành ??ng
        if (selectedOption == "T?n Công")
        {
            // Th?c hi?n hành ??ng cho ??i t?n công
            Debug.Log("Ng??i ch?i ch?n ??i t?n công");
        }
        else if (selectedOption == "Phòng Th?")
        {
            // Th?c hi?n hành ??ng cho ??i phòng th?
            Debug.Log("Ng??i ch?i ch?n ??i phòng th?");
        }
    }
}
