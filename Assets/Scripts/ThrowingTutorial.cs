using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowingTutorial : MonoBehaviour
{
    [Header("Reference")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThorow;

    [Header("Setting")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Reference")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        // kh?i t?o ??i t??ng ?? ném
        GameObject projectile = Instantiate(objectToThorow, attackPoint.position, cam.rotation);
        // l?y thành ph?n rb
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        
        // canh h??ng ném
        Vector3 forceDirection = cam.transform.forward;
        
        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        //l?c ném
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

}
