using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nutNgam : MonoBehaviour
{// ??i t??ng h�nh ?nh c?a n�t ng?m
    public GameObject aimImage;
    public GameObject nutbanImage;

    // Gi� tr? field of view ban ??u
    private float initialFieldOfView;

    // Gi� tr? field of view khi zoom
    public float zoomFieldOfView = 30f;

    void Start()
    {
        // L?u l?i k�ch th??c ban ??u c?a n�t ng?m v� ?n n� ?i
        initialFieldOfView = Camera.main.fieldOfView;
        aimImage.SetActive(false);
        nutbanImage.SetActive(false);
    }

    void Update()
    {
        // Ki?m tra khi n�t chu?t ph?i ???c nh?n
        // Ki?m tra khi n�t chu?t ph?i ???c nh?n
        if (Input.GetMouseButtonDown(1))
        {
            aimImage.SetActive(true);
            // Zoom m�n h�nh l?i
            Camera.main.fieldOfView = zoomFieldOfView;
        }
        // Ki?m tra khi n�t chu?t ph?i ???c th? ra
        else if (Input.GetMouseButtonUp(1))
        {
            aimImage.SetActive(false);
            // Kh�i ph?c gi� tr? field of view ban ??u c?a Camera
            Camera.main.fieldOfView = initialFieldOfView;
        }
    }
}
