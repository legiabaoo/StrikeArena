using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth = 100;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth; // Cập nhật giá trị ban đầu
    }

    public void SetPlayerHealth(int health)
    {
        healthSlider.value = health; // Gán sức khỏe ban đầu
    }

    public void UpdateHealth(int health)
    {
        healthSlider.value = health; // Cập nhật sức khỏe khi có thay đổi
    }
}
