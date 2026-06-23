using UnityEngine;

public class Squirrel : Critter
{
    [SerializeField] private Transform target;

    private void Start()
    {
        SetTarget(target);
    }
}
