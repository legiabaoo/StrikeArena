using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace scgFullBodyController
{
    public class CameraController : MonoBehaviour
    {
        public float Sensitivity = 10f;
        public float minPitch = -30f;
        public float maxPitch = 60f;
        public Transform parent;
        public Transform boneParent;

        private float pitch = 0f;
        [HideInInspector] public float yaw = 0f;
        [HideInInspector] public float relativeYaw = 0f;

        private PhotonView photonView; // Thêm biến PhotonView

        void Awake()
        {
            // Lấy PhotonView từ GameObject
            photonView = GetComponent<PhotonView>();
        }

        void OnEnable()
        {
            if (photonView.IsMine) // Chỉ thực hiện khi là nhân vật của người chơi này
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void LateUpdate()
        {
            if (!photonView.IsMine) // Kiểm tra quyền sở hữu
                return;

            CameraRotate();
            transform.position = boneParent.position;
        }

        void CameraRotate()
        {
            // Get input to turn the cam view
            relativeYaw = Input.GetAxis("Mouse X") * Sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * Sensitivity;
            yaw += Input.GetAxis("Mouse X") * Sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }
}
