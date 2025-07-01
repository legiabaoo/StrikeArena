using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider;
    public string url;
    public string doiten;
    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenUrlQuenMK()
    {
        Application.OpenURL(url);
    }
    public void OpenUrlDoiten()
    {
        Application.OpenURL(doiten);
    }
}
