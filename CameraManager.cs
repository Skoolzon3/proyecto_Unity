using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // Posición de cámara por defecto
    [SerializeField] private CinemachineVirtualCamera _idleCam;
    // Posición de cámara de seguimiento del pájaro
    [SerializeField] private CinemachineVirtualCamera _followCam;

    // Awake: Invocación de cámara por defecto al inicio
    private void Awake()
    {
        SwitchToIdleCam();
    }

    // SwitchToIdleCam: Cambio a cámara por defecto
    public void SwitchToIdleCam()
    {
        _idleCam.enabled = true;
        _followCam.enabled = false;
    }

    // SwitchToFollowCam: Cambio a cámara del pájaro lanzado
    public void SwitchToFollowCam(Transform followTransform)
    {
        _followCam.Follow = followTransform;

        _followCam.enabled = true;
        _idleCam.enabled = false;
    }

}
