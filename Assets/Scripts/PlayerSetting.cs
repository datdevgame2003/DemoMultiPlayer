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
            // Server gán tên cho người chơi
            networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        }

        // Cập nhật UI hiển thị tên
        playerName.text = networkPlayerName.Value.ToString();

        // Đăng ký sự kiện khi giá trị tên thay đổi
        networkPlayerName.OnValueChanged += OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        playerName.text = current.ToString();
    }

    void Update()
    {
        // Đảm bảo TextHolder luôn giữ hướng cố định
        if (textHolder != null)
        {
            textHolder.transform.rotation = Quaternion.identity; // Reset rotation
        }
    }
}

