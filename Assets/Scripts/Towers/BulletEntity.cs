using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BulletEntity : MonoBehaviour
{
    [SerializeField] private Transform firePos;

    private Rigidbody rb;

    private Vector3 startPos;
    public float bulletRange;
    [SerializeField] private Damage damage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(startPos, transform.position) > bulletRange)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float speed, float spread, float range, float damage, GameObject source)
    {
        this.damage = new Damage(damage, source);

        float angle = UnityEngine.Random.Range(0f - (spread / 2), (spread / 2));
        transform.rotation = transform.rotation * Quaternion.Euler(0f, angle, 0f);
        rb.linearVelocity = transform.forward * speed;

        startPos = transform.position;
        bulletRange = range;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CritterHealth>(out CritterHealth health))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
