using UnityEngine;

public class Racoon : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private CritterSettingsSO settings;

    [SerializeField] private CritterHealth health;
    [SerializeField] private CritterMovement movement;

    private void Awake()
    {
        health.Initialize(settings.Health);
        movement.Initialize(settings);

        health.OnDamaged += ApplyStun;
        health.OnDied += HandleDeath;

        SetTarget(target);
    }

    public void SetTarget(Transform target)
    {
        movement.SetTarget(target);
    }

    private void ApplyStun(Damage damage)
    {
        movement.StunRoutine(.5f);
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
