using UnityEngine;

public class PlatformObject : MonoBehaviour
{
    private SpinningPlatform _owner;
    private readonly Color _gizmoOwnerLineColor = new Color(1f, 0.5f, 0f, 1f);

    public void SetOwner(SpinningPlatform newOwner)
    {
        _owner = newOwner;
    }

    public void RotateWithPlatform(Vector3 pivot, float angleDelta)
    {
        transform.RotateAround(pivot, Vector3.up, angleDelta);
    }

    public SpinningPlatform Owner => _owner;

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