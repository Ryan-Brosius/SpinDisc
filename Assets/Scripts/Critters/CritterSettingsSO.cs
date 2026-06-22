using UnityEngine;

[CreateAssetMenu(fileName = "CritterSettingsSO", menuName = "Scriptable Objects/CritterSettingsSO")]
public class CritterSettingsSO : ScriptableObject
{
    [Header("Health")]
    public float Health = 5f;

    [Header("Movement")]
    public float MaxSpeed = 5f;
    // TODO: Add acceleration data here

    // TODO: Add target data here (enums?)
}
