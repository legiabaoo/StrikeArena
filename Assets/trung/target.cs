using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{

    public float drop = 4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Commence (float amount)
    {
        drop -= amount;
        if (drop<= 0f)
        {
            Go();
        }
    }    

    void Go()
    {
        Destroy(gameObject);
    }
}
