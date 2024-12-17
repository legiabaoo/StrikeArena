using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodOverlay : MonoBehaviour
{
    public GameObject bloodOverlay; 
    public float displayTime = 0.5f;  
    private bool isActive = false;
    public GameObject handUI;

    void Start()
    {
        if (bloodOverlay != null)
        {
            bloodOverlay.SetActive(false); 
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Side")
        {
           
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Side")
        {
           
            handUI.SetActive(false);
        }
    }


    void Update()
    {
        
    }
    public void ShowBloodEffect()
    {
        if (isActive) return; 
        StartCoroutine(ActivateBloodOverlay());
    }
 
    private IEnumerator ActivateBloodOverlay()
    {
        isActive = true;
        bloodOverlay.SetActive(true); 
        yield return new WaitForSeconds(displayTime);  
        bloodOverlay.SetActive(false); 
        isActive = false; 
    }
}
