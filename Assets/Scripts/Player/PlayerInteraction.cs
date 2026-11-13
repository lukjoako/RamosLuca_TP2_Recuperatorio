using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción por Raycast")]
    [SerializeField] private Transform rayOrigin; // Asignar cámara o punto frontal del Player
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private void Awake()
    {
        if (rayOrigin == null)
            rayOrigin = Camera.main != null ? Camera.main.transform : transform;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        Debug.DrawRay(rayOrigin.position, rayOrigin.forward * interactDistance, Color.cyan);

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }
}