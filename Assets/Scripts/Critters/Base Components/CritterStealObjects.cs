using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CritterStealObjects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CritterHealth health;
    [SerializeField] private CritterMovement movement;
    [SerializeField] private Critter critter;
    [SerializeField] private Transform ObjectHoldLocation;

    public bool IsHoldingObject = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (IsHoldingObject) return;

        if (collision.gameObject.TryGetComponent<PlatformObject>(out PlatformObject obj))
        {
            if (!obj.isNotStolen)
                return;

            IsHoldingObject = true;
            obj.Owner.RemoveRider(obj);
            obj.SetOwner(null);
            obj.isNotStolen = false;

            health.TakeDamage(new Damage(-9999, gameObject));
            critter.enabled = false;
            movement.enabled = false;

            if (collision.TryGetComponent<TowerEntity>(out TowerEntity tower))
            {
                tower.enabled = false;
            }

            StartCoroutine(StealSequence(obj));
        }
    }

    IEnumerator StealSequence(PlatformObject obj)
    {
        obj.transform.SetParent(ObjectHoldLocation, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        Vector3 reverseDirection = Vector3.ProjectOnPlane((transform.position - Vector3.zero), Vector3.up).normalized;

        //xd
        if (critter is Ant)
        {
            Ant ant = (Ant)critter;
            ant.AntGroup.LeavePosition = reverseDirection * 100f;
            movement.enabled = true;
        }
        else
            movement.MoveTowards(reverseDirection * 100f);

        yield break;
    }
}
