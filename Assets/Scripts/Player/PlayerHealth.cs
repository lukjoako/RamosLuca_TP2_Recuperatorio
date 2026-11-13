using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float immunityDuration = 1.2f;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsFullHealth => CurrentHealth >= maxHealth;
    public bool IsInvisible { get; set; } // Lista para Opción B si la elegís

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    private float lastDamageTime = -Mathf.Infinity;

    private void Start()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount, bool ignoreImmunity = false)
    {
        if (CurrentHealth <= 0) return;
        if (!ignoreImmunity && Time.time < lastDamageTime + immunityDuration) return;

        lastDamageTime = Time.time;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        Debug.Log($"[PlayerHealth] Daño recibido: -{amount}. Vida actual: {CurrentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (CurrentHealth <= 0 || IsFullHealth) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        Debug.Log($"[PlayerHealth] Curación: +{amount}. Vida actual: {CurrentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}