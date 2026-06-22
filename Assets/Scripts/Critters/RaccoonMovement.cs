using UnityEngine;

public class RaccoonMovement : CritterMovement
{
    private Transform target;

    public override void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null || isStunned) return;
        MoveTowards(target.position);
    }
}
