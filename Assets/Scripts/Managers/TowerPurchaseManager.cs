using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPurchaseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpinningPlatform spawnPlatform;

    [Header("Tower Prefabs")]
    [SerializeField] TowerEntity Smores;
    [SerializeField] TowerEntity Corn;
    [SerializeField] TowerEntity Hotdog;
    [SerializeField] TowerEntity Chili;
    [SerializeField] TowerEntity BearMace;

    // Dont spawn a tower if there isnt any space
    private bool CanSpawnTower => spawnPlatform.maxRiders > spawnPlatform.RiderCount;

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SpawnTower(Smores);
        }
    }

    private void SpawnTower(TowerEntity tower)
    {
        if (!CanSpawnTower)
            return;

        // Pushes it slightly forward
        Instantiate(tower, spawnPlatform.transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.identity);
    }

    #region Spawn Tower API Calls
    public void SpawnSmores() => SpawnTower(Smores);
    public void SpawnCorn() => SpawnTower(Corn);
    public void SpawnHotdog() => SpawnTower(Hotdog);
    public void SpawnChili() => SpawnTower(Chili);
    public void SpawnBearMace() => SpawnTower(BearMace);
    #endregion
}
