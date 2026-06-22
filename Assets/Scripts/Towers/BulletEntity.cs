using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BulletEntity : MonoBehaviour
{
    [SerializeField] private Transform firePos;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(float speed, Vector3 dir)
    {
        rb.linearVelocity = dir * speed;
    }
}
