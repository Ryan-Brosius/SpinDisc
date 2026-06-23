using UnityEngine;

[CreateAssetMenu(fileName = "TowerSO", menuName = "Scriptable Objects/TowerSO")]
public class TowerSO : ScriptableObject
{
    public string Name;
    
    public float Damage;

    public float Range;

    public float FireRate;

    [Range(0f,360f)]
    public float Spread;

    public float Speed;
}
