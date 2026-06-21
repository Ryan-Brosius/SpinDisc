using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class SpinningPlatform : MonoBehaviour
{
    [Header("Pivot")]
    public Transform customPivot;

    [Header("Spin Settings")]
    public float torqueSensitivity = 10f;
    [Range(0f, 1f)]
    public float angularDrag = 0.95f;

    [Header("Materials")]
    private Material defaultMaterial;
    public Material hoverMaterial;

    private Color gizmoPivotColor = Color.cyan;
    private Color gizmoDragColor = Color.yellow;
    private Color gizmoRiderColor = Color.green;

    private bool _isDragging;
    private float _angularVelocity;
    private Vector3 _lastMouseWorld;

    private bool _isHovered;
    private Renderer _renderer;
    private Collider _collider;

    private readonly HashSet<PlatformObject> _inside = new();
    private readonly List<PlatformObject> _riders = new();

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        defaultMaterial = _renderer.material;
    }

    private void Update()
    {
        CheckHover();
        HandleDragInput();
        ApplyCoast();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlatformObject>(out var obj))
            _inside.Add(obj);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlatformObject>(out var obj))
            _inside.Remove(obj);
    }


    private void CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool hit = Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide)
            && info.collider == _collider;

        if (hit && !_isHovered)
        {
            _isHovered = true;
            ApplyMaterial(hoverMaterial);
        }
        else if (!hit && _isHovered && !_isDragging)
        {
            _isHovered = false;
            ApplyMaterial(defaultMaterial);
        }

        if (hit && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _angularVelocity = 0f;
            _lastMouseWorld = MouseWorldXZ();
            ClaimRiders();
        }
    }

    private void HandleDragInput()
    {
        if (!_isDragging) return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
            if (!_isHovered)
                ApplyMaterial(defaultMaterial);
            return;
        }

        Vector3 pivot = PivotPosition();
        Vector3 mouseWorld = MouseWorldXZ();
        Vector3 mouseDelta = mouseWorld - _lastMouseWorld;
        _lastMouseWorld = mouseWorld;

        if (mouseDelta.sqrMagnitude < 0.000001f)
        {
            _angularVelocity = 0f;
            return;
        }

        Vector3 toMouse = mouseWorld - pivot;
        toMouse.y = 0f;
        float radius = toMouse.magnitude;
        if (radius < 0.001f)
            return;

        Vector3 radial = toMouse / radius;
        Vector3 tangent = new Vector3(-radial.z, 0f, radial.x);

        float tangential = Vector3.Dot(mouseDelta, tangent);

        float angleDelta = -tangential * torqueSensitivity;
        _angularVelocity = Time.deltaTime > 0f ? angleDelta / Time.deltaTime : 0f;

        ApplyRotation(angleDelta);
    }


    private void ApplyCoast()
    {
        if (_isDragging) return;

        if (Mathf.Abs(_angularVelocity) < 0.0001f)
        {
            _angularVelocity = 0f;
            return;
        }

        ApplyRotation(_angularVelocity * Time.deltaTime);
        _angularVelocity *= Mathf.Pow(1f - angularDrag, Time.deltaTime);
    }


    private void ClaimRiders()
    {
        foreach (var obj in _inside)
        {
            if (obj == null) continue;
            obj.SetOwner(this);
            _riders.Add(obj);
        }
    }

    public void ForceClaimRider(PlatformObject obj)
    {
        if (_riders.Remove(obj))
            obj.SetOwner(null);
    }


    private void ApplyRotation(float angleDelta)
    {
        Vector3 pivot = PivotPosition();
        transform.RotateAround(pivot, Vector3.up, angleDelta);

        foreach (var rider in _riders)
        {
            if (rider != null)
                rider.RotateWithPlatform(pivot, angleDelta);
        }
    }


    private Vector3 PivotPosition() =>
        customPivot != null ? customPivot.position : transform.position;

    private Vector3 MouseWorldXZ()
    {
        var mouse = Mouse.current;
        if (mouse == null) return Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
        float y = PivotPosition().y;
        float t = (y - ray.origin.y) / ray.direction.y;
        return ray.origin + ray.direction * t;
    }

    private void ApplyMaterial(Material mat)
    {
        if (_renderer != null && mat != null)
            _renderer.material = mat;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Vector3 pivot = PivotPosition();
        float radius = _collider != null ? _collider.bounds.extents.magnitude : 0.5f;

        Gizmos.color = gizmoPivotColor;
        Gizmos.DrawSphere(pivot, 0.08f);
        Gizmos.DrawLine(pivot, transform.position);

        if (Application.isPlaying && _isDragging)
        {
            Vector3 mouseWorld = MouseWorldXZ();
            Gizmos.color = gizmoDragColor;
            Gizmos.DrawLine(pivot, mouseWorld);

            Vector3 toMouse = mouseWorld - pivot;
            toMouse.y = 0f;
            if (toMouse.sqrMagnitude > 0.001f)
            {
                Vector3 radial = toMouse.normalized;
                Vector3 tangent = new Vector3(-radial.z, 0f, radial.x);
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(mouseWorld, tangent * 0.5f);
                Gizmos.DrawRay(mouseWorld, -tangent * 0.5f);
            }
        }

        Gizmos.color = gizmoRiderColor;
        foreach (var rider in _riders)
        {
            if (rider != null)
                Gizmos.DrawWireSphere(rider.transform.position, 0.12f);
        }

        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(pivot + Vector3.up * (radius + 0.3f),
            $"Vel: {_angularVelocity:F1}°/s  Riders: {_riders.Count}  Inside: {_inside.Count}");
    }
    #endregion
}