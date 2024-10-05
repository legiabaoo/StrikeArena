using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nutNgam : MonoBehaviour
{// ??i t??ng hình ?nh c?a nút ng?m
    public GameObject aimImage;
    public GameObject nutbanImage;

    // Giá tr? field of view ban ??u
    private float initialFieldOfView;

    // Giá tr? field of view khi zoom
    public float zoomFieldOfView = 30f;

    void Start()
    {
        // L?u l?i kích th??c ban ??u c?a nút ng?m và ?n nó ?i
        initialFieldOfView = Camera.main.fieldOfView;
        aimImage.SetActive(false);
        nutbanImage.SetActive(false);
    }

    void Update()
    {
        // Ki?m tra khi nút chu?t ph?i ???c nh?n
        // Ki?m tra khi nút chu?t ph?i ???c nh?n
        if (Input.GetMouseButtonDown(1))
        {
            aimImage.SetActive(true);
            // Zoom màn hình l?i
            Camera.main.fieldOfView = zoomFieldOfView;
        }
        // Ki?m tra khi nút chu?t ph?i ???c th? ra
        else if (Input.GetMouseButtonUp(1))
        {
            aimImage.SetActive(false);
            // Khôi ph?c giá tr? field of view ban ??u c?a Camera
            Camera.main.fieldOfView = initialFieldOfView;
        }
    }
}
