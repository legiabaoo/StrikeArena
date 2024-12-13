using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOverlay : MonoBehaviour
{
    public GameObject bloodOverlay; // K�o t?m ?nh m�u t? Canvas v�o ?�y
    public float displayTime = 0.5f;  // Th?i gian hi?n th? ?nh m�u
    private bool isActive = false;  // Ki?m tra tr?ng th�i ?nh m�u

 
    void Start()
    {
        if (bloodOverlay != null)
        {
            bloodOverlay.SetActive(false); // ??m b?o ?nh m�u b? t?t khi b?t ??u
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowBloodEffect()
    {
        if (isActive) return; // Tr�nh k�ch ho?t hi?u ?ng nhi?u l?n c�ng l�c
        StartCoroutine(ActivateBloodOverlay());
    }
    // Coroutine x? l� b?t v� t?t hi?u ?ng m�u
    private IEnumerator ActivateBloodOverlay()
    {
        isActive = true;
        bloodOverlay.SetActive(true); // B?t ?nh m�u tr�n m�n h�nh
        yield return new WaitForSeconds(displayTime);  // Ch? th?i gian hi?n th? ???c ch? ??nh
        bloodOverlay.SetActive(false); // T?t ?nh m�u
        isActive = false; // ??t l?i tr?ng th�i hi?u ?ng
    }
}
