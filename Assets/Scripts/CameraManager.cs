using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCameraBase thirdPersonCamera;
    public CinemachineVirtualCameraBase topDownCamera;

    private void OnEnable()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void ActiveThirdPersonCamera()
    {
        thirdPersonCamera.gameObject.SetActive(true);
        topDownCamera.gameObject.SetActive(false);
    }

    public void ActiveTopDownCamera()
    {
        thirdPersonCamera.gameObject.SetActive(false);
        topDownCamera.gameObject.SetActive(true);
    }
    public void ActiveChaseCamera()
    {
        thirdPersonCamera.gameObject.SetActive(false);
        topDownCamera.gameObject.SetActive(false);
    }
}