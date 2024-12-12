using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField] LayerMask groundLayer, playerLayer;

    // patrol
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
    }

    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (playerInSight && !playerInAttackRange)
        {
            Chase();
        }
        else if (!playerInSight && !playerInAttackRange)
        {
            Patrol();
        }
        else if (playerInAttackRange)
        {
            Attack();
        }

        UpdateAnimation();
        UpdateRotation();
    }

    void Patrol()
    {
        // Kembalikan kecepatan normal saat patroli
        agent.speed = 1.5f;

        if (!walkPointSet) SearchForDest();
        if (walkPointSet) agent.SetDestination(destPoint);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            walkPointSet = false;
        }
    }

    void Chase()
    {
        // Atur kecepatan menjadi 3 saat mengejar pemain
        agent.speed = 3f;
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        agent.SetDestination(transform.position); // Berhenti saat menyerang
        Debug.Log("Attacking Player!");
    }

    void SearchForDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkPointSet = true;
        }
    }

    void UpdateAnimation()
    {
        // Periksa apakah AI sedang bergerak
        float speed = agent.velocity.magnitude;

        // Atur parameter animasi berdasarkan kecepatan
        animator.SetBool("isWalking", speed > 0.1f && agent.speed < 3f); // Animasi berjalan
        animator.SetBool("isRunning", agent.speed == 3f && speed > 0.1f); // Animasi berlari
    }

    void UpdateRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion correction = Quaternion.Euler(0, 180, 0); // Koreksi orientasi jika diperlukan
            targetRotation *= correction;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
