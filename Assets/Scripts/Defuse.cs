using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Defuse : MonoBehaviour
{
    public static Defuse instance;
    public float holdTimeRequired = 3.0f;
    private float holdTime = 0.0f;
    private bool isInRange = false;
    private GameObject currentSpike;
    public Slider progressBar;
    public Image iconPlant;

    private float blinkInterval = 0.5f;
    private bool isBlinking = false;
    private bool spikeExists = false;

    private void Awake()
    {
        instance = this;
    }
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

        //Kiểm tra trạng thái spike khi bắt đầu
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (!spikeExists) RemoveSpikeFromScene();
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            bool currentSpikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (currentSpikeExists != spikeExists)
            {
                spikeExists = currentSpikeExists;
                if (spikeExists)
                    StartBlinking();
                else
                    StopBlinkingIcon();
            }
        }

        if (isInRange && currentSpike != null)
        {
            if (Input.GetKey(KeyCode.F))
            {
                progressBar.gameObject.SetActive(true);
                holdTime += Time.deltaTime;
                progressBar.value = holdTime / holdTimeRequired;

                if (holdTime >= holdTimeRequired)
                {
                    DefuseSpike();
                    progressBar.gameObject.SetActive(false);
                    holdTime = 0.0f;
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

    public void DefuseSpike()
    {
        if (currentSpike == null) return;

        PhotonView spikePhotonView = currentSpike.GetComponent<PhotonView>();
        spikePhotonView.RPC("RemoveSpike", RpcTarget.AllBuffered, spikePhotonView.ViewID);

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            { "SpikeExists", false }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        //SpikeManager.instance.photonView.RPC("SetIsSpikeExists", RpcTarget.AllBuffered, false);
        //TimeManager.instance.isPlantSpike = false;
        PhotonView.Get(this).RPC("SetIsPlantSpike", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void SetIsPlantSpike()
    {
        TimeManager.instance.isPlantSpike = false;
    }

    [PunRPC]
    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkIcon());
        }
    }

    private IEnumerator BlinkIcon()
    {
        while (isBlinking)
        {
            iconPlant.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            iconPlant.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    [PunRPC]
    public void StopBlinkingIcon()
    {
        isBlinking = false;
        StopAllCoroutines();
        iconPlant.gameObject.SetActive(false);
    }

    private void RemoveSpikeFromScene()
    {
        GameObject spike = GameObject.FindWithTag("Spike");
        if (spike != null) Destroy(spike);
    }
}
