using UnityEngine;

public class Bear : Critter
{
    [SerializeField] Transform target;

    protected override void Awake()
    {
        base.Awake();

        health.OnDamaged += OnDamaged;

        //movement.SetTarget(target);
    }

    void OnDamaged(Damage damage)
    {
        if (damage.Source != null && damage.Source.TryGetComponent<Mace>(out Mace mace))
        {
            OnBearMaceHit(mace.gameObject);
        }
    }

    public void OnBearMaceHit(GameObject maceSource)
    {
        BearMovement bearMovement = (BearMovement)movement;
        bearMovement.ApplyBearMace(maceSource);
    }
}