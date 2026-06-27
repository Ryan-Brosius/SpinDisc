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
        //if (Keyboard.current.tKey.wasPressedThisFrame)
        //{
        //    SpawnTower(Smores);
        //}
    }

    private void SpawnTower(TowerEntity tower)
    {
        if (!CanSpawnTower)
            return;

        Vector3 spawnOffset = GetBestSpawnOffset(spawnPlatform.circumferenceRadius) * 0.1f;
        Vector3 spawnPos = spawnPlatform.transform.position + spawnOffset;

        GameObject newTower = Instantiate(tower, spawnPos, Quaternion.identity).gameObject;
        GameManager.Instance.AddTowerToList(newTower);
    }

    private Vector3 GetBestSpawnOffset(float placementRadius)
    {
        (Vector3 offset, float angleDeg)[] cardinals = new[]
        {
            (new Vector3( 0, 0,  1) * placementRadius,  90f),
            (new Vector3(-1, 0,  0) * placementRadius, 180f),
            (new Vector3( 0, 0, -1) * placementRadius, 270f),
            (new Vector3( 1, 0,  0) * placementRadius,   0f),
        };

        Vector3 platformPos = spawnPlatform.transform.position;
        int bestCount = int.MaxValue;
        Vector3 bestOffset = cardinals[0].offset;   

        foreach (var (offset, arcCenter) in cardinals)
        {
            int crowding = 0;
            foreach (var rider in spawnPlatform._inside)
            {
                Vector3 toRider = rider.transform.position - platformPos;
                float angle = Mathf.Atan2(toRider.z, toRider.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;
                float delta = Mathf.DeltaAngle(angle, arcCenter);
                if (Mathf.Abs(delta) <= 45f)
                    crowding++;
            }

            if (crowding == 0)
                return offset;

            if (crowding < bestCount)
            {
                bestCount = crowding;
                bestOffset = offset;
            }
        }

        return bestOffset;
    }

    #region Spawn Tower API Calls
    public void SpawnSmores() => SpawnTower(Smores);
    public void SpawnCorn() => SpawnTower(Corn);
    public void SpawnHotdog() => SpawnTower(Hotdog);
    public void SpawnChili() => SpawnTower(Chili);
    public void SpawnBearMace() => SpawnTower(BearMace);
    #endregion
}
