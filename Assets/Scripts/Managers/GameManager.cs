using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

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

    [Header("Food Purchases")]
    [SerializeField] List<TowerEntity> towersList;

    [Header("Critter Spawning")]
    private EnemySpawner spawner;
    [SerializeField] bool spawnEnabled = false;
    [SerializeField] float timeBetweenSpawns = 10.0f;

    [Header("Tutorial Sequence")]
    [SerializeField] Transform firstSpawnPoint;
    [SerializeField] Transform secondSpawnPoint;
    [SerializeField] float delayBetweenSpawns;
    [SerializeField] GameObject tutorialRaccoon;

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
            var tower = prefab.GetComponent<TowerEntity>().GetTowerInfo();
            var platformObj = prefab.GetComponent<PlatformObject>();
            return tower != null && towerType.IsInstanceOfType(tower) && platformObj.isNotStolen;
        });
    }

    public GameObject FindAnyTower()
    {
        var availableTowers = activeTowers
            .Where(t => t.GetComponent<PlatformObject>()?.isNotStolen == true)
            .ToList();

        if (availableTowers.Count == 0) return null;
        return availableTowers[UnityEngine.Random.Range(0, availableTowers.Count)];
    }

    public IEnumerator AutomatedSpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            if (spawnEnabled) spawner.SpawnCritter();
        }
    }

    public IEnumerator TutorialSequence()
    {
        spawner.ManualSpawnEnemy(tutorialRaccoon, activeTowers[0].transform, firstSpawnPoint);
        yield return new WaitForSeconds(delayBetweenSpawns);
        spawner.ManualSpawnEnemy(tutorialRaccoon, activeTowers[0].transform, secondSpawnPoint);
    }
}
