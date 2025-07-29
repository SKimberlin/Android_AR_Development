using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPunch : NetworkBehaviour
{
    private PlayerInputControls playerInputControls;
    private const float PUNCH_DELAY = 0.4f;
    private const float PUNCH_TIMER = 0.4f;
    private Transform punchTransform;

    [SerializeField] private GameObject punchPrefab;
    public override void OnNetworkSpawn()
    {
        punchTransform = GetComponentInChildren<PunchTransformReference>().transform;
        

        if (GetComponent<NetworkObject>().IsOwner)
        {
            playerInputControls = GetComponent<PlayerInputControls>();

            playerInputControls.OnAttack1 += OnPlayerInputAttack;
        }
    }

    private void OnPlayerInputAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(PUNCH_DELAY);
        OnPlayerInputAttackServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerInputAttackServerRpc(ulong id)
    {
        GameObject punch = Instantiate(punchPrefab, punchTransform);

        NetworkObject punchNetworkObj = punch.GetComponent<NetworkObject>();
        punchNetworkObj.Spawn();

        punch.GetComponent<PunchData>().SetOwnershipServerRpc(id);
    }
}
