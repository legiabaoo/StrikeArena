using Photon.Pun;
using Photon.Realtime;
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
    private void Awake()
    {
        instance = this;
    }
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

        gunPositions[3] = new Vector3(-669.454224f, -221.10997f, 1.03728712f); // V? trí cho súng s? 3
        gunRotations[3] = Quaternion.Euler(-1.06721711e-06f, 354.690002f, 18.2900028f);

        gunPositions[4] = new Vector3(-669.454224f, -221.10997f, 1.03728712f); // V? trí cho súng s? 3
        gunRotations[4] = Quaternion.Euler(-1.06721711e-06f, 354.690002f, 18.2900028f);
    }

    void Update()
    {
        //SetMoney(playerMoney);
        // T́m nhân v?t c?a ng??i ch?i và v? trí g?n súng n?u ch?a t́m th?y
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
        foreach (var player in FindObjectsOfType<PhotonView>())
        {
            // Ch? ki?m tra ??i t??ng c?a ng??i ch?i hi?n t?i
            if (player.IsMine)
            {
                Vector3 playerPosition = player.transform.position;

                gunPosition = player.transform.Find("Main Camera/GunPosition");
                
                if (gunPosition != null)
                {
                    gunPositionFound = true;
                }
                else
                {
                    gunPositionFound = false;
                }
                break;
            }
        }
    }
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
                var txtTienTransform = player.transform.Find("Main Camera/Canvas/Tien");
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
        if (gunIndex < 0 || gunIndex >= gunPrefabs.Length)
        {
            Debug.LogError("Ch? s? súng không h?p l?: " + gunIndex);
            return;
        }
        if (gunPosition == null)
        {
            return;
        }
        if (gunIndex == 0)
        {
            Debug.Log("Súng 0");
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
            Debug.Log("Súng 1");
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
            Debug.Log("Súng 3");
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
        else if (gunIndex == 4)
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
            // Lấy con đầu tiên của gunPosition (game con thứ 1)
            Transform oldChild = gunPosition.childCount > 0 ? gunPosition.GetChild(gunIndex) : null;

            // Kiểm tra và xóa game con cũ nếu nó tồn tại
            if (oldChild != null)
            {
                Destroy(oldChild.gameObject); // Hoặc dùng `oldChild.gameObject.SetActive(false);` nếu chỉ muốn ẩn đi
            }

            // Đặt gunInstance làm con của gunPosition
            gunInstance.transform.SetParent(gunPosition, false);
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



}
