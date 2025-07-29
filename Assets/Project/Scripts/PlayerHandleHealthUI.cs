using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandleHealthUI : NetworkBehaviour
{
    [SerializeField] private Slider healthSlider;

    private Camera camera;

    public override void OnNetworkSpawn()
    {
        camera = Camera.main;
        PlayerDataManager.Instance.OnPlayerHealthChange += Instance_OnPlayerHealthChangeServerRpc;
        Instance_OnPlayerHealthChangeServerRpc(GetComponentInParent<NetworkObject>().OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Instance_OnPlayerHealthChangeServerRpc(ulong id)
    {
        if (GetComponentInParent<NetworkObject>().OwnerClientId == id)
        {
            SetHealthSliderClientRpc(id);
        }
    }

    [ClientRpc]
    private void SetHealthSliderClientRpc(ulong id)
    {
        healthSlider.value = PlayerDataManager.Instance.GetPlayerHealth(id) / 100;
    }

    private void Update()
    {
        if (camera)
        {
            healthSlider.transform.LookAt(camera.transform.position);
        }
    }
}
