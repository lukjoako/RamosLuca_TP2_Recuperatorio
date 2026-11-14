using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.4f; // 40% de la velocidad normal

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController controller))
        {
            controller.SetSpeedMultiplier(slowMultiplier);
            Debug.Log("[Zona Barro] Velocidad reducida.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController controller))
        {
            controller.SetSpeedMultiplier(1f);
            Debug.Log("[Zona Barro] Velocidad restablecida.");
        }
    }
}