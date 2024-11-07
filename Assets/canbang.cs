using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canbang : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject Gun;
    public Camera Camera;
    public float range = 2f; // Tầm xa của raycast
    public float pickUpRange = 2f; // Khoảng cách tối đa để nhặt súng

    void Start()
    {
        Gun.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            EquipObject();
        }

        if (Input.GetKeyDown("g"))
        {
            UnequipObject();
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            target target = hit.transform.GetComponent<target>();
            if (target != null)
            {
                EquipObject();
            }
        }
    }

    void UnequipObject()
    {
        PlayerTransform.DetachChildren();
        Gun.transform.eulerAngles = new Vector3(Gun.transform.eulerAngles.x, Gun.transform.eulerAngles.y, Gun.transform.eulerAngles.z - 45);
        Gun.GetComponent<Rigidbody>().isKinematic = false;
    }

    void EquipObject()
    {
        // Kiểm tra khoảng cách giữa người chơi và súng
        float distanceToGun = Vector3.Distance(PlayerTransform.position, Gun.transform.position);
        if (distanceToGun <= pickUpRange)
        {
            Gun.GetComponent<Rigidbody>().isKinematic = true;
            Gun.transform.position = PlayerTransform.transform.position;
            Gun.transform.rotation = PlayerTransform.transform.rotation;
            Gun.transform.SetParent(PlayerTransform);
            Debug.Log("Đã nhặt súng.");
        }
        else
        {
            Debug.Log("Quá xa để nhặt súng.");
        }
    }
}
