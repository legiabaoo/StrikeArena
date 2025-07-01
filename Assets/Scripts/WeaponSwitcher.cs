using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public static WeaponSwitcher instance;
    public int selectedWeapon = 0;
    public GameObject nutbanImage;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }
     public void SetNutbanImageActive(bool isActive)
    {
        nutbanImage.SetActive(isActive);
    }
    // Update is called once per frame
    void Update()
    {
        SelectWeapon();
        int previousSelectedWeapon = selectedWeapon;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            nutbanImage.SetActive(true);
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            nutbanImage.SetActive(true);
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            nutbanImage.SetActive(false);
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeapon = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedWeapon = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedWeapon = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedWeapon = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedWeapon = 8;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon += 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon -= 1;
            }
        }


        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }
    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform _weapon in transform)
        {
            if(i == selectedWeapon) 
            { 
                _weapon.gameObject.SetActive(true); 
            }else
            {
                _weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
