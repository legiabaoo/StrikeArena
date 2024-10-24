using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI; // Để sử dụng các thành phần UI

public class PlantTheSpike : MonoBehaviour
{
    public GameObject spikePrefab; // Prefab của Spike
    public float holdTimeRequired = 3.0f; // Thời gian giữ phím cần thiết (3 giây)
    public Slider progressBar; // Thanh loading

    private float holdTime = 0.0f; // Biến đếm thời gian giữ phím
    private bool hasPlacedBomb = false; // Biến kiểm tra xem bom đã được đặt hay chưa
    private bool canPlantBomb = false; // Biến kiểm tra người chơi có thể đặt bom hay không
    private Vector3 vertor3;

    void Start()
    {
        progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();
        
        // Đặt giá trị ban đầu cho thanh progressBar
        progressBar.gameObject.SetActive(false); // Ẩn thanh loading khi chưa nhấn phím
        progressBar.value = 0f; // Đặt giá trị ban đầu của thanh loading là 0

        // Kiểm tra custom properties để xác định trạng thái của spike
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SpikeExists"))
        {
            bool spikeExists = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpikeExists"];
            if (!spikeExists)
            {
                // Nếu spike không tồn tại, gọi hàm để xóa spike từ scene
                RemoveSpikeFromScene();
            }
        }
    }

    void Update()
    {
        // Nếu chưa đặt bom thì mới cho phép đặt bom và hiển thị thanh loading
        if (!hasPlacedBomb && canPlantBomb)
        {
            if (Input.GetKey(KeyCode.Alpha4))
            {
                // Bật thanh loading
                progressBar.gameObject.SetActive(true);

                // Tăng thời gian giữ phím
                holdTime += Time.deltaTime;

                // Cập nhật thanh loading (progressBar)
                progressBar.value = holdTime / holdTimeRequired;

                // Nếu giữ đủ thời gian yêu cầu, đặt bom
                if (holdTime >= holdTimeRequired)
                {
                    PlaceSpike();
                    hasPlacedBomb = true;
                    progressBar.gameObject.SetActive(false); // Ẩn thanh loading sau khi đặt bom
                }
            }
            else
            {
                // Nếu thả phím, reset thời gian và ẩn thanh loading
                holdTime = 0.0f;
                progressBar.value = 0f; // Reset giá trị thanh loading
                progressBar.gameObject.SetActive(false); // Ẩn thanh loading
            }
        }
    }

    // Hàm sinh đối tượng Spike tại vị trí của người chơi
    void PlaceSpike()
    {
        vertor3 = GameObject.Find("positionSpike").transform.position;

        GameObject spike = PhotonNetwork.Instantiate(spikePrefab.name, vertor3, Quaternion.identity);

        // Lấy PhotonView của spike sau khi instantiate
        PhotonView spikePhotonView = spike.GetComponent<PhotonView>();

        if (spikePhotonView != null)
        {
            // Gọi RPC để đồng bộ hóa tag
            spikePhotonView.RPC("SetSpikeTag", RpcTarget.AllBuffered, spikePhotonView.ViewID);

            // Cập nhật custom properties để đánh dấu rằng spike đã được đặt
            Hashtable properties = new Hashtable();
            properties["SpikeExists"] = true; // Spike tồn tại
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        else
        {
            Debug.LogError("No PhotonView found on Spike prefab!");
        }
    }

    private void RemoveSpikeFromScene()
    {
        // Tìm và xóa spike nếu nó tồn tại trong scene
        GameObject spike = GameObject.FindWithTag("Spike");
        if (spike != null)
        {
            Destroy(spike); // Hủy spike nếu tìm thấy
        }
    }


    // Khi người chơi va chạm với đối tượng có tag "Side"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Side"))
        {
            canPlantBomb = true; // Cho phép đặt bom khi ở trên đối tượng có tag "Side"
        }
    }

    // Khi người chơi rời khỏi đối tượng có tag "Side"
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Side"))
        {
            canPlantBomb = false; // Không cho phép đặt bom khi rời khỏi đối tượng có tag "Side"
        }
    }
}
