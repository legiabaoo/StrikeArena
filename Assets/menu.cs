using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          
            thoatgame();
        }
      
    }
    public void thoatgame()
    {
        Application.Quit();
    }
    public void trove()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
        SceneManager.UnloadSceneAsync("choi");
    }
    public void choingay()
    {
        SceneManager.LoadScene("choi");

    }
    public void huongdan()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Huongdan");
    }
}
