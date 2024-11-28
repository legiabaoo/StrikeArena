using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    // Start is called before the first frame update
   public void chinnhnhac()
    {
        float amluong = musicSlider.value;
        myMixer.SetFloat("nhac", Mathf.Log10(amluong)*20);
    }    
}
