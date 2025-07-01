using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ThrowingTutorial : MonoBehaviourPunCallbacks
{
    public static ThrowingTutorial Instance;

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject bom;
    public GameObject smoke;
    public AudioClip explosionSound; // Âm thanh nổ
    public AudioClip smokeSound;     // Âm thanh bom khói

    [Header("Settings")]
    public int totalBom;
    public int totalSmoke;
    public float throwCooldown;

    [Header("Throw Settings")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

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
        if (Input.GetKeyDown(throwKey) && readyToThrow)
        {
            if (totalBom > 0 && WeaponSwitcher.instance.selectedWeapon == 3)
            {
                ThrowBom();
            }
            if (totalSmoke > 0 && WeaponSwitcher.instance.selectedWeapon == 4)
            {
                ThrowSmoke();
            }
        }
    }

    private void ThrowBom()
    {
        readyToThrow = false;

        GameObject projectile = PhotonNetwork.Instantiate(bom.name, attackPoint.position, cam.rotation);

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalBom--;

        Invoke(nameof(ResetThrow), throwCooldown);

        // Gắn lớp xử lý âm thanh nổ bom
        BombExplosion explosion = projectile.AddComponent<BombExplosion>();
        explosion.explosionSound = explosionSound;
    }

    private void ThrowSmoke()
    {
        readyToThrow = false;

        GameObject projectile = PhotonNetwork.Instantiate(smoke.name, attackPoint.position, cam.rotation);

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalSmoke--;

        Invoke(nameof(ResetThrow), throwCooldown);

        // Gắn lớp xử lý âm thanh bom khói
        SmokeEffect smokeEffect = projectile.AddComponent<SmokeEffect>();
        smokeEffect.smokeSound = smokeSound;
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}

// Lớp BombExplosion để quản lý âm thanh nổ bom
public class BombExplosion : MonoBehaviour
{
    public AudioClip explosionSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = explosionSound;
        audioSource.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Gọi RPC để phát âm thanh đồng bộ
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.RPC("PlayExplosionSound", RpcTarget.All);
        }

        // Hủy đối tượng sau khi phát âm thanh xong
        Destroy(gameObject, explosionSound.length);
    }

    [PunRPC]
    private void PlayExplosionSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}

// Lớp SmokeEffect để quản lý âm thanh bom khói
public class SmokeEffect : MonoBehaviour
{
    public AudioClip smokeSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = smokeSound;
        audioSource.playOnAwake = false;

        // Gọi RPC để phát âm thanh khói đồng bộ
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.RPC("PlaySmokeSound", RpcTarget.All);
        }

        // Tự hủy đối tượng sau thời gian khói tan (ví dụ: 5 giây)
        Destroy(gameObject, 5f);
    }

    [PunRPC]
    private void PlaySmokeSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
