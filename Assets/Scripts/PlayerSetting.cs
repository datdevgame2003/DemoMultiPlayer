using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Transform textHolder;

    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>(
        "Player: 0",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server gan ten cho nguoi choi
            networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        }

        // UI hien thi tên
        playerName.text = networkPlayerName.Value.ToString();

        networkPlayerName.OnValueChanged += OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        playerName.text = current.ToString();
    }

    void Update()
    {
        // TextHolder giu huong co đinh
        if (textHolder != null)
        {
            textHolder.transform.rotation = Quaternion.identity; // Reset rotation
            
        }
    }
}

