    using System;
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
        /*  [Header("HieuUng Thay Dan")]
          public Animation animation;
          public AnimationClip reload;*/

        [Header("Do Giat")]
    /*    [Range(0f, 1f)]
        public float recoilPercent = 0.3f;*/
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
                nextFire -=Time.deltaTime;

            }
            if(Input.GetButton("Fire1") && nextFire <= 0 && ammo >0 /*&& animation.isPlaying == false*/)
            {
                Debug.Log("ban ne...");
                nextFire = 1 / fireRate;
                ammo--;
                magText.text = mag.ToString();
                ammoText.text = ammo + "/" + magAmmo;
                Fire();

            }
            if (Input.GetKeyDown(KeyCode.R)&& mag > 0)
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
           /* animation.Play(reload.name);*/
            if(mag > 0)
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
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hit;
            PhotonNetwork.LocalPlayer.AddScore(0);
            if (Physics.Raycast(ray.origin, ray.direction,out hit, doxa))
            {
                PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
                if (hit.transform.gameObject.GetComponent<health>())
                {
                    PhotonNetwork.LocalPlayer.AddScore(damege);
                    if(damege >= hit.transform.gameObject.GetComponent<health>().healths)
                    {
                        RoomManager.instance.kills++;
                        RoomManager.instance.SetHashes();
                        PhotonNetwork.LocalPlayer.AddScore(100);
                    }
                    Debug.Log("ban trung roi ne");
                    hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamege",RpcTarget.All,damege);
                }
            }
        }
        void Recoil()
        {
            Vector3 finalPositon = new Vector3(origianlPosition.x, origianlPosition.y + recoilUp, origianlPosition.z - recoilBack);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPositon, ref recoilVeclocity, recoilLength);

            if(transform.localPosition == finalPositon)
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
