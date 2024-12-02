using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOverlay : MonoBehaviour
{
    public GameObject bloodOverlay; // K�o t?m ?nh m�u t? Canvas v�o ?�y
    public float displayTime = 0.5f;  // Th?i gian hi?n th? ?nh m�u
    private bool isActive = false;  // Ki?m tra tr?ng th�i ?nh m�u

    // Start is called before the first frame update
    void Start()
    {
        if (bloodOverlay != null)
        {
            bloodOverlay.SetActive(false); // ??m b?o ?nh m�u t?t khi b?t ??u
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

    private IEnumerator ActivateBloodOverlay()
    {
        isActive = true;
        bloodOverlay.SetActive(true); // B?t ?nh m�u
        yield return new WaitForSeconds(displayTime); // Ch? 1 gi�y (ho?c th?i gian b?n ??t)
        bloodOverlay.SetActive(false); // T?t ?nh m�u
        isActive = false;
    }
}
