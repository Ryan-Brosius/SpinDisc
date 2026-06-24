using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] BoxCollider arenaBounds;
    public Transform debugTarget;
    public GameObject debugEnemy;

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

    public void SpawnEnemy(GameObject EnemyType, Transform Target)
    {
        Debug.Log("Spawning " + EnemyType.name + " at" + Target);
        Vector3 spawnPosition = SetSpawnPoint(Target);

        GameObject enemy = Instantiate(EnemyType, spawnPosition, Quaternion.identity);
        if (enemy.TryGetComponent<Critter>(out Critter enemyScript))
        {
            enemyScript.SetTarget(Target);
            Debug.Log("tried to set target");
        }
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            // Triggers exactly once when the Spacebar is first pushed down
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SpawnEnemy(debugEnemy, debugTarget);
            }
        }
    }
}
