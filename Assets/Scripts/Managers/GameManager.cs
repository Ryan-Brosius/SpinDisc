using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header ("References")]
    [SerializeField] List<GameObject> activeTowers;

    [Header("Critter Spawning")]
    private EnemySpawner spawner;
    [SerializeField] bool spawnEnabled = false;
    [SerializeField] float timeBetweenSpawns = 10.0f;

    private void Start()
    {
        spawner = EnemySpawner.Instance;
        StartCoroutine(AutomatedSpawnEnemy());
    }

    private void Update()
    {
        if (activeTowers.Count > 0) spawnEnabled = true;
        else spawnEnabled = false;
    }

    public void AddTowerToList(GameObject towerToAdd)
    {
        activeTowers.Add(towerToAdd);
    }

    public void RemoveTowerFromList(GameObject towerToRemove)
    {
        activeTowers.Remove(towerToRemove);
    }

    public List<GameObject> GetActiveTowers()
    {
        return activeTowers;
    }

    public bool DoesTowerExist(TowerSO towerToCheck)
    {
        foreach(GameObject tower in activeTowers)
        {
            if (towerToCheck == tower.GetComponent<TowerEntity>().GetTowerInfo()) return true;
        }
        return false;
    }

    public GameObject FindActiveTower(Type towerType)
    {
        return activeTowers.FirstOrDefault(prefab =>
        {
            var tower = prefab.GetComponent<TowerSO>();
            return tower != null && towerType.IsInstanceOfType(tower);
        });
    }

    public GameObject FindAnyTower()
    {
        int randomIndex = UnityEngine.Random.Range(0, activeTowers.Count);
        return activeTowers[randomIndex];
    }

    public IEnumerator AutomatedSpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            spawner.SpawnCritter();
        }
    }
}
