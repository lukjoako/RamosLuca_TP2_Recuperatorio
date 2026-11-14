using System.Collections;
using UnityEngine;

public class HealingStation : MonoBehaviour
{
    [Header("Curación Progresiva")]
    [SerializeField] private int healPerTick = 10;
    [SerializeField] private float healInterval = 1.5f;

    [Header("Carga y Cooldown")]
    [SerializeField] private int maxCharge = 50;
    [SerializeField] private float cooldownDuration = 10f;

    [Header("Retroalimentación Visual")]
    [SerializeField] private Light stationLight;
    [SerializeField] private ParticleSystem stationParticles;

    private int currentCharge;
    private bool isOnCooldown = false;
    private float nextHealTime = 0f;

    private void Start()
    {
        currentCharge = maxCharge;
        SetVisualsActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isOnCooldown || Time.timeScale == 0f) return;
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerHealth health))
        {
            if (health.IsFullHealth || health.CurrentHealth <= 0) return;

            if (Time.time >= nextHealTime)
            {
                nextHealTime = Time.time + healInterval;

                int healToApply = Mathf.Min(healPerTick, currentCharge);
                health.Heal(healToApply);
                currentCharge -= healToApply;

                Debug.Log($"[Estación de Curación] Curando +{healToApply} HP. Carga restante: {currentCharge}/{maxCharge}");

                if (currentCharge <= 0)
                {
                    StartCoroutine(CooldownRoutine());
                }
            }
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        SetVisualsActive(false);
        Debug.Log($"[Estación de Curación] Estación en Recarga ({cooldownDuration}s)...");

        yield return new WaitForSeconds(cooldownDuration);

        currentCharge = maxCharge;
        isOnCooldown = false;
        SetVisualsActive(true);
        Debug.Log("[Estación de Curación] Estación recargada y lista.");
    }

    private void SetVisualsActive(bool active)
    {
        if (stationLight != null)
            stationLight.enabled = active;

        if (stationParticles != null)
        {
            if (active) stationParticles.Play();
            else stationParticles.Stop();
        }
    }
}