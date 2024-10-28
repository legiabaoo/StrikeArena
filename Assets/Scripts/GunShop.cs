using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;

public class GunShop : MonoBehaviourPun
{
    public GameObject shopUI;                  // UI c?a h�ng
    public int playerMoney = 500;              // Ti?n c?a ng??i ch?i
    public int gunPrice = 100;                 // Gi� c?a m?i s�ng
    public GameObject[] gunPrefabs;            // M?ng ch?a c�c prefab s�ng

    private Transform gunPosition;             // V? tr� g?n s�ng trong nh�n v?t
    private bool gunPositionFound = false;     // C? ?? ki?m tra xem ?� t�m th?y v? tr� g?n s�ng ch?a
    public Vector3[] gunPositions; // M?ng l?u v? tr� c? ??nh c?a m?i c�y s�ng
    public Quaternion[] gunRotations; // M?ng l?u rotation c? ??nh c?a m?i c�y s�ng
    private WeaponSwitcher weaponSwitcher;
    void Start()
    {
        // Ki?m tra gunPrefabs
        if (gunPrefabs == null || gunPrefabs.Length == 0)
        {
            Debug.LogError("gunPrefabs kh�ng ???c g�n ho?c tr?ng!");
        }
        gunPositions = new Vector3[10]; // 10 c�y s�ng
        gunRotations = new Quaternion[10];
       
        // V� d? g�n v? tr� v� rotation cho 10 c�y s�ng
        gunPositions[0] = new Vector3(-669.460022f, -221.089996f, 1f); // V? tr� cho s�ng s? 1
        gunRotations[0] = Quaternion.Euler(13, -99.3f, 13); // Rotation cho s�ng s? 1

        gunPositions[1] = new Vector3(-669.1f, -221.3f, 1.32f); // V? tr� cho s�ng s? 2
        gunRotations[1] = Quaternion.Euler(-1.24f, 86.72f, 1.38f); // Rotation cho s�ng s? 2


        gunPositions[2] = new Vector3(-669.6f, -221.3f, 0.7f); // V? tr� cho s�ng s? 3
        gunRotations[2] = Quaternion.Euler(-178.8f, 0.86f, 0.9f); // Rotation cho s�ng s? 3
    }

    void Update()
    {
        // T�m nh�n v?t c?a ng??i ch?i v� v? tr� g?n s�ng n?u ch?a t�m th?y
        if (!gunPositionFound && PhotonNetwork.IsConnected)
        {
            FindPlayerGunPosition();
        }

        // M? UI c?a h�ng khi nh?n ph�m B
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShopUI();
        }
    }

    void ToggleShopUI()
    {
        shopUI.SetActive(!shopUI.activeSelf);

        if (shopUI.activeSelf)
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


    void FindPlayerGunPosition()
{
    foreach (var player in FindObjectsOfType<PhotonView>())
    {
        // Ch? ki?m tra ??i t??ng c?a ng??i ch?i hi?n t?i
        if (player.IsMine)
        {
                Vector3 playerPosition = player.transform.position;
                Debug.Log("V? tr� c?a ng??i ch?i: " + playerPosition);
                // T�m `GunPosition` theo ???ng d?n ??y ?? trong c?u tr�c `Hierarchy`
                gunPosition = player.transform.Find("Main Camera/GunPosition"); // ?� ?i?u ch?nh ???ng d?n

            if (gunPosition != null)
            {
                gunPositionFound = true; // ?� t�m th?y v? tr� g?n s�ng
                Debug.Log("?� t�m th?y v? tr� GunPosition: " + gunPosition.position);
                
                // L?y v? tr� c?a ng??i ch?i
              
            }
            else
            {
                Debug.LogWarning("Kh�ng t�m th?y v? tr� GunPosition. Ki?m tra ???ng d?n trong c?u tr�c nh�n v?t.");
            }
            break; // K?t th�c v�ng l?p sau khi t�m th?y ng??i ch?i
        }
    }
}




    public void BuyGunButton(int gunIndex)
    {
        Debug.Log("Mua s�ng v?i ch? s?: " + gunIndex);
        Debug.Log("Ti?n ng??i ch?i: " + playerMoney);
        Debug.Log("vi tri sung: " + gunIndex);
        if (gunPosition == null)
        {
            Debug.LogError("Kh�ng t�m th?y v? tr� g?n s�ng (gunPosition)!");
            return;
        }

        if (gunIndex < 0 || gunIndex >= gunPrefabs.Length)
        {
            Debug.LogError("Ch? s? s�ng kh�ng h?p l?: " + gunIndex);
            return;
        }

        if (playerMoney >= gunPrice)
        {
            playerMoney -= gunPrice; // Tr? ti?n khi mua
            photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
        }
        else
        {
            Debug.Log("Kh�ng ?? ti?n ?? mua s�ng.");
        }
        /*if (gunIndex == 2 )
        {
            weaponSwitcher.SetNutbanImageActive(false);
        }
        else
        {
            weaponSwitcher.SetNutbanImageActive(true);
        }*/
    }
  

    [PunRPC]
    void CreateGunForPlayer(int playerID, int gunIndex)
    {
        // Ki?m tra n?u ?�y l� ng??i ch?i hi?n t?i v� s�ng h?p l?
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID && gunIndex >= 0 && gunIndex < gunPrefabs.Length)
        {
            // L?y v? tr� v� rotation cho s�ng t??ng ?ng
            Vector3 fixedPosition = gunPositions[gunIndex];
            Quaternion fixedRotation = gunRotations[gunIndex];

            // T?o s�ng t?i v? tr� v� rotation c? ??nh
            GameObject gunInstance = Instantiate(gunPrefabs[gunIndex], fixedPosition, fixedRotation);

            // G�n c�y s�ng v�o gunPosition n?u c?n thi?t, n?u kh�ng th� g�n null
            gunInstance.transform.SetParent(gunPosition,false); // G�n kh�ng c� cha
            Weapon weaponScript = gunInstance.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                // T�m camera c?a ng??i ch?i qua tag ho?c qua c?u tr�c hierarchy
                Camera playerCamera = Camera.main; // N?u ?� g�n tag "MainCamera" cho camera ng??i ch?i
                if (playerCamera != null)
                {
                    weaponScript.camera = playerCamera;
                }
                TextMeshProUGUI ammoText = GameObject.FindWithTag("AmmoText")?.GetComponent<TextMeshProUGUI>();
                if (ammoText != null)
                {
                    weaponScript.ammoText = ammoText;
                }

            }
          /*  nutNgam nutNgam = gunInstance.GetComponent<nutNgam>();
        
            // T�m ??i t??ng Player c?a ng??i ch?i hi?n t?i
            PhotonView playerView = FindObjectsOfType<PhotonView>().FirstOrDefault(p => p.IsMine);
            if (playerView != null)
            {
                Transform nutngamTransform = playerView.transform.Find("Main Camera/Cavas/nutNgam");
                if (nutngamTransform != null)
                {
                    GameObject nutngam = nutngamTransform.gameObject;
                    nutNgam.aimImage = nutngam;
                }
                else
                {
                    Debug.LogWarning("Kh�ng t�m th?y nutngam trong c?u tr�c Player.");
                }
            }
            else
            {
                Debug.LogWarning("Kh�ng t�m th?y ??i t??ng Player c?a ng??i ch?i hi?n t?i.");
            }*/

            Debug.Log("S�ng s? " + gunIndex + " ???c t?o t?i v? tr�: " + fixedPosition + " v?i rotation: " + fixedRotation.eulerAngles);
        }
    }



}
