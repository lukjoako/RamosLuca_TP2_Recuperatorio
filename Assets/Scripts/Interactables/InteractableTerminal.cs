using UnityEngine;

public class InteractableTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorToOpen; // Opcional: arrastrá una puerta para abrirla/desactivarla
    [SerializeField] private int bonusPoints = 10;

    private bool used = false;

    public void Interact()
    {
        if (used) return;
        used = true;

        Debug.Log("[Terminal] ¡Terminal activada por Raycast!");
        GameManager.Instance?.AddScore(bonusPoints);

        if (doorToOpen != null)
        {
            doorToOpen.SetActive(false);
        }
    }
}