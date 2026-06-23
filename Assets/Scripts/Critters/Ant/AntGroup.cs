using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntGroup : MonoBehaviour
{
    [SerializeField] private GameObject antPrefab;
    [SerializeField] private CritterSettingsSO antSettings;
    [SerializeField] private float spawnInterval = 0.4f;
    [SerializeField] private int spawnCount = 5;

    [Header("Path Settings")]
    [SerializeField] private float sineAmplitude = 0.5f;
    [SerializeField] private float waveLength = 2f;
    [SerializeField] private int pointsPerWave = 20;

    private List<Vector3> trail = new List<Vector3>();
    [SerializeField] private Transform currentTarget;
    private float antPrefabY;
    private List<AntMovement> ants = new List<AntMovement>();

    private void Awake()
    {
        antPrefabY = antPrefab.transform.position.y;

        Spawn(currentTarget, spawnCount);
    }

    public void Spawn(Transform target, int antCount)
    {
        currentTarget = target;
        AppendWave(transform.position);
        StartCoroutine(SpawnAntsRoutine(antCount));
    }

    private IEnumerator SpawnAntsRoutine(int antCount)
    {
        for (int i = 0; i < antCount; i++)
        {
            SpawnAnt();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnAnt()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, antPrefabY, transform.position.z);
        GameObject go = Instantiate(antPrefab, spawnPos, Quaternion.identity);

        AntMovement ant = go.GetComponent<AntMovement>();
        ant.Initialize(antSettings);
        ant.SetupWithTrail(trail, antPrefabY);
        ants.Add(ant);
    }

    private void Update()
    {
        ants.RemoveAll(a => a == null);
        if (ants.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        AntMovement lead = GetLeadAnt();
        if (lead != null && lead.TrailIndex >= trail.Count - 3)
            AppendWave(GetTrailEnd());
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    private void AppendWave(Vector3 from)
    {
        if (currentTarget == null) return;

        Vector3 toTarget = (currentTarget.position - from);
        toTarget.y = 0f;
        Vector3 dir = toTarget.normalized;
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;

        for (int i = 0; i <= pointsPerWave; i++)
        {
            float t = i / (float)pointsPerWave;
            Vector3 point = from + dir * (t * waveLength);
            point += perp * (Mathf.Sin(t * Mathf.PI * 2f) * sineAmplitude);
            point.y = antPrefabY;
            trail.Add(point);
        }
    }

    private AntMovement GetLeadAnt()
    {
        AntMovement lead = null;
        int highestIndex = -1;

        foreach (AntMovement ant in ants)
        {
            if (ant != null && ant.TrailIndex > highestIndex)
            {
                highestIndex = ant.TrailIndex;
                lead = ant;
            }
        }

        return lead;
    }

    private Vector3 GetTrailEnd() => trail.Count > 0 ? trail[trail.Count - 1] : transform.position;

    private void OnDrawGizmos()
    {
        if (trail == null || trail.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < trail.Count - 1; i++)
            Gizmos.DrawLine(trail[i], trail[i + 1]);

        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        foreach (Vector3 p in trail)
            Gizmos.DrawSphere(p, 0.05f);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentTarget.position, 0.2f);
        }

        AntMovement lead = GetLeadAnt();
        if (lead != null && lead.TrailIndex < trail.Count)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(trail[lead.TrailIndex], 0.15f);
        }
    }
}