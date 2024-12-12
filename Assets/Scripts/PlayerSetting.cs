using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName; //hien thi player name
    [SerializeField] private Transform textHolder;//vi tri ten
    //bien mang luu player name
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>(
        "Player: 0",
        NetworkVariableReadPermission.Everyone, //clients doc ten
        NetworkVariableWritePermission.Server   //server ghi ten
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Server gan ten cho nguoi choi
            networkPlayerName.Value = "Player: " + (OwnerClientId + 1); //gan ten theo id player
        }

        // UI hien thi tên
        playerName.text = networkPlayerName.Value.ToString(); //gan ten

        networkPlayerName.OnValueChanged += OnPlayerNameChanged; //cap nhat ten
    }

    private void OnPlayerNameChanged(FixedString128Bytes previous, FixedString128Bytes current) //cap nhat ten
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

