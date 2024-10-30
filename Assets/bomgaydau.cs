using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    [Header("Stats")]
    public int health;

    public void TakeDamege(int damge)
    {
        health -= damge;

        if (health <- 0 )
            Destroy(gameObject);
    }
}
