using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOverlay : MonoBehaviour
{
    public GameObject bloodOverlay; // Kéo t?m ?nh máu t? Canvas vào ?ây
    public float displayTime = 0.5f;  // Th?i gian hi?n th? ?nh máu
    private bool isActive = false;  // Ki?m tra tr?ng thái ?nh máu

    // Start is called before the first frame update
    void Start()
    {
        if (bloodOverlay != null)
        {
            bloodOverlay.SetActive(false); // ??m b?o ?nh máu t?t khi b?t ??u
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowBloodEffect()
    {
        if (isActive) return; // Tránh kích ho?t hi?u ?ng nhi?u l?n cùng lúc
        StartCoroutine(ActivateBloodOverlay());
    }

    private IEnumerator ActivateBloodOverlay()
    {
        isActive = true;
        bloodOverlay.SetActive(true); // B?t ?nh máu
        yield return new WaitForSeconds(displayTime); // Ch? 1 giây (ho?c th?i gian b?n ??t)
        bloodOverlay.SetActive(false); // T?t ?nh máu
        isActive = false;
    }
}
