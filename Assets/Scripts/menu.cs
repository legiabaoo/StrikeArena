using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class menu : MonoBehaviour
{
    public GameObject thoattran;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          
            ToggleOut();
        }
      
    }
    public void thoatgame()
    {

        
    }
    void ToggleOut()
    {
       thoattran.SetActive(!thoattran.activeSelf);

        if (thoattran.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void veSanh()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); 
        }
        SceneManager.LoadScene("LoginScene");
    }
    public void Huy()
    {
        thoattran.SetActive(false);
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
