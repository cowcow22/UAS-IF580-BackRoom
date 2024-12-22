using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCP096AI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    BoxCollider boxCollider;
    [SerializeField] LayerMask groundLayer, playerLayer;
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;
    bool isPlayerLookingAtAI = false;
    bool isChasing = false;

    [SerializeField] AudioSource chaseAudioSource;
    [SerializeField] AudioSource attackAudioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("FPSController");
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (!isChasing)
        {
            CheckIfPlayerIsLooking();
            if (isPlayerLookingAtAI)
            {
                if (playerInAttackRange)
                {
                    animator.SetBool("attack", true);
                    Attack();
                }
                else if (!playerInAttackRange)
                {
                    animator.SetBool("attack", false);
                }
                StartCoroutine(StartChase());
            }
        }


    }

    void CheckIfPlayerIsLooking()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
        {
            if (hit.collider == boxCollider)
            {
                isPlayerLookingAtAI = true;
                animator.SetBool("idle", false);
                animator.SetBool("chasing", true);

            }
            else
            {
                isPlayerLookingAtAI = false;
                animator.SetBool("idle", true);
                animator.SetBool("chasing", false);

            }
        }
        else
        {

            animator.SetBool("idle", true);
            animator.SetBool("chasing", false);
            isPlayerLookingAtAI = false;
        }
    }

    IEnumerator StartChase()
    {
        isChasing = true;
        float chaseDuration = 5f;
        float chaseTimer = 0f;

        while (chaseTimer < chaseDuration)
        {
            animator.SetBool("idle", false);
            animator.SetBool("chasing", true);
            agent.isStopped = true;

            ChasePlayer();
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        isChasing = false;

        // After 10 seconds, check if the player is still looking
        CheckIfPlayerIsLooking();
        if (!isPlayerLookingAtAI)
        {
            animator.SetBool("idle", true);
            animator.SetBool("chasing", false);
            StopMoving();
        }
    }

    void Attack()
    {
        agent.SetDestination(transform.position); // Berhenti saat menyerang

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            agent.isStopped = true;
            animator.SetBool("attack", true);
            Debug.Log("SCP096 Attacking Player!");

            if (attackAudioSource != null && !attackAudioSource.isPlaying)
            {
                attackAudioSource.volume = 1f; // Volume tetap untuk attack
                attackAudioSource.Play();
            }


        }
    }


    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= sightRange)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void StopMoving()
    {
        agent.SetDestination(transform.position);
    }
}
