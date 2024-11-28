using UnityEngine;

public class bomgaydau : MonoBehaviour
{
    public float currentHealth = 100f;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed!");
        Destroy(gameObject);
    }
}
