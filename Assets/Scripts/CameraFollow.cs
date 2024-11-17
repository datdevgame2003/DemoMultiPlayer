using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 0.125f;
    void Start()
    {
       
        if (NetworkManager.Singleton.LocalClient != null)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            target = playerObject.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}


