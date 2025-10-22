using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera defaultCam;
    [SerializeField] private CinemachineCamera zoomCam;

    public void SwitchToZoom()
    {
        zoomCam.Priority = 20;
        defaultCam.Priority = 10;
    }

    public void SwitchToDefault()
    {
        zoomCam.Priority = 10;
        defaultCam.Priority = 20;
    }
}
