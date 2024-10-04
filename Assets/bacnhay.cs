using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bacnhay : MonoBehaviour
{
    // ?? cao mà ng??i ch?i s? nh?y lên khi va ch?m v?i ?i?m gi?m nh?y
    public float jumpHeight = 25f;

    // X? lý s? ki?n khi có va ch?m v?i Collider
    private void OnTriggerEnter(Collider other)
    {
        // Ki?m tra xem ng??i ch?i có va ch?m v?i ?i?m gi?m nh?y không
        if (other.CompareTag("Player"))
        {
            // N?u có, áp d?ng l?c nh?y lên ng??i ch?i
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
        }
    }
}
