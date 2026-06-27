using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] GrillMenu grill;
    [SerializeField] GameObject cornButton;
    [SerializeField] GameObject hotdogButton;
    [SerializeField] GameObject maceButton;
    [SerializeField] GameObject tacocatButton;
    [SerializeField] GameObject lossScreen;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI cookTutorialText;
    [SerializeField] TextMeshProUGUI spinTutorialText;

    [Header("Food Purchases")]
    [SerializeField] List<TowerEntity> towersList;

    [Header("Critter Spawning")]
    private EnemySpawner spawner;
    [SerializeField] bool spawnEnabled = false;
    [SerializeField] float timeBetweenSpawns = 10.0f;
    [SerializeField] int critterScore;

    [Header("Tutorial Sequence")]
    [SerializeField] Transform firstSpawnPoint;
    [SerializeField] Transform secondSpawnPoint;
    [SerializeField] float delayBetweenSpawns = 10.0f;
    [SerializeField] GameObject tutorialRaccoon;
    private bool playTutorial = true;

    private void Start()
    {
        playTutorial = true;
        critterScore = 0;
        if (scoreText) scoreText.text = "SCORE: " + critterScore;
        if (spinTutorialText) spinTutorialText.gameObject.SetActive(false);
        spawner = EnemySpawner.Instance;
        StartCoroutine(AutomatedSpawnEnemy());
        ResetButtons();
        if (lossScreen  != null)
        {
            if (lossScreen.activeSelf == true)
            {
                lossScreen.SetActive(false);
                Time.timeScale = lossScreen.activeSelf ? 0f : 1f;
            }
        }
    }

    private void Update()
    {
        /*
        if (activeTowers.Count > 0) spawnEnabled = true;
        else spawnEnabled = false;
        */

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && lossScreen != null)
        {
            if (lossScreen.activeSelf == true)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.buildIndex);
            }
        }
    }

    public void AddTowerToList(GameObject towerToAdd)
    {
        activeTowers.Add(towerToAdd);
    }

    public void RemoveTowerFromList(GameObject towerToRemove)
    {
        activeTowers.Remove(towerToRemove);
        if (activeTowers.Count <= 0)
        {
            if (spawnEnabled) GameLoss();
        }
    }

    public void GameLoss()
    {
        lossScreen.SetActive(true);
        spawnEnabled = false;
        Time.timeScale = lossScreen.activeSelf ? 0f : 1f;
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

    public void PlayTutorialSequence()
    {
        if (!playTutorial) return;
        StartCoroutine(TutorialSequence());
    }

    public IEnumerator TutorialSequence()
    {
        playTutorial = false;
        spawner.ManualSpawnEnemy(tutorialRaccoon, activeTowers[0].transform, firstSpawnPoint);
        if (cookTutorialText != null) cookTutorialText.gameObject.SetActive(false);
        yield return new WaitForSeconds(delayBetweenSpawns / 2);
        if (spinTutorialText != null) spinTutorialText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenSpawns / 2);
        spawner.ManualSpawnEnemy(tutorialRaccoon, activeTowers[0].transform, secondSpawnPoint);
        yield return new WaitForSeconds(delayBetweenSpawns);
        if (spinTutorialText != null) spinTutorialText.gameObject.SetActive(false);
        spawnEnabled = true;
    }

    public void ActivateButton(GameObject button)
    {
        if (button != null && !button.gameObject.activeSelf)
        {
            button.gameObject.SetActive(true);
            if (grill != null) grill.Open();
        }
    }

    public void DeactivateButton(GameObject button)
    {
        if (button != null && button.gameObject.activeSelf) button.gameObject.SetActive(false);
    }

    public void ResetButtons()
    {
        DeactivateButton(cornButton);
        DeactivateButton(hotdogButton);
        DeactivateButton(maceButton);
        DeactivateButton(tacocatButton);
    }

    public void AddCritterScore(Critter critterType)
    {
        if (critterType is Raccoon) critterScore += 5;
        if (critterType is Ant) critterScore += 1;
        if (critterType is Squirrel) critterScore += 10;
        if (critterType is Bear) critterScore += 30;
        if (scoreText) scoreText.text = "SCORE: " + critterScore;
        UnlockTower();
        spawner.UnlockEnemies();
    }

    public void UnlockTower()
    {
        if (critterScore >= 15 && !cornButton.activeSelf) ActivateButton(cornButton);
        if (critterScore >= 35 && !hotdogButton.activeSelf) ActivateButton(hotdogButton);
        if (critterScore >= 300 && !tacocatButton) ActivateButton(tacocatButton);
    }

    public void UnlockBearMace()
    {
        if (!maceButton.activeSelf) ActivateButton(maceButton);
    }

    public int GetCurrentSore()
    {
        return critterScore;
    }
}
