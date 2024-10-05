using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bacnhay : MonoBehaviour
{
    // ?? cao m� ng??i ch?i s? nh?y l�n khi va ch?m v?i ?i?m gi?m nh?y
    public float jumpHeight = 25f;

    // X? l� s? ki?n khi c� va ch?m v?i Collider
    private void OnTriggerEnter(Collider other)
    {
        // Ki?m tra xem ng??i ch?i c� va ch?m v?i ?i?m gi?m nh?y kh�ng
        if (other.CompareTag("Player"))
        {
            // N?u c�, �p d?ng l?c nh?y l�n ng??i ch?i
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
        }
    }
}
