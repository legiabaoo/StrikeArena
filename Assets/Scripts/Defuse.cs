using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Defuse : MonoBehaviour
{
    public float holdTimeRequired = 3.0f; // Thời gian giữ phím cần thiết để gỡ bom
    private float holdTime = 0.0f; // Biến đếm thời gian giữ phím
    private bool isInRange = false; // Biến kiểm tra xem người chơi có đứng gần bom không
    private GameObject currentSpike; // Spike hiện tại để gỡ
    public Slider progressBar; // Thanh loading
    public Image iconPlant; // Icon nhấp nháy khi spike được đặt

    private float blinkInterval = 0.5f; // Thời gian nhấp nháy icon

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.value = 0f;
        }
        else
        {
            Debug.LogError("ProgressBar not found in the player prefab!");
        }
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        //{
        //    bool spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
        //    if (spikeExists)
        //    {
        //        PhotonView.Get(this).RPC("StartBlinking", RpcTarget.All);
        //    }
        //}
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            bool spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (spikeExists)
            {
                PhotonView.Get(this).RPC("StartBlinking", RpcTarget.All);
            }
        }
        if (isInRange && currentSpike != null)
        {
            if (Input.GetKey(KeyCode.Alpha4))
            {
                progressBar.gameObject.SetActive(true);
                holdTime += Time.deltaTime;
                progressBar.value = holdTime / holdTimeRequired;

                if (holdTime >= holdTimeRequired)
                {
                    DefuseSpike();
                    progressBar.gameObject.SetActive(false);
                }
            }
            else
            {
                holdTime = 0.0f;
                progressBar.value = 0f;
                progressBar.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spike"))
        {
            isInRange = true;
            currentSpike = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spike"))
        {
            isInRange = false;
            currentSpike = null;
        }
    }

    private void DefuseSpike()
    {
        PhotonView spikePhotonView = currentSpike.GetComponent<PhotonView>();
        spikePhotonView.RPC("RemoveSpike", RpcTarget.AllBuffered, spikePhotonView.GetComponent<PhotonView>().ViewID);

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["SpikeExists"] = false;
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        // Ngừng nhấp nháy icon khi spike đã bị gỡ
        StopBlinkingIcon();
    }

    [PunRPC]
    public void StartBlinking()
    {
        StartCoroutine(BlinkIcon());
    }

    private IEnumerator BlinkIcon()
    {
        while (true)
        {
            iconPlant.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            iconPlant.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void StopBlinkingIcon()
    {
        StopCoroutine(BlinkIcon());
        iconPlant.gameObject.SetActive(false); // Tắt icon khi không cần nhấp nháy
    }
}
