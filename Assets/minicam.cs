using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minicam : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;


    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
        Vector3 vector3 = new Vector3(90, player.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(vector3);
    }
}
