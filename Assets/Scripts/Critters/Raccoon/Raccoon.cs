using Unity.VisualScripting;
using UnityEngine;

public class Raccoon : Critter
{
    [SerializeField] private Transform target;

    private void Start()
    {
        SetTarget(target);
    }
}
