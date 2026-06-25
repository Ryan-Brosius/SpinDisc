using UnityEngine;

// Bruh who made this again - Ryan xd
public struct Damage
{
    public Damage(float amount, GameObject source)
    {
        Amount = amount;
        Source = source;
    }

    public float Amount;
    public GameObject Source;
}
