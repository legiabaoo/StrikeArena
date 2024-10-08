using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Dropdown : MonoBehaviour
{
    public TMP_Dropdown teamDropdown; // Tham chi?u ??n TMP_Dropdown

    void Start()
    {
        // L?ng nghe s? thay ??i gi� tr? c?a dropdown
        teamDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // H�m n�y s? ch?y m?i khi gi� tr? c?a dropdown thay ??i
    void OnDropdownValueChanged(int index)
    {
        // L?y t�n c?a option hi?n t?i
        string selectedOption = teamDropdown.options[index].text;

        // In ra gi� tr? hi?n t?i (ch? s? v� t�n)
        Debug.Log("Ch? s?: " + index + ", Gi� tr?: " + selectedOption);

        // Ki?m tra gi� tr? v� th?c hi?n h�nh ??ng
        if (selectedOption == "T?n C�ng")
        {
            // Th?c hi?n h�nh ??ng cho ??i t?n c�ng
            Debug.Log("Ng??i ch?i ch?n ??i t?n c�ng");
        }
        else if (selectedOption == "Ph�ng Th?")
        {
            // Th?c hi?n h�nh ??ng cho ??i ph�ng th?
            Debug.Log("Ng??i ch?i ch?n ??i ph�ng th?");
        }
    }
}
