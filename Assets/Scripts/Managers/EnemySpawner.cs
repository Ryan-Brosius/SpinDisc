using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("References")]
    [SerializeField] BoxCollider arenaBounds;
    [SerializeField] List<Critter> unlockedEnemies;
    [SerializeField] GameObject[] enemyPrefabs;
    private GameManager gameManager;


    [Header("Drama Director")]
    [SerializeField] List<Critter> currentEnemies;
    [SerializeField] int maxEnemies;
    [SerializeField] Critter lastEnemySpawned;

    // Private trackers
    private Transform chosenTarget;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void SpawnCritter()
    {
        Critter selectedType = PickEnemyToSpawn();
        GameObject enemyToSpawn = FindEnemyPrefab(selectedType.GetType());
        Transform target;

        if (selectedType.GetPreferredTarget() != null) target = gameManager.FindActiveTower(selectedType.GetType()).transform;
        else target = gameManager.FindAnyTower()?.transform;

        Vector3 spawnPosition = SetSpawnPoint(target);

        GameObject enemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        if (enemy.TryGetComponent<Critter>(out Critter enemyScript))
        {
            enemyScript.SetTarget(target);
        }
    }

    public Critter PickEnemyToSpawn()
    {
        if (currentEnemies.Count >= maxEnemies) return null;

        List<Critter> enemyCandidates = new List<Critter>();

        foreach (Critter enemy in unlockedEnemies)
        {
            if (CanISpawn(enemy))
            {
                enemyCandidates.Add(enemy);
            }
        }

        if (enemyCandidates.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemyCandidates.Count);
            return enemyCandidates[randomIndex];
        }
        else
        {
            // Spawn a Raccoon if no possible targets
            return unlockedEnemies[0];
        }
    }

    private bool CanISpawn(Critter enemyType)
    {
        bool isSameAsPrevious = enemyType == lastEnemySpawned;
        bool hasTarget = gameManager.DoesTowerExist(enemyType.GetPreferredTarget());

        if (!isSameAsPrevious && hasTarget) return true;
        else return false;
    }
    
    private GameObject FindEnemyPrefab (Type enemyType)
    {
        return enemyPrefabs.FirstOrDefault(prefab =>
        {
            var critter = prefab.GetComponent<Critter>(); 
            return critter != null && critter.enabled && enemyType.IsInstanceOfType(critter);
        });
    }

    private Critter PickRandomEnemy()
    {
        int randomIndex = UnityEngine.Random.Range(0, unlockedEnemies.Count);
        return unlockedEnemies[randomIndex];
    }

    public Vector3 SetSpawnPoint(Transform Target)
    {
        Bounds bounds = arenaBounds.bounds;
        Vector3 center = bounds.center;

        Vector3 direction = (Target.position - center).normalized;

        float verticalEdge;
        float horizontalEdge;

        verticalEdge = Mathf.Abs(direction.z) > 0f 
            ? bounds.extents.z / Mathf.Abs(direction.z) 
            : float.MaxValue;

        horizontalEdge = Mathf.Abs(direction.x) > 0f
            ? bounds.extents.x / Mathf.Abs(direction.x)
            : float.MaxValue;

        float closestSide = Mathf.Min(verticalEdge, horizontalEdge);

        Vector3 spawnPos = center + direction * closestSide;
        spawnPos.y = 0f;
        return spawnPos;
    }

    public void ManualSpawnEnemy(GameObject EnemyPrefab, Transform Target)
    {
        Debug.Log("Spawning " + EnemyPrefab.name + " at" + Target);
        Vector3 spawnPosition = SetSpawnPoint(Target);

        GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);
        if (enemy.TryGetComponent<Critter>(out Critter enemyScript))
        {
            enemyScript.SetTarget(Target);
        }
    }

    private void Update()
    {

    }
}
