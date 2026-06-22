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

    public void Initialize(float speed, float spread)
    {
        float angle = UnityEngine.Random.Range(0f - (spread/2), (spread/2));
        transform.rotation = transform.rotation * Quaternion.Euler(0f, angle, 0f);
        rb.linearVelocity = transform.forward * speed;
    }
}
