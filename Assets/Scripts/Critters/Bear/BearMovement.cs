using System.Collections;
using UnityEngine;

public class BearMovement : CritterMovement
{
    [SerializeField] private float fleeSpeedMultiplier = .75f;
    [SerializeField] private float fleeDuration = 4f;

    private Transform target;
    private bool isFleeing = false;
    private Vector3 fleeDirection;
    private Coroutine fleeRoutine;

    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ApplyBearMace(GameObject maceSource)
    {
        Vector3 awayFromMace = (transform.position - maceSource.transform.position);
        awayFromMace.y = 0f;
        fleeDirection = awayFromMace.normalized;

        if (fleeRoutine != null) StopCoroutine(fleeRoutine);
        fleeRoutine = StartCoroutine(FleeRoutine());
    }

    private void Update()
    {
        if (isStunned) return;

        if (isFleeing)
        {
            Vector3 fleeTarget = transform.position + fleeDirection * 2f;
            fleeTarget.y = transform.position.y;
            MoveTowards(fleeTarget);
        }
        else if (target != null)
        {
            MoveTowards(target.position);
        }
    }

    private IEnumerator FleeRoutine()
    {
        isFleeing = true;
        speedMultiplier = fleeSpeedMultiplier;
        yield return new WaitForSeconds(fleeDuration);
        StopFleeing();
    }

    private void StopFleeing()
    {
        if (fleeRoutine != null)
        {
            StopCoroutine(fleeRoutine);
            fleeRoutine = null;
        }

        isFleeing = false;
        speedMultiplier = 1f;
    }
}
