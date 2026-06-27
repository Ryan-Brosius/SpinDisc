using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

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
    [SerializeField] GameObject stupidAntGroupSpecialCaseAssB;
    private GameManager gameManager;


    [Header("Drama Director")]
    [SerializeField] List<GameObject> currentEnemies;
    [SerializeField] int maxEnemies;
    [SerializeField] Critter lastEnemySpawned;

    // Enemy targetting stuff
    List<PlatformObject> existingObjects;
    private int roundRobinIndex = 0;

    // Private trackers
    private Transform chosenTarget;

    private void Start()
    {
        gameManager = GameManager.Instance;

        StartCoroutine(EnemyUpdateRoutine());
    }

    public void UnlockEnemies()
    {
        int score = gameManager.GetCurrentSore();
        if (score >= 15 && !IsEnemyUnlocked(enemyPrefabs[1]))
        {
            if (enemyPrefabs[1].TryGetComponent<Critter>(out Critter critterType))
            {
                unlockedEnemies.Add(critterType);
            }
        }
        if (score >= 50 && !IsEnemyUnlocked(enemyPrefabs[2]))
        {
            if (enemyPrefabs[2].TryGetComponent<Critter>(out Critter critterType))
            {
                unlockedEnemies.Add(critterType);
            }
        }
        if (score >= 100 && !IsEnemyUnlocked(enemyPrefabs[3]))
        {
            if (enemyPrefabs[3].TryGetComponent<Critter>(out Critter critterType))
            {
                unlockedEnemies.Add(critterType);
            }
        }
    }

    public bool IsEnemyUnlocked(GameObject critterObj)
    {
        Critter critterType = critterObj.GetComponent<Critter>();
        if (critterType != null)
        {
            if (unlockedEnemies.Contains(critterType)) return true;
        }
        return false;
    }

    public void SpawnCritter()
    {
        Critter selectedType = PickEnemyToSpawn();
        GameObject enemyToSpawn = null;
        if (selectedType is Ant) enemyToSpawn = stupidAntGroupSpecialCaseAssB;
        else enemyToSpawn = FindEnemyPrefab(selectedType.GetType());

        Transform target = null;

        if (selectedType.GetPreferredTarget() != null)
        {
            GameObject activeTower = gameManager.FindActiveTower(selectedType.GetPreferredTarget().GetType());
            if (activeTower != null) target = activeTower.transform;
        }
        else
        {
            if (gameManager.FindAnyTower() != null) target = gameManager.FindAnyTower().transform;
        }

        if (target == null)
            return;

        Vector3 spawnPosition = SetSpawnPoint(target);

        GameObject enemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        lastEnemySpawned = enemy.GetComponent<Critter>();
        currentEnemies.Add(enemy);
        if (enemy.TryGetComponent<Critter>(out Critter enemyScript))
        {
            enemyScript.SetTarget(target);
        }
        if (enemy.TryGetComponent<AntGroup>(out AntGroup antGroup))
        {
            antGroup.Spawn(target);
        }
    }

    public Critter PickEnemyToSpawn()
    {
        // Need to enable later when max critter is implemented
        // if (currentEnemies.Count >= maxEnemies) return null;

        List<Critter> enemyCandidates = new List<Critter>();

        foreach (Critter enemy in unlockedEnemies)
        {
            if (CanISpawn(enemy))
            {
                enemyCandidates.Add(enemy);
                Debug.Log("Added enemy " + enemy.name);
            }
        }

        if (enemyCandidates.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemyCandidates.Count);
            return enemyCandidates[randomIndex];
        }
        else
        {
            Debug.Log("Spawning only raccoon");
            // Spawn a Raccoon if no possible targets
            return unlockedEnemies[0];
        }
    }

    private bool CanISpawn(Critter enemyType)
    {
        bool isSameAsPrevious = false;
        bool hasTarget = false;
        if (lastEnemySpawned != null && enemyType.GetType() == lastEnemySpawned.GetType()) isSameAsPrevious = true;
        if (enemyType.GetPreferredTarget() != null) hasTarget = gameManager.DoesTowerExist(enemyType.GetPreferredTarget());
        else return true;


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

    public void ManualSpawnEnemy(GameObject EnemyPrefab, Transform Target, Transform position)
    {
        // Debug.Log("Spawning " + EnemyPrefab.name + " at" + Target);
        Vector3 spawnPosition = SetSpawnPoint(position);

        GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);
        if (enemy.TryGetComponent<Critter>(out Critter enemyScript))
        {
            enemyScript.SetTarget(Target);
        }
    }

    private void Update()
    {

    }

    private IEnumerator EnemyUpdateRoutine()
    {
        while (true)
        {
            if (currentEnemies.Count > 0)
            {
                if (roundRobinIndex == 0)
                {
                    Collider[] hits = Physics.OverlapSphere(Vector3.zero, 10);
                    existingObjects = hits
                        .Select(h => h.GetComponent<PlatformObject>())
                        .Where(p => p != null && p.isNotStolen)
                        .ToList();
                }

                roundRobinIndex %= currentEnemies.Count;
                GameObject enemy = currentEnemies[roundRobinIndex];

                if (enemy != null)
                {
                    UpdateEnemy(enemy);
                    roundRobinIndex = (roundRobinIndex + 1) % currentEnemies.Count;
                }
                else
                {
                    currentEnemies.RemoveAt(roundRobinIndex);
                    roundRobinIndex = currentEnemies.Count > 0 ? roundRobinIndex % currentEnemies.Count : 0;
                }
            }
            yield return null;
        }
    }

    private void UpdateEnemy(GameObject enemy)
    {
        PlatformObject closest = existingObjects
            .Where(p => p != null && p.isNotStolen)
            .OrderBy(p => Vector3.Distance(enemy.transform.position, p.transform.position))
            .FirstOrDefault();

        if (closest == null) return;

        if (enemy.TryGetComponent<Critter>(out Critter critter))
        {
            if (enemy.TryGetComponent<CritterStealObjects>(out CritterStealObjects steal))
            {
                if (steal.IsHoldingObject) return;
            }
            critter.SetTarget(closest.transform);
        }
        else if (enemy.TryGetComponent<AntGroup>(out AntGroup antGroup))
        {
            // antGroup.SetTarget(closest.transform);
        }
    }
}
