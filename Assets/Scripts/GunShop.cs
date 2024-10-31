using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;

public class GunShop : MonoBehaviourPun
{
    public GameObject shopUI;                  // UI c?a hàng
    public int playerMoney = 500;              // Ti?n c?a ng??i ch?i
    public int gunPrice = 100;                 // Giá c?a m?i súng
    public GameObject[] gunPrefabs;            // M?ng ch?a các prefab súng

    private Transform gunPosition;             // V? trí g?n súng trong nhân v?t
    private bool gunPositionFound = false;     // C? ?? ki?m tra xem ?ã tìm th?y v? trí g?n súng ch?a
    public Vector3[] gunPositions; // M?ng l?u v? trí c? ??nh c?a m?i cây súng
    public Quaternion[] gunRotations; // M?ng l?u rotation c? ??nh c?a m?i cây súng
    private WeaponSwitcher weaponSwitcher;
    void Start()
    {
        // Ki?m tra gunPrefabs
        if (gunPrefabs == null || gunPrefabs.Length == 0)
        {
            Debug.LogError("gunPrefabs không ???c gán ho?c tr?ng!");
        }
        gunPositions = new Vector3[10]; // 10 cây súng
        gunRotations = new Quaternion[10];
       
        // Ví d? gán v? trí và rotation cho 10 cây súng
        gunPositions[0] = new Vector3(-669.460022f, -221.089996f, 1f); // V? trí cho súng s? 1
        gunRotations[0] = Quaternion.Euler(13, -99.3f, 13); // Rotation cho súng s? 1

        gunPositions[1] = new Vector3(-669.1f, -221.3f, 1.32f); // V? trí cho súng s? 2
        gunRotations[1] = Quaternion.Euler(-1.24f, 86.72f, 1.38f); // Rotation cho súng s? 2


        gunPositions[2] = new Vector3(-669.6f, -221.3f, 0.7f); // V? trí cho súng s? 3
        gunRotations[2] = Quaternion.Euler(-178.8f, 0.86f, 0.9f); // Rotation cho súng s? 3
    }

    void Update()
    {
        // Tìm nhân v?t c?a ng??i ch?i và v? trí g?n súng n?u ch?a tìm th?y
        if (!gunPositionFound && PhotonNetwork.IsConnected)
        {
            FindPlayerGunPosition();
        }

        // M? UI c?a hàng khi nh?n phím B
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
                Debug.Log("V? trí c?a ng??i ch?i: " + playerPosition);
                // Tìm `GunPosition` theo ???ng d?n ??y ?? trong c?u trúc `Hierarchy`
                gunPosition = player.transform.Find("Main Camera/GunPosition"); // ?ã ?i?u ch?nh ???ng d?n

            if (gunPosition != null)
            {
                gunPositionFound = true; // ?ã tìm th?y v? trí g?n súng
                Debug.Log("?ã tìm th?y v? trí GunPosition: " + gunPosition.position);
                
                // L?y v? trí c?a ng??i ch?i
              
            }
            else
            {
                Debug.LogWarning("Không tìm th?y v? trí GunPosition. Ki?m tra ???ng d?n trong c?u trúc nhân v?t.");
            }
            break; // K?t thúc vòng l?p sau khi tìm th?y ng??i ch?i
        }
    }
}




    public void BuyGunButton(int gunIndex)
    {
        Debug.Log("Mua súng v?i ch? s?: " + gunIndex);
        Debug.Log("Ti?n ng??i ch?i: " + playerMoney);
        Debug.Log("vi tri sung: " + gunIndex);
        if (gunPosition == null)
        {
            Debug.LogError("Không tìm th?y v? trí g?n súng (gunPosition)!");
            return;
        }

        if (gunIndex < 0 || gunIndex >= gunPrefabs.Length)
        {
            Debug.LogError("Ch? s? súng không h?p l?: " + gunIndex);
            return;
        }

        if (playerMoney >= gunPrice)
        {
            playerMoney -= gunPrice; // Tr? ti?n khi mua
            photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
        }
        else
        {
            Debug.Log("Không ?? ti?n ?? mua súng.");
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
        // Ki?m tra n?u ?ây là ng??i ch?i hi?n t?i và súng h?p l?
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID && gunIndex >= 0 && gunIndex < gunPrefabs.Length)
        {
            // L?y v? trí và rotation cho súng t??ng ?ng
            Vector3 fixedPosition = gunPositions[gunIndex];
            Quaternion fixedRotation = gunRotations[gunIndex];

            // T?o súng t?i v? trí và rotation c? ??nh
            GameObject gunInstance = Instantiate(gunPrefabs[gunIndex], fixedPosition, fixedRotation);

            // Gán cây súng vào gunPosition n?u c?n thi?t, n?u không thì gán null
            gunInstance.transform.SetParent(gunPosition,false); // Gán không có cha
            Weapon weaponScript = gunInstance.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                // Tìm camera c?a ng??i ch?i qua tag ho?c qua c?u trúc hierarchy
                Camera playerCamera = Camera.main; // N?u ?ã gán tag "MainCamera" cho camera ng??i ch?i
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
        
            // Tìm ??i t??ng Player c?a ng??i ch?i hi?n t?i
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
                    Debug.LogWarning("Không tìm th?y nutngam trong c?u trúc Player.");
                }
            }
            else
            {
                Debug.LogWarning("Không tìm th?y ??i t??ng Player c?a ng??i ch?i hi?n t?i.");
            }*/

            Debug.Log("Súng s? " + gunIndex + " ???c t?o t?i v? trí: " + fixedPosition + " v?i rotation: " + fixedRotation.eulerAngles);
        }
    }



}
