using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Defuse : MonoBehaviour
{
    public float holdTimeRequired = 3.0f; // Thời gian giữ phím cần thiết để gỡ bom
    private float holdTime = 0.0f; // Biến đếm thời gian giữ phím
    private bool isInRange = false; // Biến kiểm tra xem người chơi có đứng gần bom không
    private GameObject currentSpike; // Spike hiện tại để gỡ
    public Slider progressBar; // Thanh loading

    void Start()
    {
        // Tìm ProgressBar trong prefab
        
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false); // Ẩn thanh loading khi bắt đầu
            progressBar.value = 0f; // Đặt giá trị ban đầu của thanh loading
        }
        else
        {
            Debug.LogError("ProgressBar not found in the player prefab!");
        }
    }

    void Update()
    {
        if (isInRange && currentSpike != null)
        {
            if (Input.GetKey(KeyCode.Alpha4))
            {
                // Bật thanh loading
                progressBar.gameObject.SetActive(true);

                // Tăng thời gian giữ phím
                holdTime += Time.deltaTime;

                // Cập nhật thanh loading (progressBar)
                progressBar.value = holdTime / holdTimeRequired;

                // Nếu giữ đủ thời gian yêu cầu, gỡ bom
                if (holdTime >= holdTimeRequired)
                {
                    DefuseSpike();
                    progressBar.gameObject.SetActive(false); // Ẩn thanh loading sau khi gỡ bom
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spike")) // Thay thế "Spike" bằng tag của đối tượng Spike
        {
            isInRange = true; // Đặt cờ cho phép gỡ bom
            currentSpike = other.gameObject; // Lưu đối tượng spike hiện tại
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spike")) // Thay thế "Spike" bằng tag của đối tượng Spike
        {
            isInRange = false; // Đặt cờ không cho phép gỡ bom
            currentSpike = null; // Xóa đối tượng spike hiện tại
        }
    }

    private void DefuseSpike()
    {
        PhotonView spikePhotonView = currentSpike.GetComponent<PhotonView>();
        // Gọi RPC để gỡ spike
        spikePhotonView.RPC("DestroySpike", RpcTarget.AllBuffered, spikePhotonView.GetComponent<PhotonView>().ViewID);
    }

    // RPC để hủy spike
    [PunRPC]
    private void DestroySpike(int spikeViewID)
    {
        PhotonView spikeView = PhotonView.Find(spikeViewID);
        if (spikeView != null)
        {
            Destroy(spikeView.gameObject); // Hủy spike trên tất cả các client
            Debug.Log("Spike defused on all clients!");
        }
        else
        {
            Debug.LogError("Spike not found with ViewID: " + spikeViewID);
        }
        holdTime = 0.0f; // Reset thời gian
    }
}
