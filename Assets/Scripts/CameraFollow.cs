using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner) return;

        // Tìm Virtual Camera trong cảnh
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform; // Gán Player làm đối tượng Follow
        }
    }
}