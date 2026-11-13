using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemySentinel : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float waitTimeAtPoint = 1.5f;

    [Header("Detección y Persecución")]
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private float chaseSpeed = 5.5f;

    [Header("Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 20;

    private NavMeshAgent agent;
    private Transform player;
    private PlayerHealth playerHealth;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private Vector3 startPosition;
    private float stunTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        GoToNextPatrolPoint();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // Si está aturdido (Opción A), queda congelado
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;

        bool canSeePlayer = player != null
            && (playerHealth == null || !playerHealth.IsInvisible)
            && Vector3.Distance(transform.position, player.position) <= detectionRadius;

        if (canSeePlayer)
        {
            ChaseAndAttack();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            // Patrulla aleatoria alrededor de su posición inicial
            Vector3 randomDir = Random.insideUnitSphere * patrolRadius + startPosition;
            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    private void ChaseAndAttack()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // Daño si toca al jugador directamente
    private void OnTriggerStay(Collider other)
    {
        if (stunTimer > 0f) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage);
        }
    }

    // Método público para la Opción A (Arma de aturdimiento)
    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        Debug.Log($"[EnemySentinel] {name} aturdido por {duration} segundos.");
    }

    // Visualización de radios en la ventana Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}