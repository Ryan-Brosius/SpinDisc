using System.Collections.Generic;
using UnityEngine;
using Paper.Core.Spring;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class SpinningPlatform : MonoBehaviour
{
    [Header("Pivot")]
    [Tooltip("Optional explicit pivot. Leave null to use this transform's position.")]
    public Transform customPivot;

    [Header("Spring Settings")]
    public float springHalfLife = 0.075f;
    public float springFrequency = 18f;

    [Header("Materials")]
    private Material defaultMaterial;
    public Material hoverMaterial;

    [Header("Gizmos")]
    public Color gizmoPivotColor = Color.cyan;
    public Color gizmoRadiusColor = new Color(0f, 1f, 1f, 0.25f);
    public Color gizmoDragColor = Color.yellow;
    public Color gizmoRiderColor = Color.green;

    private Spring _rotationSpring;
    private float _springAngle;
    private float _targetAngle;

    private bool _isDragging;
    private float _lastMouseAngle;
    private float _debugMouseAngle;

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

        _rotationSpring = new Spring(springHalfLife, springFrequency);
        _springAngle = transform.eulerAngles.y;
        _targetAngle = _springAngle;
        _rotationSpring.Initialize(new Vector3(_springAngle, 0f, 0f));

        defaultMaterial = _renderer.material;
    }

    private void Update()
    {
        CheckHover();
        HandleDragInput();
        TickSpring();
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
            _lastMouseAngle = MouseAngleFromPivot();
            ClaimRiders();
        }
    }

    private void HandleDragInput()
    {
        if (!_isDragging) return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
            ReleaseRiders();
            if (!_isHovered)
                ApplyMaterial(defaultMaterial);
            return;
        }

        float current = MouseAngleFromPivot();
        _debugMouseAngle = current;

        float delta = Mathf.DeltaAngle(_lastMouseAngle, current);
        _targetAngle -= delta;
        _lastMouseAngle = current;
    }

    private void ClaimRiders()
    {
        _riders.Clear();
        foreach (var obj in _inside)
        {
            if (obj == null) continue;
            obj.SetOwner(this);
            _riders.Add(obj);
        }
    }

    private void ReleaseRiders()
    {
        foreach (var rider in _riders)
        {
            if (rider != null)
                rider.SetOwner(null);
        }
        _riders.Clear();
    }

    private void TickSpring()
    {
        _rotationSpring.halfLife = springHalfLife;
        _rotationSpring.frequency = springFrequency;

        float prev = _springAngle;
        _springAngle = _rotationSpring.Update(new Vector3(_targetAngle, 0f, 0f), Time.deltaTime).x;

        float delta = _springAngle - prev;
        if (Mathf.Abs(delta) > 0.0001f)
            ApplyRotation(delta);
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

    private float MouseAngleFromPivot()
    {
        var mouse = Mouse.current;
        if (mouse == null) return 0f;

        Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());

        float y = PivotPosition().y;
        float t = (y - ray.origin.y) / ray.direction.y;
        Vector3 world = ray.origin + ray.direction * t;

        Vector3 pivot = PivotPosition();
        Vector2 dir = new Vector2(world.x - pivot.x, world.z - pivot.z);
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
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

        Gizmos.color = gizmoPivotColor;
        Gizmos.DrawSphere(pivot, 0.08f);
        Gizmos.DrawLine(pivot, transform.position);

        float radius = _collider != null ? _collider.bounds.extents.magnitude : 0.5f;

        if (Application.isPlaying && _isDragging)
        {
            Gizmos.color = gizmoDragColor;
            Vector3 mouseDir = new Vector3(
                Mathf.Cos(_debugMouseAngle * Mathf.Deg2Rad), 0f,
                Mathf.Sin(_debugMouseAngle * Mathf.Deg2Rad));
            Gizmos.DrawRay(pivot, mouseDir * radius);

            Gizmos.color = Color.red;
            Vector3 targetDir = new Vector3(
                Mathf.Cos(_targetAngle * Mathf.Deg2Rad), 0f,
                Mathf.Sin(_targetAngle * Mathf.Deg2Rad));
            Gizmos.DrawRay(pivot, targetDir * (radius * 0.7f));
        }

        Gizmos.color = gizmoRiderColor;
        foreach (var rider in _riders)
        {
            if (rider != null)
                Gizmos.DrawWireSphere(rider.transform.position, 0.12f);
        }

        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(pivot + Vector3.up * (radius + 0.3f),
            $"Spring: {(Application.isPlaying ? _springAngle : transform.eulerAngles.y):F1}°  Riders: {_riders.Count}  Inside: {_inside.Count}");
    }
    #endregion
}