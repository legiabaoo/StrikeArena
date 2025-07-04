﻿//SlapChickenGames
//2021
//Sniper scope controller

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;

namespace scgFullBodyController
{
    public class sniperScopeController : MonoBehaviour
    {
        public CameraController camControl;
        public PostProcessProfile pprocessing;
        public float sniperAimSensitivty;
        float originalCamSensitivity;
        DepthOfField dofComponent;
        public Animator blackLensAnim;
        // Start is called before the first frame update
        void Start()
        {
            GameObject cameraControlObject = GameObject.Find("CameraControl");
            if (cameraControlObject != null)
            {
                camControl = cameraControlObject.GetComponent<CameraController>();
            }
            else
            {
                Debug.LogError("CameraControl object not found!");
            }
            
            originalCamSensitivity = camControl.Sensitivity;

            DepthOfField tmp;
            if (pprocessing.TryGetSettings<DepthOfField>(out tmp))
            {
                dofComponent = tmp;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gameObject.GetComponent<GunController>().aiming)
            {
                camControl.Sensitivity = sniperAimSensitivty;
                dofComponent.active = true;
                blackLensAnim.SetBool("aiming", true);
            }
            else
            {
                camControl.Sensitivity = originalCamSensitivity;
                dofComponent.active = false;
                blackLensAnim.SetBool("aiming", false);
            }
        }
    }
}
