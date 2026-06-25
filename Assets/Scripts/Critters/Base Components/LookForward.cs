using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LookForward : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RotateTowards(rb.linearVelocity);
    }

    protected void RotateTowards(Vector3 linearVelocity)
    {
        if (linearVelocity == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(linearVelocity);
    }
}
