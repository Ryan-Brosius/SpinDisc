using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TowerEntity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected TowerSO tower;
    [SerializeField] protected Transform firePos;
    [SerializeField] protected BulletEntity pfBullet;
    [SerializeField] protected Animator animator;

    [Header("Range Settings")]
    private float range => tower.Range;
    private float rangeAngle => tower.Spread;
    [SerializeField] private LayerMask enemyLayer;
    private readonly List<Collider> _detected = new();

    protected bool TowerOnCooldown = false;

    // Update is called once per frame
    protected virtual void Update()
    {
        _detected.Clear();
        DetectInCone(_detected);

        if (TowerOnCooldown == false && _detected.Count > 0)
        {
            animator.SetTrigger("Fire");
        }
    }

    public virtual void Fire()
    {
        BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.LookRotation(firePos.transform.forward));
        bullet.Initialize(tower.Speed, tower.Spread, tower.Range, tower.Damage, gameObject);
        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }

    protected IEnumerator RefreshCooldown()
    {
        yield return new WaitForSeconds(tower.FireRate);

        TowerOnCooldown = false;
        animator.ResetTrigger("Fire");
    }

    private void DetectInCone(List<Collider> results)
    {
        var hits = Physics.OverlapSphere(
            transform.position,
            range,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            Vector3 closestPoint = hit.ClosestPoint(transform.position);
            Vector3 dirToTarget = (closestPoint - transform.position).normalized;
            dirToTarget = Vector3.ProjectOnPlane(dirToTarget, Vector3.up).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle <= rangeAngle / 2f)
                results.Add(hit);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        bool hasTarget = _detected != null && _detected.Count > 0;
        Color coneColor = hasTarget ? Color.green : Color.red;

        // Cone edges
        Gizmos.color = coneColor;
        Vector3 leftEdge = Quaternion.Euler(0, rangeAngle / 2f, 0) * forward * range;
        Vector3 rightEdge = Quaternion.Euler(0, -rangeAngle / 2f, 0) * forward * range;
        Gizmos.DrawLine(origin, origin + leftEdge);
        Gizmos.DrawLine(origin, origin + rightEdge);

        // Arc
        int arcSegments = 20;
        float angleStep = rangeAngle / arcSegments;
        Vector3 prev = origin + (Quaternion.Euler(0, rangeAngle / 2f, 0) * forward * range);
        for (int i = 1; i <= arcSegments; i++)
        {
            float a = rangeAngle / 2f - angleStep * i;
            Vector3 next = origin + (Quaternion.Euler(0, a, 0) * forward * range);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        // Lines to detected targets
        foreach (var col in _detected)
        {
            if (col != null)
                Gizmos.DrawLine(origin, col.transform.position);
        }
    }

    public TowerSO GetTowerInfo()
    {
        return tower;
    }
}