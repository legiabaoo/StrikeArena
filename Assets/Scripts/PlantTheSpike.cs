using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlantTheSpike : MonoBehaviour
{
    public GameObject spikePrefab;
    public float holdTimeRequired = 3.0f;
    public Slider progressBar;
    public Image iconPlant;

    private float holdTime = 0.0f;
    private bool hasPlacedBomb = false;
    private bool canPlantBomb = false;
    private bool isBlinking = false;
    //private bool spikeExists = false;
    private Vector3 spikePosition;

    private float blinkInterval = 0.5f;

    void Start()
    {
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();
        progressBar.gameObject.SetActive(false);
        progressBar.value = 0f;

        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        //{
        //    spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
        //    if (!spikeExists) RemoveSpikeFromScene();
        //}
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            bool currentSpikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (currentSpikeExists != SpikeManager.instance.spikeExists)
            {
                SpikeManager.instance.spikeExists = currentSpikeExists;
                if (SpikeManager.instance.spikeExists)
                    StartBlinking();
                else
                    StopBlinkingIcon();
            }
        }


        if (Input.GetKey(KeyCode.Alpha4))
        {
            if (!hasPlacedBomb && canPlantBomb)
            {
                progressBar.gameObject.SetActive(true);
                holdTime += Time.deltaTime;
                progressBar.value = holdTime / holdTimeRequired;

                if (holdTime >= holdTimeRequired)
                {
                    PlaceSpike();
                    Debug.Log("plant");
                    hasPlacedBomb = true;
                    progressBar.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            holdTime = 0.0f;
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }

    }

    void PlaceSpike()
    {
        spikePosition = GameObject.Find("positionSpike").transform.position;
        GameObject spike = PhotonNetwork.Instantiate(spikePrefab.name, spikePosition, Quaternion.identity);
        PhotonView spikePhotonView = spike.GetComponent<PhotonView>();

        if (spikePhotonView != null)
        {
            spikePhotonView.RPC("SetSpikeTag", RpcTarget.AllBuffered, spikePhotonView.ViewID);
            //PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SpikeExists", true } });
            SpikeManager.instance.photonView.RPC("ReateIsSpikeExists", RpcTarget.AllBuffered, true);
        }
        else
        {
            Debug.LogError("No PhotonView found on Spike prefab!");
        }
    }

    [PunRPC]
    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            TimeManager.instance.isPlantSpike = true;
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            iconPlant.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);

            iconPlant.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    //private void RemoveSpikeFromScene()
    //{
    //    GameObject spike = GameObject.FindWithTag("Spike");
    //    if (spike != null) Destroy(spike);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Side")) canPlantBomb = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Side")) canPlantBomb = false;
    }

    private void StopBlinkingIcon()
    {
        isBlinking = false;
        StopCoroutine(Blink());
        iconPlant.gameObject.SetActive(false);
    }
}
