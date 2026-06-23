using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class CritterMovement : MonoBehaviour
{
    protected CritterSettingsSO settings;
    protected Rigidbody rb;
    protected float speedMultiplier = 1f;

    protected bool isStunned = false;

    public virtual void Initialize(CritterSettingsSO settings)
    {
        this.settings = settings;
        rb = GetComponent<Rigidbody>();
    }

    public abstract void SetTarget(Transform target);

    protected void MoveTowards(Vector3 targetPosition)
    {
        if (isStunned) return;

        Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 dir = Vector3.ProjectOnPlane(flatTarget - transform.position, Vector3.up).normalized;
        rb.linearVelocity = dir * settings.MaxSpeed * speedMultiplier;

        RotateTowards(targetPosition);
    }

    protected void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    protected void StopMoving()
    {
        rb.linearVelocity = Vector3.zero;
    }

    public IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        StopMoving();
        yield return new WaitForSeconds(duration);
        isStunned = false;
        OnStunEnd();
    }

    protected virtual void OnStunEnd() { }
}
