using DevionGames;
using Photon.Pun;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public AudioClip amThanhDatBom;   // Âm thanh khi đặt bom
    public AudioClip amThanhGoBom;    // Âm thanh khi gỡ bom
    public float amThanhVolume = 1.0f; // Âm lượng âm thanh

    private AudioSource audioSource;  // AudioSource để quản lý âm thanh

    private void Awake()
    {
        // Thêm AudioSource vào đối tượng nếu chưa có
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = amThanhVolume;
        audioSource.loop = false; // Đảm bảo âm thanh không lặp lại
    }

    // Đặt tag Spike và phát âm thanh đặt bom
    [PunRPC]
    public void SetSpikeTag(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);

        if (spikeView != null)
        {
            GameObject spike = spikeView.gameObject;
            spike.tag = "Spike"; // Đặt tag cho Spike trên tất cả các máy

            // Phát âm thanh khi đặt bom
            if (amThanhDatBom != null)
            {
                if (audioSource.isPlaying) audioSource.Stop(); // Dừng âm thanh hiện tại nếu đang phát
                audioSource.clip = amThanhDatBom;
                audioSource.Play();
                Debug.Log("Playing set bomb sound on all clients");
            }

            Debug.Log("Tag set successfully on all clients");
        }
        else
        {
            Debug.LogError("Spike object not found with the given ViewID: " + spikeViewID);
        }
    }

    // Gỡ Spike và phát âm thanh gỡ bom
    [PunRPC]
    private void RemoveSpike(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);

        if (spikeView != null)
        {
            GameObject spike = spikeView.gameObject;

            // Dừng âm thanh đặt bom nếu nó đang phát
            if (audioSource.isPlaying && audioSource.clip == amThanhDatBom)
            {
                audioSource.Stop();
                Debug.Log("Stopped set bomb sound");
            }

            // Phát âm thanh khi gỡ bom
            if (amThanhGoBom != null)
            {
                audioSource.clip = amThanhGoBom;
                audioSource.Play();
                Debug.Log("Playing disarm bomb sound on all clients");

                // Trì hoãn việc hủy spike cho đến khi âm thanh gỡ bom phát xong
                Destroy(spike, amThanhGoBom.length);
            }
            else
            {
                // Hủy spike ngay nếu không có âm thanh gỡ bom
                Destroy(spike);
                Debug.Log("Spike removed successfully on all clients");
            }
        }
        else
        {
            Debug.LogError("Spike object not found with the given ViewID: " + spikeViewID);
        }
    }
}
