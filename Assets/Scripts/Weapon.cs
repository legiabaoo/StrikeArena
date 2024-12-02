using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using scgFullBodyController;

public class Weapon : MonoBehaviour
{
    public int damege;
    public float fireRate;
    public Camera camera;
    private float nextFire;
    public GameObject raycat;
    [Header("Hieu Ung")]
    public GameObject hitVFX;
    [Header("So Dan")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;
    [Header("UI")]
  
    public TextMeshProUGUI ammoText;

   /* [Header("Do Giat")]
    [Range(0f, 2f)]
    public float recoverPercent = 0.7f;
    [Space]*/
   /* public float recoilUp = 1f;
    public float recoilBack = 0f;*/

    private Vector3 origianlPosition;
    private Vector3 recoilVeclocity = Vector3.zero;

 /*   private bool recoiling;*/
   /* public bool recovering;*/
  /*  private float recoilLength;
    private float recoverLength;*/
    public float doxa = 0f;
    public GunController gun;
    public 
    // Start is called before the first frame update
    void Start()
    {
     

    

       
    }

    // Update is called once per frame
    void Update()
    {
        PhotonView photonView = GetComponentInParent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }

        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }
        GunController.ShootTypes shootType = gun.shootType;


        if (shootType == GunController.ShootTypes.SemiAuto  )
        {
            if (Input.GetButtonDown("Fire1") && nextFire <= 0 && gun.bulletsInMag > 0)
            {
                Debug.Log("sung luc va ngam");
                nextFire = 1 / fireRate;
                /*  ammo--;

                  ammoText.text = ammo + "/" + magAmmo;*/
                Fire();
            }
        }
        if (shootType == GunController.ShootTypes.FullAuto)
        {
            if (Input.GetButton("Fire1") && nextFire <= 0 && gun.bulletsInMag > 0)
            {
                Debug.Log("sungtruongdangban");
                nextFire = 1 / fireRate;
                /*  ammo--;

                  ammoText.text = ammo + "/" + magAmmo;*/
                Fire();
            }
        }
        if (shootType == GunController.ShootTypes.BoltAction)
        {
            if (Input.GetButtonDown("Fire1") && nextFire <= 0 && gun.bulletsInMag > 0)
            {
                Debug.Log("sung luc va ngam");
                nextFire = 1 / fireRate;
                /*  ammo--;

                  ammoText.text = ammo + "/" + magAmmo;*/
                Fire();
            }
        }





        /* if (Input.GetKeyDown(KeyCode.R) && mag > 0)
         {
             Reload();
         }*/

        /*  if (recoiling)
          {
              Recoil();
          }

          if (recovering)
          {
              Recovering();
          }*/
    }

/*    void Reload()
    {
        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }
        
        ammoText.text = ammo + "/" + magAmmo;
    }*/

    public void Fire()
    {
 /*       recoiling = true;
        recovering = false;*/

        // L?y PhotonView c?a ng??i b?n (shooter)
        PhotonView shooterPhotonView = GetComponentInParent<PhotonView>();
        if (shooterPhotonView == null || !shooterPhotonView.Owner.CustomProperties.ContainsKey("team"))
        {
            Debug.LogError("PhotonView ho?c CustomProperties kh�ng h?p l?. H?y b?n.");
            return;
        }

        // L?y team c?a ng??i b?n
        int shooterTeam = (int)shooterPhotonView.Owner.CustomProperties["team"];

        // T?o ray t? camera
        Ray ray = new Ray(raycat.transform.position, raycat.transform.forward);
        RaycastHit hit;

        // Ki?m tra va ch?m v?i raycast
        if (Physics.Raycast(ray.origin, ray.direction, out hit, doxa))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            // L?y component "health" c?a ??i t??ng b? b?n
            health targetHealth = hit.transform.gameObject.GetComponent<health>();

            if (targetHealth != null)
            {
                // L?y PhotonView c?a ??i t??ng b? b?n
                PhotonView targetPhotonView = hit.transform.gameObject.GetComponent<PhotonView>();
                if (targetPhotonView == null || !targetPhotonView.Owner.CustomProperties.ContainsKey("team"))
                {
                    Debug.LogError("PhotonView ho?c CustomProperties c?a m?c ti�u kh�ng h?p l?.");
                    return;
                }

                // L?y team c?a ??i t??ng b? b?n
                int targetTeam = (int)targetPhotonView.Owner.CustomProperties["team"];

                // Ki?m tra n?u kh�c team m?i cho ph�p g�y s�t th??ng
                if (shooterTeam != targetTeam)
                {
                  int finalDamage = damege; // Gi� tr? s�t th??ng m?c ??nh
                    if (hit.collider.CompareTag("head")) // Ki?m tra tag "head"
                    {
                        finalDamage *= 3; // Nh�n s�t th??ng l�n 4 l?n
                        Debug.Log("trung dau n�....");
                    }

                    // G�y s�t th??ng cho m?c ti�u
                    PhotonNetwork.LocalPlayer.AddScore(finalDamage);
                    if (damege >= targetHealth.healths)
                    {
                        RoomManager.instance.kills++;
                        RoomManager.instance.SetHashes();
                        PhotonNetwork.LocalPlayer.AddScore(100);
                    }
                    Debug.LogWarning(finalDamage);
                    // G?i RPC ?? c?p nh?t s�t th??ng l�n t?t c? c�c client
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, finalDamage);
                }
                else
                {
                    Debug.Log("Kh�ng th? b?n ??ng ??i.");
                }
            }
            else
            {
                Debug.Log(" kh�ng c� component health.");
            }
        }
    }

   /* void Recoil()
    {
        Vector3 finalPositon = new Vector3(origianlPosition.x, origianlPosition.y + recoilUp, origianlPosition.z - recoilBack);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPositon, ref recoilVeclocity, recoilLength);

        if (transform.localPosition == finalPositon)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPositon = origianlPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPositon, ref recoilVeclocity, recoverLength);

        if (transform.localPosition == finalPositon)
        {
            recoiling = false;
            recovering = false;
        }
    }*/
}
