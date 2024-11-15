using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainHall : MonoBehaviour
{
    public TMP_Text username;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        username.text = PlayerPrefs.GetString("Username");
    }
    public void Play()
    {
        SceneManager.LoadScene("Scene1");
    }
}
