using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner) return;

        // Tim Virtual Camera
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform; 
        }
    }
}