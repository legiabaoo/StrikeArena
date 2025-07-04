﻿//SlapChickenGames
//2021
//Manager for weapon inventory and switching

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace scgFullBodyController
{
    public class GunManager : MonoBehaviourPunCallbacks
    {
        public static GunManager instance;
        public GameObject[] weapons;
        public Animator anim;
        public OffsetRotation oRot;
        public float swapTime;
        public int index = 0;
        private void Awake()
        {
            instance = this;
        }
        void Start()
        {

            //Initialize each weapon and set state to swapping automatically so gun controller knows to setup weapon positions
            foreach (GameObject weapon in weapons)
            {
                weapon.GetComponent<GunController>().swapping = true;
            }
            /*
            The invoke timing here is based off the time it takes for the swap animation to complete + transition time,
            this is so that the weapon aiming position is based off where its first position is out of the swap anim
            */
            Invoke("setSwappedWeaponPositions", .567f + .25f);
            index = 0;
            Invoke("swapWeapons", swapTime);
            foreach (GameObject weapon in weapons)
            {
                weapon.GetComponent<GunController>().swapping = true;
            }
            anim.SetBool("putaway", true);
        }


        //[PunRPC]
        //public void AddWeaponToInventory(GameObject newWeapon)
        //{
        //    if (weapons == null)
        //    {
        //        // Nếu mảng chưa khởi tạo, khởi tạo mảng với vũ khí đầu tiên
        //        weapons = new GameObject[] { newWeapon };
        //    }
        //    else
        //    {
        //        // Chuyển mảng sang List để dễ thêm vũ khí
        //        List<GameObject> weaponList = new List<GameObject>(weapons);

        //        if (newWeapon.name.ToString() == "M500(Clone)")
        //        {
        //            Debug.LogError("Vi tri 1");
        //            // Xóa vũ khí ở vị trí 0 nếu danh sách không rỗng
        //            if (weaponList.Count > 0)
        //            {
        //                Debug.LogError("Xoa 1");
        //                weaponList.RemoveAt(0);
        //            }
        //            Invoke("swapWeapons", swapTime);
        //            // Thêm M500 vào vị trí đầu tiên
        //            weaponList.Insert(0, newWeapon);
        //        }
        //        else if (newWeapon.name.ToString() == "AK47 1(Clone)" || newWeapon.name.ToString() == "M4A1(Clone)" || newWeapon.name.ToString() == "Sniper(Clone)")
        //        {
        //            Debug.LogError("Vi tri 2");
        //            // Xóa vũ khí ở vị trí 0 nếu danh sách không rỗng
        //            if (weaponList.Count > 1)
        //            {
        //                Debug.LogError("Xoa 2");
        //                weaponList.RemoveAt(1);
        //            }
        //            Invoke("swapWeapons", swapTime);
        //            weaponList.Insert(1, newWeapon);
        //        }

        //        // Cập nhật lại mảng
        //        weapons = weaponList.ToArray();
        //    }

        //    Debug.Log($"Đã thêm vũ khí {newWeapon.name} vào inventory tại vị trí {weapons.Length - 1}.");
        //}

        void Update()
        {
            if (!GetComponent<PhotonView>().IsMine)
            {
                return;
            }
            //To add more weapons, just copy one of these blocks of code, add an else if, and change the keybind to the next one up ex., 
            //Aplha4, then set index to the corresponding key value such as 4
            if (Input.GetKeyDown(KeyCode.Alpha1) && index != 0)
            {
                if (!weapons[index].GetComponent<GunController>().firing && !weapons[index].GetComponent<GunController>().swapping
                    && !weapons[index].GetComponent<GunController>().aiming && weapons[index].GetComponent<GunController>().aimFinished
                    && !weapons[index].GetComponent<GunController>().reloading && !weapons[index].GetComponent<GunController>().cycling)
                {
                    index = 0;

                    Invoke("swapWeapons", swapTime);
                    foreach (GameObject weapon in weapons)
                    {
                        weapon.GetComponent<GunController>().swapping = true;
                    }
                    anim.SetBool("putaway", true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && index != 1 && weapons.Length > 1)
            {
                if (!weapons[index].GetComponent<GunController>().firing && !weapons[index].GetComponent<GunController>().swapping
                    && !weapons[index].GetComponent<GunController>().aiming && weapons[index].GetComponent<GunController>().aimFinished
                    && !weapons[index].GetComponent<GunController>().reloading && !weapons[index].GetComponent<GunController>().cycling)
                {
                    {
                        index = 1;
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && index != 2 && weapons.Length > 2)
            {
                if (!weapons[index].GetComponent<GunController>().firing && !weapons[index].GetComponent<GunController>().swapping
                    && !weapons[index].GetComponent<GunController>().aiming && weapons[index].GetComponent<GunController>().aimFinished
                    && !weapons[index].GetComponent<GunController>().reloading && !weapons[index].GetComponent<GunController>().cycling)
                {
                    {
                        index = 2;
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && index != 3 && weapons.Length > 3)
            {
                if (!weapons[index].GetComponent<GunController>().firing && !weapons[index].GetComponent<GunController>().swapping
                    && !weapons[index].GetComponent<GunController>().aiming && weapons[index].GetComponent<GunController>().aimFinished
                    && !weapons[index].GetComponent<GunController>().reloading && !weapons[index].GetComponent<GunController>().cycling)
                {
                    {
                        index = 3;
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && index != 4 && weapons.Length > 4)
            {
                if (!weapons[index].GetComponent<GunController>().firing && !weapons[index].GetComponent<GunController>().swapping
                    && !weapons[index].GetComponent<GunController>().aiming && weapons[index].GetComponent<GunController>().aimFinished
                    && !weapons[index].GetComponent<GunController>().reloading && !weapons[index].GetComponent<GunController>().cycling)
                {
                    {
                        index = 4;
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                    }
                }
            }
        }
        [PunRPC]
        public void activeWeapon(int i)
        {
            if (weapons == null || i < 0 || i >= weapons.Length)
            {
                Debug.LogError($"Chỉ số {i} không hợp lệ. Mảng weapons có {weapons?.Length ?? 0} phần tử.");
                return;
            }
            weapons[i].SetActive(false);
        }
        [PunRPC]
        public void AddWeaponToInventory(int viewID)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null)
            {
                GameObject newWeapon = view.gameObject;

                if (weapons == null)
                {
                    weapons = new GameObject[] { newWeapon };
                }
                else
                {
                    List<GameObject> weaponList = new List<GameObject>(weapons);

                    if (newWeapon.name == "M500(Clone)")
                    {
                        Debug.LogError("Vi tri 1");
                        if (weaponList.Count > 0)
                        {
                            Debug.LogError("Xoa 1");
                            weaponList.RemoveAt(0);
                        }
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                        weaponList.Insert(0, newWeapon);
                    }
                    else if (newWeapon.name == "AK47 1(Clone)" || newWeapon.name == "M4A1(Clone)" || newWeapon.name == "Sniper(Clone)")
                    {
                        Debug.LogError("Vi tri 2");
                        if (weaponList.Count > 1)
                        {
                            Debug.LogError("Xoa 2");
                            weaponList.RemoveAt(1);
                        }
                        Invoke("swapWeapons", swapTime);
                        foreach (GameObject weapon in weapons)
                        {
                            weapon.GetComponent<GunController>().swapping = true;
                        }
                        anim.SetBool("putaway", true);
                        weaponList.Insert(1, newWeapon);
                    }

                    weapons = weaponList.ToArray();
                }

                Debug.Log($"Đã thêm vũ khí {newWeapon.name} vào inventory tại vị trí {weapons.Length - 1}.");
            }
            else
            {
                Debug.LogError($"Không tìm thấy vũ khí với viewID: {viewID}");
            }
        }
        
        void swapWeapons()
        {
            if (!GetComponent<PhotonView>().IsMine) return;
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i != index)
                {
                    photonView.RPC("activeWeapon", RpcTarget.All, i);

                }
            }
            // Set desired weapon to active
            weapons[index].SetActive(true);
            Invoke("setSwappedWeaponPositions", .567f + .25f);

            // Initialize the correct spine rotation on the spine bone's orientation script
            if (weapons[index].GetComponent<GunController>().Weapon == GunController.WeaponTypes.Rifle)
            {
                oRot.rifle = true;
                oRot.pistol = false;
            }
            else if (weapons[index].GetComponent<GunController>().Weapon == GunController.WeaponTypes.Pistol)
            {
                oRot.rifle = false;
                oRot.pistol = true;
            }

            anim.SetBool("putaway", false);

            // Gửi thông tin đổi súng đến các client khác
            GetComponent<PhotonView>().RPC("RPC_SyncWeapon", RpcTarget.Others, index);
        }
        [PunRPC]
        void RPC_SyncWeapon(int weaponIndex)
        {
            index = weaponIndex; // Cập nhật chỉ số súng
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i != index)
                {
                    weapons[i].SetActive(false);
                }
            }

            weapons[index].SetActive(true); // Bật súng được chọn

            // Đồng bộ hóa trạng thái của các spine bone (nếu cần)
            if (weapons[index].GetComponent<GunController>().Weapon == GunController.WeaponTypes.Rifle)
            {
                oRot.rifle = true;
                oRot.pistol = false;
            }
            else if (weapons[index].GetComponent<GunController>().Weapon == GunController.WeaponTypes.Pistol)
            {
                oRot.rifle = false;
                oRot.pistol = true;
            }

            anim.SetBool("putaway", false);
        }

        void setSwappedWeaponPositions()
        {
            //Initialize the correct original aim position if it is the first time swapping
            if (!weapons[index].GetComponent<GunController>().aimPosSet)
            {
                weapons[index].GetComponent<GunController>().initiliazeOrigPositions();
                weapons[index].GetComponent<GunController>().aimPosSet = true;
            }

            foreach (GameObject weapon in weapons)
            {
                weapon.GetComponent<GunController>().swapping = false;
            }
        }
    }
}
