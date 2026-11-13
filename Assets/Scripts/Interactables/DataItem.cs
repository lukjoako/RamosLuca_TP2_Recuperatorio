using UnityEngine;

public class DataItem : MonoBehaviour
{
    [SerializeField] private int points = 10;
    [SerializeField] private float rotateSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.AddScore(points);
            Destroy(gameObject);
        }
    }
}