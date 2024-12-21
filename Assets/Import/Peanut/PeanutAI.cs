using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class PeanutAI : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    // Animator animator;
    // MeshCollider meshCollider;
    BoxCollider boxCollider;
    [SerializeField] LayerMask groundLayer, playerLayer;
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;
    bool isPlayerLookingAtAI = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
        // meshCollider = GetComponent<MeshCollider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        CheckIfPlayerIsLooking();

        if (isPlayerLookingAtAI)
        {
            StopMoving();
        }
        else
        {
            ChasePlayer();
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

    void OnMouseOver()
    {
        isPlayerLookingAtAI = true;
    }

    void OnMouseExit()
    {
        isPlayerLookingAtAI = false;
    }
}
