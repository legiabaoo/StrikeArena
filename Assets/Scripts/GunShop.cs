using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using scgFullBodyController;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GunShop : MonoBehaviourPun
{
    public static GunShop instance;
    public GameObject shopUI;                  // UI c?a hàng
    public int playerMoney = 800;              // Ti?n c?a ng??i ch?i
    public int gunPrice = 100;                 // Giá c?a m?i súng
    public GameObject[] gunPrefabs;            // M?ng ch?a các prefab súng

    public Transform gunPosition;             // V? trí g?n súng trong nhân v?t
    private bool gunPositionFound = false;     // C? ?? ki?m tra xem ?ă t́m th?y v? trí g?n súng ch?a
    public Vector3[] gunPositions; // M?ng l?u v? trí c? ??nh c?a m?i cây súng
    public Quaternion[] gunRotations; // M?ng l?u rotation c? ??nh c?a m?i cây súng
    private WeaponSwitcher weaponSwitcher;
    private Text txtTien;
    private GameObject playerObject; // Người chơi cục bộ
    private Animator playerAnimator;
    private Animator playerCamAnim;
    private GameObject cam;
    private GameObject campoint;
    private Transform righthand;
    private Transform lefthand;
    private Transform head;
    private Transform ngontay;
    private void Awake()
    {
        instance = this;
    }
    //public void AssignGunPosition(Transform playerGunPosition)
    //{
    //    gunPosition = playerGunPosition;

    //    if (gunPosition != null)
    //    {
    //        Debug.Log($"GunPos đã được gán cho GunShop: {gunPosition.name} tại {gunPosition.position}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("GunPos chưa được gán cho GunShop!");
    //    }
    //}
    void Start()
    {
        StartCoroutine(CheckForPlayerObject());

      
        // Ki?m tra gunPrefabs
        if (gunPrefabs == null || gunPrefabs.Length == 0)
        {
            Debug.LogError("gunPrefabs không ???c gán ho?c tr?ng!");
        }

        gunPositions = new Vector3[10]; // 10 cây súng
        gunRotations = new Quaternion[10];

        // Ví d? gán v? trí và rotation cho 10 cây súng
        gunPositions[0] = new Vector3(0.03108988f, -0.01033462f, 0.07142594f); // V? trí cho súng s? 1
        gunRotations[0] = Quaternion.Euler(1 - 9.366f, -75.064f, 25.989f); // Rotation cho súng s? 1

        gunPositions[1] = new Vector3(0.03270616f, 0.006453183f, 0.06735089f); // V? trí cho súng s? 2
        gunRotations[1] = Quaternion.Euler(188.205f, 102.547f, 206.354f); // Rotation cho súng s? 2


        gunPositions[2] = new Vector3(0.03270616f, 0.006453183f, 0.06735089f); // V? trí cho súng s? 3
        gunRotations[2] = Quaternion.Euler(-8.205f, -77.453f, 26.354f); // Rotation cho súng s? 3

        gunPositions[3] = new Vector3(0.03270616f, 0.006453183f, 0.06735089f); // V? trí cho súng s? 3
        gunRotations[3] = Quaternion.Euler(-8.205f, -77.453f, 26.354f);

        gunPositions[4] = new Vector3(0.03108988f, -0.01033462f, 0.07142594f); // V? trí cho súng s? 1
        gunRotations[4] = Quaternion.Euler(1 - 9.366f, -75.064f, 25.989f); // Rotation cho súng s? 1

        gunPositions[5] = new Vector3(0.03108988f, -0.01033462f, 0.07142594f); // V? trí cho súng s? 1
        gunRotations[5] = Quaternion.Euler(1 - 9.366f, -75.064f, 25.989f); // Rotation cho súng s? 1

        
    }
    
    void Update()
    {
        StartCoroutine(CheckForPlayerObject());
        //SetMoney(playerMoney);
        // T́m nhân v?t c?a ng??i ch?i và v? trí g?n súng n?u ch?a t́m th?y
        if (!gunPositionFound && PhotonNetwork.IsConnected)
        {
            
            FindPlayerGunPosition();
            Debug.Log("CheckForPlayer1");
        }

        // M? UI c?a hàng khi nh?n phím B
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShopUI();
        }
    }
    private IEnumerator CheckForPlayerObject()
    {
        // Chờ cho đến khi nhân vật được tạo
        while (playerObject == null)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    playerObject = player;
                    playerAnimator = playerObject.GetComponent<Animator>();
                    GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                    if (cameraObject != null)
                    {
                        playerCamAnim = cameraObject.GetComponent<Animator>();
                    }
                    cam = cameraObject;
                    GameObject cameraPoint = GameObject.FindGameObjectWithTag("campoint");
                    campoint = cameraPoint;
                    GameObject tayphai = GameObject.FindGameObjectWithTag("RightHand");
                    righthand = tayphai.GetComponent<Transform>();
                    GameObject taytrai = GameObject.FindGameObjectWithTag("LeftHand");
                    lefthand = taytrai.GetComponent<Transform>();
                    GameObject dau = GameObject.FindGameObjectWithTag("head");
                    head = dau.GetComponent<Transform>();
                    GameObject tay = GameObject.FindGameObjectWithTag("ngontay");
                    ngontay = tay.GetComponent<Transform>();

                    break;
                }
            }
            yield return new WaitForSeconds(1f);  // Kiểm tra lại sau 1 giây
        }

        // Sau khi tìm thấy playerObject, bật script GunShop
        enabled = true;

        //Debug.Log("PlayerObject đã được tìm thấy, bật script GunShop.");
    }
    public void ResetGunPosition()
    {
        gunPositionFound = false;
        FindPlayerGunPosition(); // Gọi lại hàm tìm vị trí để cập nhật gunPosition
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


    public void FindPlayerGunPosition()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("viewID", out var viewIDValue) &&
                viewIDValue is int viewID)
            {
                GameObject playerObject = PhotonView.Find(viewID)?.gameObject;
                if (playerObject != null)
                {
                    // Tìm Main Camera trong cấu trúc con của GameObject này
                    gunPosition = playerObject.transform.Find("Ch15_nonPBR/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/GunPos");
                    if (gunPosition != null)
                    {
                        gunPositionFound = true;
                        Debug.Log("GunPO");
                    }
                    else
                    {
                        gunPositionFound = false;
                        Debug.Log("GunPO nul roio");
                    }
                }
                else
                {
                    Debug.LogWarning($"No GameObject found for Player: {player.NickName}");
                }
            }
        }
    }
    //public void FindPlayerGunPosition()
    //{
    //    GameObject localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;

    //    if (localPlayer != null)
    //    {
    //        PlayerSetup playerSetup = localPlayer.GetComponent<PlayerSetup>();
    //        if (playerSetup != null && playerSetup.gunPositionFound)
    //        {
    //            Transform gunPosition = playerSetup.gunPosition;
    //            Debug.Log($"GunPos found at {gunPosition.position}");
    //            gunPositionFound = true;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("GunPos not found for local player!");
    //        }
    //    }

    //}


    public void SetMoney(int newMoneyAmount)
    {
        foreach (var player in FindObjectsOfType<PhotonView>())
        {
            // Kiểm tra nếu đây là đối tượng của người chơi hiện tại
            if (player.IsMine)
            {
                // Cập nhật giá trị tiền mới
                playerMoney = newMoneyAmount;

                // Tìm đối tượng Text để hiển thị số tiền
                var txtTienTransform = player.transform.Find("CameraControl/Main Camera/Canvas/Tien");
                if (txtTienTransform != null)
                {
                    Text txtTien = txtTienTransform.GetComponentInChildren<Text>();
                    if (txtTien != null)
                    {
                        txtTien.text = playerMoney.ToString();
                    }
                    else
                    {
                        Debug.LogWarning("Không tìm thấy thành phần Text trên đối tượng 'Tien'.");
                    }
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy đối tượng 'Canvas/Tien' trong Main Camera.");
                }
                
            }
        }
    }


    public void BuyGunButton(int gunIndex)
    {
        Debug.Log("khong hoat dong");
        if (gunIndex < 0 || gunIndex >= gunPrefabs.Length)
        {
            Debug.LogError("Ch? s? súng không h?p l?: " + gunIndex);
            return;
        }
        if (gunPosition == null)
        {
            Debug.Log("gun pos bi null");
            return;
        }
        if (gunIndex == 0)
        {
            Debug.Log("Súng M500");
            gunPrice = 100;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
        else if (gunIndex == 1)
        {
            Debug.Log("Súng Ak47");
            gunPrice = 200;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
        else if (gunIndex == 2)
        {
            Debug.Log("Súng M4A1");
            gunPrice = 300;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
        else if (gunIndex == 3)
        {
            Debug.Log("Súng ngắm");
            gunPrice = 300;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
        else if (gunIndex == 4)
        {
            Debug.Log("bom");
            gunPrice = 5;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                ThrowingTutorial.Instance.totalBom++;
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
        else if (gunIndex == 5)
        {
            Debug.Log("khoi");
            gunPrice = 5;
            if (playerMoney >= gunPrice)
            {
                int tien = playerMoney - gunPrice; // Tr? ti?n khi mua
                SetMoney(tien);
                ThrowingTutorial.Instance.totalSmoke++;
                photonView.RPC("CreateGunForPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, gunIndex);
            }
            else
            {
                Debug.Log("Không ?? ti?n ?? mua súng.");
            }
        }
       
    }


    [PunRPC]
    void CreateGunForPlayer(int playerID, int gunIndex)
    {
        // Ki?m tra n?u ?ây là ng??i ch?i hi?n t?i và ch? s? súng h?p l?
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID && gunIndex >= 0 && gunIndex < gunPrefabs.Length)
        {
            // L?y v? trí và rotation cho súng t??ng ?ng
            Vector3 fixedPosition = gunPositions[gunIndex];
            Quaternion fixedRotation = gunRotations[gunIndex];

            // T?o và ??ng b? cây súng trên t?t c? client
            GameObject gunInstance = PhotonNetwork.Instantiate(gunPrefabs[gunIndex].name, fixedPosition, fixedRotation);
            gunInstance.SetActive(false);
            GunController gunController = gunInstance.GetComponent<GunController>();
            if (gunController != null)
            {
                // Truyền các giá trị từ Player vào GunController
                gunController.anim = playerAnimator;
                gunController.camAnim = playerCamAnim;
                gunController.mainCam = cam;
                gunController.shootPointCamera = campoint;
                gunController.mainHandTransform = righthand;
            }
            else
            {
                Debug.LogError("Không tìm thấy GunController trên cây súng!");
            }
            Adjuster adj = gunInstance.GetComponent<Adjuster>();
            if (adj != null)
            {
                adj.handBone = lefthand;
                adj.headBone = head;
                adj.indexFinger = ngontay;
            }
            else
            {
                Debug.LogError("Không tìm thấy Adjuster trên cây súng!");
            }
            // Lấy con đầu tiên của gunPosition (game con thứ 1)
            Transform oldChild = gunPosition.childCount > 0 ? gunPosition.GetChild(gunIndex) : null;
            GunManager gunManager = playerObject.GetComponent<GunManager>();
            if (gunManager != null)
            {
                gunManager.AddWeaponToInventory(gunInstance);
            }
            else
            {
                Debug.LogError("Không tìm thấy GunManager trên PlayerObject!");
            }
            /*  // Kiểm tra và xóa game con cũ nếu nó tồn tại
              if (oldChild != null)
              {
                  Destroy(oldChild.gameObject); // Hoặc dùng `oldChild.gameObject.SetActive(false);` nếu chỉ muốn ẩn đi
              }
  */
            // Đặt gunInstance làm con của gunPosition
            photonView.RPC(nameof(SetGunParent), RpcTarget.AllBuffered, gunInstance.GetComponent<PhotonView>().ViewID);
            gunInstance.transform.SetSiblingIndex(gunIndex);

            // Thi?t l?p các thu?c tính cho script Weapon n?u c?n
            Weapon weaponScript = gunInstance.GetComponent<Weapon>();
            if (weaponScript != null)
            {
                // T́m camera c?a ng??i ch?i qua tag ho?c c?u trúc hierarchy
                Camera playerCamera = Camera.main; // N?u ?ă gán tag "MainCamera" cho camera ng??i ch?i
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

            Debug.Log("Súng số " + gunIndex + " ?ă ???c t?o t?i v? trí: " + fixedPosition + " v?i rotation: " + fixedRotation.eulerAngles);
        }
    }
    [PunRPC]
    void SetGunParent(int gunViewID)
    {
        // Lấy GameObject của súng từ ViewID
        GameObject gunObject = PhotonView.Find(gunViewID)?.gameObject;
        if (gunObject != null && gunPosition != null)
        {
            // Gán Parent và cập nhật vị trí/rotation
            gunObject.transform.SetParent(gunPosition, false);

            // Nếu cần, điều chỉnh lại vị trí hoặc rotation của súng
         
          /*  gunObject.SetActive(true); // Hiển thị súng sau khi gán Parent*/
            Debug.Log($"Gun {gunObject.name} set as child of {gunPosition.name}.");
        }
        else
        {
            Debug.LogWarning("Failed to set gun parent! Gun or GunPosition is null.");
        }
    }



}
