using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;
    private PlayerInputControls playerInputControls;
    private Rigidbody rb;

    private Vector3 lastPosition;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            animator = GetComponent<Animator>();
            playerInputControls = GetComponent<PlayerInputControls>();
            rb = GetComponent<Rigidbody>();

            lastPosition = transform.position;

            playerInputControls.OnAttack1 += OnTriggerPlayerAttack1;
        }
    }

    private void OnTriggerPlayerAttack1()
    {
        animator.SetTrigger("Attack1");
    }

    void Update()
    {
        if (!IsOwner) return;

        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", speed);
        lastPosition = transform.position;
    }
}
