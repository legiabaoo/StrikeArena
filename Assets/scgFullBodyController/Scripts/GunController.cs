﻿using System.Collections;
using UnityEngine;
using Photon.Pun;
namespace scgFullBodyController
{
    [RequireComponent(typeof(AudioSource))]
    public class GunController : MonoBehaviour
    {
        Weapon weapon;
        private PhotonView photonView;
        [HideInInspector] public bool reloading = false;
        [HideInInspector] public bool firing = false;
        [HideInInspector] public bool recoilAuto = false;
        [HideInInspector] public bool recoilSemi = false;
        [HideInInspector] public bool aiming = false;
        [HideInInspector] public bool throwing = false;
        [HideInInspector] public bool cycling = false;

        public enum WeaponTypes { Rifle, Pistol };
        public enum ShootTypes { SemiAuto, FullAuto, BoltAction };
       
        public WeaponTypes Weapon;

        [Header("ShootType")]
        public ShootTypes shootType;

        [Header("Animations")]
        public Animator anim;
        public Animator camAnim;
        public float rifleLayerWeight;
        public float pistolLayerWeight;
        public float rifleLayerWeightReloading;
        public float pistolLayerWeightReloading;

        [Header("Camera")]
        public GameObject mainCam;
        Vector3 originalCamPos;

        [Header("Shooting")]
        public int bulletsPerMag;
        public int totalBullets;
        public ParticleSystem[] muzzleFlashes;
        public GameObject shootPoint;
        public GameObject shootPointCamera;
        public bool shootFromCamera;
        public GameObject ejectionPoint;
        public GameObject magDropPoint;
        public GameObject Bullet;
        public GameObject Shell;
        public GameObject Mag;
        public float bulletVelocity;
        public float bulletDespawnTime;
        public float shellVelocity;
        public float magVelocity;
        public float shellDespawnTime;
        public float magDespawnTime;
        public float cycleTimeBoltAction;
        public float cycleTimeSemiAuto;
        public int bulletsInMag;

        [Header("Timing")]
        public float reloadTime;
        public float grenadeTime;
        public float fireRate;

        [Header("Damage")]
        public int Damage;

        [Header("Aiming")]
        public Vector3 aimPosition;
        public Vector3 holdingHandOffsetRot;
        public Transform mainHandTransform;
        Vector3 originalAimPos;
        Vector3 originalAimRot;
        Vector3 originalAimOffsetCamPos;
        Vector3 originalAimOffsetRot;
        public float aimTime;
        public float zoomInAmount;
        float originalCamFov;
        float originalCamClipPlane;
        public float aimInOutDuration;
        float aimTimeElapsed;
        [HideInInspector] public bool aimFinished = true;
        [HideInInspector] public bool swapping;
        [HideInInspector] public bool aimPosSet;

