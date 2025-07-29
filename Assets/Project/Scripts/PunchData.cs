using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PunchData : NetworkBehaviour
{
    private NetworkVariable<ulong> owner = new(999);
    private NetworkVariable<bool> isActive = new(true);

    public static event Action<(ulong, ulong)> OnHitPlayer;

    public override void OnNetworkSpawn()
    {
        DeactivateSelfDelay();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetOwnershipServerRpc(ulong id)
    {
        this.owner.Value = id;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPunchActiveServerRpc(bool active)
    {
        isActive.Value = active;

        if (active == false)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            GetComponent<NetworkObject>().Spawn();
            DeactivateSelfDelay();
        }
    }

    public void DeactivateSelfDelay()
    {
        StartCoroutine(DeactivateSelfDelayCoroutine());
    }

    IEnumerator DeactivateSelfDelayCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        SetPunchActiveServerRpc(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // Prevent hitting self
                    if (networkObject.OwnerClientId == owner.Value)
                        return;

                    (ulong, ulong) fromShooterToHit = new(owner.Value, networkObject.OwnerClientId);
                    OnHitPlayer?.Invoke(fromShooterToHit);
                }
            }
        }
    }

}
