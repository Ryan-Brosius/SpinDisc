using System;
using UnityEngine;
using UnityEngine.Events;

public class CritterHealth : MonoBehaviour
{
    public float MaxHealth {  get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public event UnityAction<Damage> OnDamaged;
    public event UnityAction OnDied;

    public void Initialize(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(Damage damage)
    {
        if (IsDead) return;

        CurrentHealth -= damage.Amount;
        OnDamaged?.Invoke(damage);

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            OnDied?.Invoke();
        }
    }
}
