using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCP096AI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    BoxCollider boxCollider;
    [SerializeField] LayerMask groundLayer, playerLayer;
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;
    bool isPlayerLookingAtAI = false;
    bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("FPSController");
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (!isChasing)
        {
            CheckIfPlayerIsLooking();
            if (isPlayerLookingAtAI)
            {
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
            }
            else
            {
                isPlayerLookingAtAI = false;
            }
        }
        else
        {
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
            ChasePlayer();
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        isChasing = false;

        // After 10 seconds, check if the player is still looking
        CheckIfPlayerIsLooking();
        if (!isPlayerLookingAtAI)
        {
            StopMoving();
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
