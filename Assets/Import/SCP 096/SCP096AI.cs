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
    [SerializeField] AudioSource idleAudioSource;
    [SerializeField] AudioSource chaseStartAudioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("FPSController");
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();

        // Pastikan audio idle berjalan saat awal
        if (idleAudioSource != null)
        {
            idleAudioSource.loop = true;
            idleAudioSource.Play();
        }
    }

    void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        // Update volume idle berdasarkan jarak
        UpdateIdleAudioVolume();
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

    void UpdateIdleAudioVolume()
    {
        if (idleAudioSource != null)
        {
            // Hitung jarak antara AI dan player
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Atur parameter jarak dan volume
            float maxDistance = 50f; // Jarak maksimum di mana suara masih terdengar
            float minVolume = 0.5f;  // Volume minimum ketika player jauh
            float maxVolume = 5f;    // Volume maksimum ketika player dekat

            // Gunakan fungsi eksponensial untuk membuat suara lebih keras ketika mendekat
            float volume = Mathf.Clamp((1 - Mathf.Pow(distance / maxDistance, 2)), minVolume / maxVolume, maxVolume);

            // Set volume ke audio source
            idleAudioSource.volume = volume;
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
                // Berhenti memainkan suara Idle dan mulai mengejar
                if (idleAudioSource != null && idleAudioSource.isPlaying)
                {
                    idleAudioSource.Stop();
                }

                if (chaseStartAudioSource != null && !chaseStartAudioSource.isPlaying)
                {
                    chaseStartAudioSource.Play();
                }
            }
            else
            {
                isPlayerLookingAtAI = false;
                animator.SetBool("idle", true);
                animator.SetBool("chasing", false);
                // Pastikan suara Idle berjalan kembali
                if (idleAudioSource != null && !idleAudioSource.isPlaying)
                {
                    idleAudioSource.Play();
                }
            }
        }
        else
        {

            animator.SetBool("idle", true);
            animator.SetBool("chasing", false);
            isPlayerLookingAtAI = false;
            // Pastikan suara Idle berjalan kembali
            if (idleAudioSource != null && !idleAudioSource.isPlaying)
            {
                idleAudioSource.Play();
            }
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
            // agent.isStopped = true;

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
            // Pastikan suara Idle kembali diputar
            if (idleAudioSource != null && !idleAudioSource.isPlaying)
            {
                idleAudioSource.Play();
            }
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
