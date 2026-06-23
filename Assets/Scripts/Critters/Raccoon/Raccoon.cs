using Unity.VisualScripting;
using UnityEngine;

public class Racoon : Critter
{
    [SerializeField] private Transform target;

    private void Start()
    {
        SetTarget(target);
    }
}
