using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacToCam : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
