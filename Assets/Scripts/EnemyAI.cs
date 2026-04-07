using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    EnemyHealth health;
    Transform target;

    [Header("Audio")]
    AudioClip zombieRoarSound;
    AudioSource audioSource;

    void Start()
    {
        zombieRoarSound = Resources.Load<AudioClip>("zombie_roar");
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            target = player.transform;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Make it 3D sound
        }
    }

    void Update()
    {
        if (health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
            return;
        }

        if (target == null) return;

        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            if (!isProvoked) 
            {
                PlayZombieRoar();
            }
            isProvoked = true;
        }
    }

    public void OnDamageTaken()
    {
        if (!isProvoked) 
        {
            PlayZombieRoar();
        }
        isProvoked = true;
    }

    private void PlayZombieRoar()
    {
        if (zombieRoarSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(zombieRoarSound);
        }
    }

    private void EngageTarget()
    {
        FaceTarget();
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetTrigger("move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack", true);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
