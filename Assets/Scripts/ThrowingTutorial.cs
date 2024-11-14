using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ThrowingTutorial : MonoBehaviour
{
    public static ThrowingTutorial Instance;
    [Header("References")]
    public Transform cam; // Camera của người chơi
    public Transform attackPoint; // Điểm ném
    public GameObject bom; // Đối tượng để ném (bom hoặc vật phẩm khác)
    public GameObject smoke;

    [Header("Settings")]
    public int totalBom; // Tổng số lần ném
    public int totalSmoke;
    public float throwCooldown; // Thời gian chờ giữa các lần ném

    [Header("Throw Settings")]
    public KeyCode throwKey = KeyCode.Mouse0; // Phím để ném
    public float throwForce; // Lực ném về phía trước
    public float throwUpwardForce; // Lực ném hướng lên

    private bool readyToThrow;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        readyToThrow = true;
        totalBom = 0;
        totalSmoke = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow )
        {
            if (totalBom>0 && WeaponSwitcher.instance.selectedWeapon==3)
            {
                ThrowBom();
            }
            if(totalSmoke > 0 && WeaponSwitcher.instance.selectedWeapon == 4)
            {
                ThrowSmoke();
            }
        }
    }

    private void ThrowBom()
    {
        readyToThrow = false;

        // Khởi tạo đối tượng để ném
        GameObject projectile = PhotonNetwork.Instantiate(bom.name, attackPoint.position, cam.rotation);

        // Lấy thành phần Rigidbody của đối tượng ném
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        // Tính toán hướng ném
        Vector3 forceDirection = cam.transform.forward;

        // Kiểm tra va chạm với tia raycast
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // Tính toán lực ném kết hợp với lực đẩy lên
        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        // Áp dụng lực vào đối tượng ném
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        // Giảm số lần ném
        totalBom--;

        // Đặt thời gian chờ cho lần ném tiếp theo
        Invoke(nameof(ResetThrow), throwCooldown);
    }
    private void ThrowSmoke()
    {
        readyToThrow = false;

        // Khởi tạo đối tượng để ném
        GameObject projectile = PhotonNetwork.Instantiate(smoke.name, attackPoint.position, cam.rotation);

        // Lấy thành phần Rigidbody của đối tượng ném
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        // Tính toán hướng ném
        Vector3 forceDirection = cam.transform.forward;

        // Kiểm tra va chạm với tia raycast
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // Tính toán lực ném kết hợp với lực đẩy lên
        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        // Áp dụng lực vào đối tượng ném
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        // Giảm số lần ném
        totalSmoke--;

        // Đặt thời gian chờ cho lần ném tiếp theo
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}
