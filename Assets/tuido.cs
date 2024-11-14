using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuido : MonoBehaviour
{

    public GameObject slot4;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("4"))
        {
            Equipi();
        }

        if (Input.GetKeyDown("5"))
        {
            Equipi();
        }

        void Equipi()
        {
               slot4.SetActive(true);
               
        }

    }
}