        [Header("Sounds")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        Coroutine lastRoutine = null;
        private GameObject playerObject;
        void OnEnable()
        {
            if (Weapon == WeaponTypes.Rifle)
            {
                anim.SetLayerWeight(1, 1);
                anim.SetLayerWeight(2, 0);
                anim.SetLayerWeight(4, rifleLayerWeight);
                anim.SetLayerWeight(5, 0);
            }
            else if (Weapon == WeaponTypes.Pistol)
            {
                anim.SetLayerWeight(1, 0);
                anim.SetLayerWeight(2, 1);
                anim.SetLayerWeight(4, 0);
                anim.SetLayerWeight(5, pistolLayerWeight);
            }

            //Reset adjuster to sync up every time gun is loaded
            //gameObject.GetComponent<Adjuster>().enabled = false;
            //gameObject.GetComponent<Adjuster>().enabled = true;

            originalCamFov = mainCam.GetComponent<Camera>().fieldOfView;
            originalCamClipPlane = mainCam.GetComponent<Camera>().nearClipPlane;
        }

        void Start()
        {
            photonView = GetComponent<PhotonView>();
            //Set the ammo count
            bulletsInMag = bulletsPerMag;
            originalCamPos = mainCam.transform.localPosition;
            playerObject = FindLocalPlayer();

            if (playerObject != null)
            {
                // Gán Animator cho nhân vật
                anim = playerObject.GetComponent<Animator>();

                GameObject mainCameraObject = GameObject.Find("Main Camera");

                if (mainCameraObject != null)
                {
                    camAnim = mainCameraObject.GetComponent<Animator>();
                }
                else
                {
                    Debug.LogError("Không tìm thấy Main Camera.");
                }


                if (anim == null)
                {
                    Debug.LogError("Không tìm thấy Animator trên nhân vật.");
                }

                if (camAnim == null)
                {
                    Debug.LogError("Không tìm thấy Animator trên camera.");
                }
            }
            else
            {
                Debug.LogError("Không tìm thấy PlayerObject.");
            }
        }
        GameObject FindLocalPlayer()
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    return player;
                }
            }
            //foreach (var player in PhotonNetwork.PlayerList)
            //{
            //    if (player.CustomProperties.TryGetValue("viewID", out var viewIDValue) &&
            //        viewIDValue is int viewID)
            //    {
            //        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;
            //        if (playerObject != null)
            //        {
            //            return playerObject;
            //        }
            //        else
            //        {
            //            Debug.LogError("player null");
            //        }
            //    }
            //}
            return null;
            /* foreach (var player in PhotonNetwork.PlayerList)
             {
                 if (player.CustomProperties.TryGetValue("viewID", out var viewIDValue) &&
                     viewIDValue is int viewID)
                 {
                     GameObject playerObject = PhotonView.Find(viewID)?.gameObject;
                     if (playerObject != null)
                     {
                         return playerObject;
                     }
                     else
                     {
                         Debug.LogError("player null");
                     }
                 }
             }
             return null;*/
        }
        public void initiliazeOrigPositions()
        {
            originalAimPos = mainHandTransform.localPosition;
            originalAimRot = mainHandTransform.localEulerAngles;
        }

        void Update()
        {

            if (!photonView.IsMine) return;
            //Input and actions for shooting
            if (Input.GetButtonDown("Fire1") && !GunShop.instance.isShopGun && !firing && reloading == false && bulletsInMag > 0 && !cycling && !swapping)
            {

                firing = true;
                foreach (ParticleSystem ps in muzzleFlashes)
                {
                    ps.Play(); // Hiệu ứng cho local player
                }

                // Gửi thông báo tới các client khác để phát hiệu ứng
                GetComponent<PhotonView>().RPC("RPC_PlayMuzzleFlash", RpcTarget.Others);
                gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
                /*  spawnBullet();
                  weapon.Fire(Damage);*/
                bulletsInMag--;

                if (shootType == ShootTypes.FullAuto)
                {
                    spawnShell();
                    recoilAuto = true;
                    recoilSemi = false;
                    lastRoutine = StartCoroutine(shootBullet());
                }
                else if (shootType == ShootTypes.SemiAuto)
                {
                    spawnShell();
                    recoilAuto = false;
                    recoilSemi = true;

                    if (Weapon == WeaponTypes.Rifle)
                    {
                        Invoke("fireCancel", .25f);
                    }
                    Invoke("cycleFire", cycleTimeSemiAuto);
                    cycling = true;
                }
                else if (shootType == ShootTypes.BoltAction)
                {
                    recoilAuto = false;
                    recoilSemi = true;

                    if (Weapon == WeaponTypes.Rifle)
                    {
                        Invoke("fireCancel", .25f);
                        Invoke("cycleFire", cycleTimeBoltAction);
                        Invoke("ejectShellBoltAction", cycleTimeBoltAction / 2);
                        cycling = true;
                        gameObject.GetComponent<Animator>().SetBool("cycle", true);
                    }
                }

            }
            else if (Input.GetButtonUp("Fire1") || bulletsInMag == 0)
            {
                firing = false;
                recoilSemi = false;
                recoilAuto = false;
                if (shootType == ShootTypes.FullAuto)
                    StopCoroutine(lastRoutine);
            }

            if (Input.GetButtonDown("Grenade"))
            {
                anim.SetTrigger("Grenade");
                throwing = true;
                Invoke("throwingCancel", grenadeTime);
            }

            if (Input.GetButtonDown("Reload") && !reloading && !firing && bulletsInMag < bulletsPerMag && totalBullets > 0)
            {
                anim.SetBool("reload", true);
                reloading = true;
                gameObject.GetComponent<AudioSource>().PlayOneShot(reloadSound);
                Invoke("reloadFinished", reloadTime);
                spawnMag();

                if (Weapon == WeaponTypes.Rifle)
                {
                    gameObject.GetComponent<Animator>().SetBool("reloading", true);
                    anim.SetLayerWeight(4, rifleLayerWeightReloading);
                }
                else
                {
                    anim.SetLayerWeight(5, pistolLayerWeightReloading);
                }
            }

            //UI
            GameObject ui = GameObject.FindGameObjectWithTag("hud");
            ui.GetComponent<hudController>().uiBullets.text = bulletsInMag.ToString() + "/" + totalBullets;

            //Anims
            anim.SetBool("Fire", firing);
            camAnim.SetBool("recoilAuto", recoilAuto);
            camAnim.SetBool("recoilSemi", recoilSemi);
        }
        [PunRPC]
        void RPC_PlayMuzzleFlash()
        {
            foreach (ParticleSystem ps in muzzleFlashes)
            {
                ps.Play(); // Phát hiệu ứng trên các client khác
            }
        }

        void cycleFire()
        {
            cycling = false;

            if (shootType == ShootTypes.BoltAction)
                gameObject.GetComponent<Animator>().SetBool("cycle", false);
        }

        void ejectShellBoltAction()
        {
            spawnShell();
        }

        void fireCancel()
        {
            firing = false;
        }

        void LateUpdate()
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView == null || !photonView.IsMine)
            {
                return;
            }
            if (Input.GetButtonDown("Fire2") && aimFinished && !swapping)
            {
                originalAimOffsetRot = holdingHandOffsetRot;
                originalAimOffsetCamPos = aimPosition;
                aimPosition += originalCamPos;
                holdingHandOffsetRot += originalAimRot;
                anim.SetBool("aimed", true);
                anim.SetFloat("idleAnimSpeed", 0);
                aimTimeElapsed = 0;
                aiming = true;
                aimFinished = false;

                //Disable crosshair
                GameObject ui = GameObject.FindGameObjectWithTag("hud");
                ui.GetComponent<hudController>().crosshair.SetActive(false);
            }
            else if (Input.GetButtonUp("Fire2") && aiming && !swapping)
            {
                aiming = false;
                aimTimeElapsed = 0;
                Invoke("aimingOutFinished", aimInOutDuration);
                anim.SetBool("aimed", false);
                anim.SetFloat("idleAnimSpeed", 1);
                mainCam.GetComponent<Camera>().nearClipPlane = originalCamClipPlane;

                //Disable crosshair
                GameObject ui = GameObject.FindGameObjectWithTag("hud");
                ui.GetComponent<hudController>().crosshair.SetActive(true);
            }

            if (aiming && !aimFinished)
            {
                LerpAimIn();
            }
            else if (!aiming && !aimFinished)
            {
                LerpAimOut();
            }
        }

        void aimingOutFinished()
        {
            mainCam.GetComponent<Camera>().fieldOfView = originalCamFov;
            mainCam.transform.localPosition = originalCamPos;
            mainHandTransform.localPosition = originalAimPos;
            mainHandTransform.localEulerAngles = originalAimRot;
            aimPosition = originalAimOffsetCamPos;
            holdingHandOffsetRot = originalAimOffsetRot;
            aimFinished = true;
        }

        void LerpAimIn()
        {
            if (aimTimeElapsed < aimInOutDuration)
            {
                mainCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(originalCamFov, zoomInAmount, aimTimeElapsed / aimInOutDuration);
                mainCam.transform.localPosition = Vector3.Lerp(originalCamPos, aimPosition, aimTimeElapsed / aimInOutDuration);
                mainHandTransform.localEulerAngles = Vector3.Lerp(originalAimRot, holdingHandOffsetRot, aimTimeElapsed / aimInOutDuration);
                aimTimeElapsed += Time.deltaTime;
            }
            else
            {
                mainCam.GetComponent<Camera>().nearClipPlane = 0.01f;
                mainCam.GetComponent<Camera>().fieldOfView = zoomInAmount;
                mainCam.transform.localPosition = new Vector3(aimPosition.x, aimPosition.y, aimPosition.z);
                mainHandTransform.localEulerAngles = holdingHandOffsetRot;
            }
        }
        void LerpAimOut()
        {
            if (aimTimeElapsed < aimInOutDuration)
            {
                mainCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(zoomInAmount, originalCamFov, aimTimeElapsed / aimInOutDuration);
                mainCam.transform.localPosition = Vector3.Lerp(aimPosition, originalCamPos, aimTimeElapsed / aimInOutDuration);
                mainHandTransform.localEulerAngles = Vector3.Lerp(holdingHandOffsetRot, originalAimRot, aimTimeElapsed / aimInOutDuration);
                aimTimeElapsed += Time.deltaTime;
            }
            else
            {
                mainCam.GetComponent<Camera>().fieldOfView = originalCamFov;
                mainCam.transform.localPosition = originalCamPos;
                mainHandTransform.localPosition = originalAimPos;
                mainHandTransform.localEulerAngles = originalAimRot;
            }
        }

        IEnumerator shootBullet()
        {
            while (true)
            {
                yield return new WaitForSeconds(fireRate);
                if (bulletsInMag > 0)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
                    foreach (ParticleSystem ps in muzzleFlashes)
                    {
                        ps.Play(); // Hiệu ứng cho local player
                    }

                    // Gửi thông báo tới các client khác để phát hiệu ứng
                    GetComponent<PhotonView>().RPC("RPC_PlayMuzzleFlash", RpcTarget.Others);
                    /*    spawnBullet();
                        weapon.Fire(Damage);*/
                    spawnShell();
                    bulletsInMag--;
                }
            }
        }

        void reloadFinished()
        {
            reloading = false;
            anim.SetBool("reload", false);
            int bulletsToRemove = (bulletsPerMag - bulletsInMag);
            if (totalBullets >= bulletsPerMag)
            {
                bulletsInMag = bulletsPerMag;
                totalBullets -= bulletsToRemove;
            }
            else if (bulletsToRemove <= totalBullets)
            {
                bulletsInMag += bulletsToRemove;
                totalBullets -= bulletsToRemove;
            }
            else
            {
                bulletsInMag += totalBullets;
                totalBullets -= totalBullets;
            }

            if (Weapon == WeaponTypes.Rifle)
            {
                gameObject.GetComponent<Animator>().SetBool("reloading", false);
                anim.SetLayerWeight(4, rifleLayerWeight);
            }
            else
            {
                anim.SetLayerWeight(5, pistolLayerWeight);
            }
        }

        void throwingCancel()
        {
            throwing = false;
        }

        void spawnBullet()
        {
            GameObject tempBullet;
            if (shootFromCamera)
            {
                //Spawn bullet from the camera shoot point position, not from the true tip of the gun
                tempBullet = Instantiate(Bullet, shootPointCamera.transform.position, shootPointCamera.transform.rotation) as GameObject;
                tempBullet.GetComponent<registerHit>().damage = Damage;
            }
            else
            {
                //Spawn bullet from the shoot point position, the true tip of the gun
                tempBullet = Instantiate(Bullet, shootPoint.transform.position, shootPoint.transform.rotation) as GameObject;
                tempBullet.GetComponent<registerHit>().damage = Damage;
            }

            //Orient it
            tempBullet.transform.Rotate(Vector3.left * 90);

            //Add forward force based on where camera is pointing
            Rigidbody tempRigidBody;
            tempRigidBody = tempBullet.GetComponent<Rigidbody>();

            //Always shoot towards where camera is facing
            tempRigidBody.AddForce(mainCam.transform.forward * bulletVelocity);

            //Destroy after time
            Destroy(tempBullet, bulletDespawnTime);
        }

        void spawnShell()
        {
            //Spawn bullet
            GameObject tempShell;
            tempShell = Instantiate(Shell, ejectionPoint.transform.position, ejectionPoint.transform.rotation) as GameObject;

            //Orient it
            tempShell.transform.Rotate(Vector3.left * 90);

            //Add forward force based on where ejection point is pointing (blue axis)
            Rigidbody tempRigidBody;
            tempRigidBody = tempShell.GetComponent<Rigidbody>();
            tempRigidBody.AddForce(ejectionPoint.transform.forward * shellVelocity);

            //Destroy after time
            Destroy(tempShell, shellDespawnTime);
        }

        void spawnMag()
        {
            //Spawn bullet
            GameObject tempMag;
            tempMag = Instantiate(Mag, magDropPoint.transform.position, magDropPoint.transform.rotation) as GameObject;

            //Orient it
            tempMag.transform.Rotate(Vector3.left * 90);

            //Add forward force based on where ejection point is pointing (blue axis)
            Rigidbody tempRigidBody;
            tempRigidBody = tempMag.GetComponent<Rigidbody>();
            tempRigidBody.AddForce(magDropPoint.transform.forward * magVelocity);

            //Destroy after time
            Destroy(tempMag, magDespawnTime);
        }
    }
}
