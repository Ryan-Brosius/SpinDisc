using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlatformObject : MonoBehaviour
{
    private SpinningPlatform _owner;
    private readonly Color _gizmoOwnerLineColor = new Color(1f, 0.5f, 0f, 1f);

    public SpinningPlatform Owner => _owner;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    public void SetOwner(SpinningPlatform newOwner)
    {
        if (_owner != null && _owner != newOwner)
            _owner.ForceClaimRider(this);
        _owner = newOwner;
    }

    public void RotateWithPlatform(Vector3 pivot, float angleDelta)
    {
        transform.RotateAround(pivot, Vector3.up, angleDelta);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;
        if (!other.TryGetComponent<PlatformObject>(out var otherObj)) return;
        if (otherObj.Owner == null || otherObj.Owner == _owner) return;

        if (Mathf.Abs(Owner.AngularVelocity) < Mathf.Abs(otherObj.Owner.AngularVelocity)) return;

        float sourceVelocity = _owner.AngularVelocity;

        if (_owner.IsDragging)
        {
            otherObj.Owner.ReceiveAngularImpulse(-sourceVelocity);
        }
        else
        {
            otherObj.Owner.ReceiveAngularImpulse(-sourceVelocity * 0.8f);
            _owner.ReceiveAngularImpulse(-sourceVelocity * 0.8f);
        }
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (_owner != null)
        {
            Gizmos.color = _gizmoOwnerLineColor;
            Gizmos.DrawLine(transform.position, _owner.transform.position);
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.15f);
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.25f,
                $"Owner: {_owner.name}");
        }
        else
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.12f);
            UnityEditor.Handles.color = Color.grey;
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.25f, "No platform");
        }
    }
    #endregion
}