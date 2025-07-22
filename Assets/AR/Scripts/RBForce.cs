using UnityEngine;

public class RBForce : MonoBehaviour
{
    [SerializeField] float force;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * force, ForceMode.VelocityChange);
    }
}
