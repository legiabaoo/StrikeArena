using DevionGames;
using Photon.Pun;
using Photon.Realtime;
using scgFullBodyController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerSetup : MonoBehaviour
{
    public ThirdPersonUserControl movemnet;
    public Camera cameraPlayer;
    public Canvas canvas;
    public Canvas minimapCanvas;
    public string nickname;
    public TextMeshPro nickNameText;
    private PhotonView photonView;
    [SerializeField]
    private int actorNumber; // Biến này sẽ hiển thị trong Inspector
    //public Transform gunPosition; // Biến public để kéo thả GunPos
    //public bool gunPositionFound => gunPosition != null;
    public Transform gunPosition;
    public Camera minimap;
    Transform spinerCameraTransform;
    public GameObject txtName;
    void Awake()
    {
        photonView = GetComponent<PhotonView>(); // Gán photonView ở đây
        cameraPlayer.enabled = false;
        cameraPlayer.GetComponent<AudioListener>().enabled = false;
        cameraPlayer.GetComponent<MouseLook>().enabled = false;
        gunPosition = gameObject.transform.Find("Ch15_nonPBR/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/GunPos");
        spinerCameraTransform = gameObject.transform.Find("CameraControl/Main Camera/sniperCamera");
        txtName = gameObject.transform.Find("Nickname/Text (TMP)").gameObject;
        minimapCanvas = gameObject.transform.Find("MinapMapCanvas").gameObject.GetComponent<Canvas>();
    }
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer != null)
        {
            actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        }
        //if (photonView.IsMine) // Chỉ thực hiện trên máy của người chơi cục bộ
        //{
        //    GunShop gunShop = FindObjectOfType<GunShop>(); // Tìm GunShop trong Scene

        //    if (gunShop != null)
        //    {
        //        gunShop.AssignGunPosition(gunPosition); // Gán gunPosition cho GunShop
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Không tìm thấy GunShop trong Scene!");
        //    }
        //}
    }

    public void IsLocalPlayer()
    {
        gunPosition.gameObject.SetActive(true);
        movemnet.enabled = true;
        cameraPlayer.enabled = true;
        minimap.enabled = true;
        canvas.gameObject.SetActive(true);
        minimapCanvas.enabled=true;
        cameraPlayer.GetComponent<AudioListener>().enabled = true;
        cameraPlayer.GetComponent<MouseLook>().enabled = true;
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
        gameObject.GetComponentInChildren<GunController>().enabled = true;
        txtName.GetComponent<TextMeshPro>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;

        if (spinerCameraTransform != null)
        {
            Camera spinerCamera = spinerCameraTransform.GetComponentInChildren<Camera>();
            if (spinerCamera != null)
            {
                spinerCamera.enabled = true; // Bật Camera 
            }
            else
            {
                Debug.LogError("Camera component not found on spinerCamera!");
            }
        }
        else
        {
            Debug.LogError("spinerCamera not found in hierarchy!");
        }
        gameObject.GetComponent<GunManager>().enabled = true;

        //if (PhotonNetwork.LocalPlayer != null)
        //{
        //    actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        //}
        //if (photonView.IsMine) // Chỉ thực hiện trên máy của người chơi cục bộ
        //{
        //    GunShop gunShop = FindObjectOfType<GunShop>(); // Tìm GunShop trong Scene

        //    if (gunShop != null)
        //    {
        //        gunShop.AssignGunPosition(gunPosition); // Gán gunPosition cho GunShop
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Không tìm thấy GunShop trong Scene!");
        //    }
        //}
    }

    public void OnPlayerDeath()
    {
        // Tắt nhân vật
        /*    gameObject.GetComponent<MeshRenderer>().enabled=false;*/
        // Tìm GameObject con theo tên
        Transform child = transform.Find("Ch15_nonPBR");

        if (child != null)
        {   
            // Tắt GameObject con
            child.gameObject.SetActive(false);
            GameObject txtName = gameObject.transform.Find("Nickname/Text (TMP)").gameObject;
            txtName.GetComponent<TextMeshPro>().enabled = false;
            Collider collider = gameObject.GetComponent<Collider>();
            collider.enabled = false;

            //PHAN ROT BOM
            //PlantTheSpike plantTheSpike = playerViewID.GetComponent<PlantTheSpike>();
            //int ViewID = PlayerPrefs.GetInt("ViewIDHasSpike");
            //Debug.LogError(ViewID);
            //GameObject playerViewID = PhotonView.Find(ViewID).gameObject;
            //if (plantTheSpike.isHasSpike)
            //{
            //    PlantTheSpike.instance.DropSpike();
            //    Debug.LogError("dieeeeeeee");
            //    plantTheSpike.isHasSpike = false;
            //}
        }
        else
        {
            Debug.LogError("Không tìm thấy GameObject con!");
        }
    }
    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nickNameText.text = nickname;
    }
    [PunRPC]
    public void RequestDestroyPlayer(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }

}
