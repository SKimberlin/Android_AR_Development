using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{

    private Animator animator;
    private PlayerInputControls playerInputControls;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            animator = GetComponent<Animator>();
            playerInputControls = GetComponent<PlayerInputControls>();
            rb = GetComponent<Rigidbody>();

            playerInputControls.OnAttack1 += OnTriggerPlayerAttack1;
        }
    }

    private void OnTriggerPlayerAttack1()
    {
        animator.SetTrigger("Attack1");
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }
}
