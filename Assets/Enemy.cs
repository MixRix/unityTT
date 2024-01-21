using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [SerializeField] Transform spawnBulletPosition;
    public float health = 60f;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    public float sightRange;
    public float attackRange;
    
    public bool playerInSightRange;
    public bool playerInAttackRange;

    public Animator animator;

    public bool isAliveEnemy = true;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();


    }
    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
        animator.SetBool("isWalking", true);
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
        animator.SetBool("isWalking", playerInSightRange);
        animator.SetBool("isShooting", playerInAttackRange);
    }
    void ChasePlayer()
    {
        if (health > 0)
        {
        agent.SetDestination(player.position);
        animator.SetBool("isWalking", playerInSightRange);
        animator.SetBool("isShooting", playerInAttackRange);

        }
        
    }
    void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        
        lookRotation.x = 0;
        lookRotation.z = 0;

        
        transform.rotation = lookRotation;

        if (!alreadyAttacked && health > 0)
        {
            Rigidbody rb = Instantiate(projectile, spawnBulletPosition.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 24f, ForceMode.Impulse);
            
            Destroy(rb.gameObject, 3f);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 1);
        }
        animator.SetBool("isShooting", playerInAttackRange);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), .5f);
            animator.SetBool("isAlive", false);
            isAliveEnemy = false;
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    

}