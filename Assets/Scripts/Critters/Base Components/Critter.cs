using UnityEngine;

[RequireComponent(typeof(CritterHealth))]
public class Critter : MonoBehaviour
{
    [SerializeField] protected CritterSettingsSO settings;

    [SerializeField] protected CritterHealth health;
    [SerializeField] protected CritterMovement movement;

    protected virtual void Awake()
    {
        health.Initialize(settings.Health);
        movement.Initialize(settings);

        health.OnDamaged += ApplyStun;
        health.OnDied += HandleDeath;
    }

    public void SetTarget(Transform target)
    {
        movement.SetTarget(target);
    }

    public TowerSO GetPreferredTarget()
    {
        return settings.Target;
    }

    private void ApplyStun(Damage damage)
    {
        movement.StunRoutine(.5f);
    }

    protected virtual void HandleDeath()
    {
        GameManager.Instance.AddCritterScore(this);
        Destroy(gameObject);
    }
}
