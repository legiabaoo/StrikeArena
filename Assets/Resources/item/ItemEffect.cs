using Photon.Pun;
using UnityEngine;
using System.Collections;
using scgFullBodyController;

public class ItemEffect : MonoBehaviour
{
    public int healthBoostAmount = 20;       // Lượng máu tăng khi va chạm với item "hp"
    public float speedBoostDuration = 5f;    // Thời gian tăng tốc
    public float speedMultiplier = 1.5f;    // Hệ số tăng tốc

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra va chạm với người chơi (tag "Player")
        if (other.CompareTag("Player") || other.CompareTag("OldPlayer"))
        {
            // Kiểm tra nếu đối tượng va chạm có tag "hp" (item tăng máu)
            if (gameObject.CompareTag("hp"))
            {
                // Tăng máu cho người chơi
                ApplyHealthBoost(other.gameObject);
                PhotonNetwork.Destroy(gameObject);  // Xóa item sau khi sử dụng và đồng bộ hóa qua Photon
            }
            // Kiểm tra nếu đối tượng va chạm có tag "speed" (item tăng tốc)
            else if (gameObject.CompareTag("speed"))
            {
                // Tăng tốc cho người chơi
                ActivateSpeedBoost(other.gameObject);
                PhotonNetwork.Destroy(gameObject);  // Xóa item sau khi sử dụng và đồng bộ hóa qua Photon
            }
        }
    }

    // Phương thức để tăng máu cho người chơi
    void ApplyHealthBoost(GameObject player)
    {
        health playerHealth = player.GetComponent<health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(-healthBoostAmount);  // Tăng máu cho người chơi
        }
    }

    // Phương thức để tăng tốc cho người chơi
    void ActivateSpeedBoost(GameObject player)
    {
        ThirdPersonUserControl playerControl = player.GetComponent<ThirdPersonUserControl>();
        if (playerControl != null)
        {
            StartCoroutine(SpeedBoostCoroutine(playerControl));
        }
    }

    // Coroutine để tăng tốc cho người chơi
    private IEnumerator SpeedBoostCoroutine(ThirdPersonUserControl playerControl)
    {
        float originalSpeed = playerControl.walkSpeed; // Lưu tốc độ ban đầu
        playerControl.walkSpeed *= speedMultiplier;    // Tăng tốc độ

        yield return new WaitForSeconds(speedBoostDuration); // Chờ hết thời gian hiệu ứng

        // Phục hồi tốc độ ban đầu nếu chưa bị thay đổi bởi logic khác
        if (playerControl.walkSpeed == originalSpeed * speedMultiplier)
        {
            playerControl.walkSpeed = originalSpeed;
        }
    }

}
