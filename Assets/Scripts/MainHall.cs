using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainHall : MonoBehaviour
{
    public TMP_Text username;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play()
    {
        RoomManager.instance.username.text = username.text;
        SceneManager.LoadScene("Scene1");
    }
}
