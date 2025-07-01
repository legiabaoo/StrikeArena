using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlantTheSpike : MonoBehaviour
{
    public static PlantTheSpike instance;
    public GameObject spikePrefab;
    public GameObject spike0;
    public float holdTimeRequired = 3.0f;
    public Slider progressBar;
    public Image iconPlant;

    private float holdTime = 0.0f;
    public bool hasPlacedBomb = false;
    private bool canPlantBomb = false;
    private bool isBlinking = false;
    public bool spikeExists = false;
    public bool isHasSpike = false;
    public bool isLocalPlayer;
    private Vector3 spikePosition;

    private float blinkInterval = 0.5f;

    [Header("UI")]
    public GameObject hasSpike;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (isLocalPlayer) { progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();}

        
        progressBar.gameObject.SetActive(false);
        progressBar.value = 0f;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (!spikeExists) RemoveSpikeFromScene();
        }
        hasSpike.SetActive(false);
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


        if (Input.GetKey(KeyCode.Alpha4))
        {
            if (!hasPlacedBomb && canPlantBomb && isHasSpike)
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
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
            {
                { "SpikeExists", true }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            PhotonView.Get(this).RPC("SetIsPlantSpike2", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("No PhotonView found on Spike prefab!");
        }
    }
    public void DropSpike()
    {
        int ViewID = PlayerPrefs.GetInt("ViewIDHasSpike");
        GameObject playerHasSpike = PhotonView.Find(ViewID).gameObject;
        // Tìm vị trí để đặt Spike
        Transform positionSpikeTransform = playerHasSpike.transform.Find("positionSpike");
        if (positionSpikeTransform == null)
        {
            Debug.LogError("Không tìm thấy positionSpike trong Player!");
            return;
        }

        Vector3 spikePosition2 = positionSpikeTransform.position;

        // Spawn Spike tại vị trí đã xác định
        GameObject spike = PhotonNetwork.Instantiate(spike0.name, spikePosition2, Quaternion.identity);

        // Gọi RPC để gán Tag hoặc xử lý khác
        PhotonView spikePhotonView = spike.GetComponent<PhotonView>();
        if (spikePhotonView != null)
        {
            spikePhotonView.RPC("SetSpike0Tag", RpcTarget.AllBuffered, spikePhotonView.ViewID);
            Debug.LogError("Rot bom diiiiiii");
        }
    }

    [PunRPC]
    public void SetIsPlantSpike2()
    {
        TimeManager.instance.isPlantSpike = true;
    }
    [PunRPC]
    public void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
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

    private void RemoveSpikeFromScene()
    {
        GameObject spike = GameObject.FindWithTag("Spike");
        if (spike != null) Destroy(spike);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Side")) canPlantBomb = true;
        if (other.CompareTag("Spike0"))
        {
            PhotonView spikePhotonView = other.GetComponent<PhotonView>();
            spikePhotonView.RPC("RemoveSpike", RpcTarget.AllBuffered, spikePhotonView.ViewID);
            isHasSpike = true;
            hasSpike.SetActive(true);
            PhotonView playerPhotonView = gameObject.GetComponent<PhotonView>();
            int ViewID = playerPhotonView.ViewID;
            PlayerPrefs.SetInt("ViewIDHasSpike", ViewID);
        }
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
