using UnityEngine;

public class InvisibilityPowerUp : MonoBehaviour
{
    [SerializeField] private float duration = 6f;
    [SerializeField] private float rotateSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerHealth health))
        {
            health.ActivateInvisibility(duration);
            Destroy(gameObject);
        }
    }
}