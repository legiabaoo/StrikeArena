using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{
    public int damege;
    public float fireRate;
    public Camera camera;
    private float nextFire;

    [Header("Hieu Ung")]
    public GameObject hitVFX;
    [Header("So Dan")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;
    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Do Giat")]
    [Range(0f, 2f)]
    public float recoverPercent = 0.7f;
    [Space]
    public float recoilUp = 1f;
    public float recoilBack = 0f;

    private Vector3 origianlPosition;
    private Vector3 recoilVeclocity = Vector3.zero;

    private bool recoiling;
    public bool recovering;
    private float recoilLength;
    private float recoverLength;
    public float doxa = 0f;

    // Start is called before the first frame update
    void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;

        origianlPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0)
        {
            Debug.Log("ban ne...");
            nextFire = 1 / fireRate;
            ammo--;
            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && mag > 0)
        {
            Reload();
        }

        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }
    }

    void Reload()
    {
        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }

    private void Fire()
    {
        recoiling = true;
        recovering = false;

        // L?y PhotonView c?a ng??i b?n (shooter)
        PhotonView shooterPhotonView = GetComponentInParent<PhotonView>();
        if (shooterPhotonView == null || !shooterPhotonView.Owner.CustomProperties.ContainsKey("team"))
        {
            Debug.LogError("PhotonView ho?c CustomProperties không h?p l?. H?y b?n.");
            return;
        }

        // L?y team c?a ng??i b?n
        int shooterTeam = (int)shooterPhotonView.Owner.CustomProperties["team"];

        // T?o ray t? camera
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
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
                    Debug.LogError("PhotonView ho?c CustomProperties c?a m?c tiêu không h?p l?.");
                    return;
                }

                // L?y team c?a ??i t??ng b? b?n
                int targetTeam = (int)targetPhotonView.Owner.CustomProperties["team"];

                // Ki?m tra n?u khác team m?i cho phép gây sát th??ng
                if (shooterTeam != targetTeam)
                {
                    // Gây sát th??ng cho m?c tiêu
                    PhotonNetwork.LocalPlayer.AddScore(damege);
                    if (damege >= targetHealth.healths)
                    {
                        RoomManager.instance.kills++;
                        RoomManager.instance.SetHashes();
                        PhotonNetwork.LocalPlayer.AddScore(100);
                    }

                    // G?i RPC ?? c?p nh?t sát th??ng lên t?t c? các client
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, damege);
                }
                else
                {
                    Debug.Log("Không th? b?n ??ng ??i.");
                }
            }
            else
            {
                Debug.LogError("??i t??ng không có component health.");
            }
        }
    }

    void Recoil()
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
    }
}
