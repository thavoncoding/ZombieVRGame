using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour
{
    private Animator animator;
    private bool isDying = false;
    public Transform target;
    public float stopDistance = 1.5f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        if (isDying || target == null || agent == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath(); // Stop moving if close enough
        }
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        // Step 1: Stop the agent first
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false; // Disable completely to prevent any unexpected movement
        }

        // Step 2: Play animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Step 3: Destroy after animation
        Destroy(gameObject, 2f); // Adjust based on your animation length
    }

}
