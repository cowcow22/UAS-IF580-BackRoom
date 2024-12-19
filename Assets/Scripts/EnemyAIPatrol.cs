using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyAIPatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    MeshCollider meshCollider;
    public TextMeshProUGUI LevelText;
    [SerializeField] LayerMask groundLayer, playerLayer;

    [SerializeField] GameObject gameOverCanvas; // Referensi Canvas GameOver

    [SerializeField] AudioSource patrolAudioSource;
    [SerializeField] AudioSource chaseAudioSource;
    [SerializeField] AudioSource attackAudioSource;

    // Patrol
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    bool isJumpingToPlayer = false; // Status AI melompat ke pemain

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController");
        meshCollider = GetComponent<MeshCollider>();

        // Pastikan Canvas GameOver tidak aktif saat start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (patrolAudioSource != null)
        {
            patrolAudioSource.loop = true;
            patrolAudioSource.Play();
            patrolAudioSource.volume = 0;
        }

        if (chaseAudioSource != null)
        {
            chaseAudioSource.loop = true;
            chaseAudioSource.Play();
            chaseAudioSource.volume = 0;
        }
    }

    void Update()
    {
        if (isJumpingToPlayer) return; // Hentikan semua perilaku lain jika AI sedang melompat ke pemain

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
        UpdateAudio();
        LevelText.text = SceneManager.GetActiveScene().name; // Update level text
    }

    void Patrol()
    {
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
        agent.speed = 3.5f;
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        agent.SetDestination(transform.position); // Berhenti saat menyerang

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            animator.SetTrigger("Attack");
            Debug.Log("Attacking Player!");

            if (attackAudioSource != null && !attackAudioSource.isPlaying)
            {
                attackAudioSource.volume = 1f; // Volume tetap untuk attack
                attackAudioSource.Play();
            }

            // Panggil fungsi lompat ke pemain setelah animasi serangan dipicu
            if (!isJumpingToPlayer)
            {
                StartCoroutine(JumpToPlayer());
            }
        }
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
        float speed = agent.velocity.magnitude;
        animator.SetBool("isWalking", speed > 0.1f && agent.speed < 3f);
        animator.SetBool("isRunning", agent.speed == 3.5f && speed > 0.1f);

        if (!playerInAttackRange)
        {
            animator.ResetTrigger("Attack");
        }
    }

    void UpdateRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion correction = Quaternion.Euler(0, 180, 0);
            targetRotation *= correction;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void UpdateAudio()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (playerInSight && !playerInAttackRange)
        {
            if (patrolAudioSource != null) patrolAudioSource.volume = 0;
            if (chaseAudioSource != null) chaseAudioSource.volume = Mathf.Clamp(0.5f + (1f - distanceToPlayer / sightRange) * 0.5f, 0.5f, 1f);
        }
        else if (!playerInSight)
        {
            if (chaseAudioSource != null) chaseAudioSource.volume = 0;
            if (patrolAudioSource != null) patrolAudioSource.volume = Mathf.Clamp(0.5f + (1f - distanceToPlayer / sightRange) * 0.5f, 0.5f, 1f);
        }
        else
        {
            if (chaseAudioSource != null) chaseAudioSource.volume = 0;
            if (patrolAudioSource != null) patrolAudioSource.volume = 0;
        }
    }

    // Fungsi untuk memproses tabrakan dengan pemain
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (!isJumpingToPlayer)
            {
                StartCoroutine(JumpToPlayer());
            }
        }
    }

    IEnumerator JumpToPlayer()
    {
        isJumpingToPlayer = true;
        agent.isStopped = true; // Hentikan pergerakan AI

        // Panggil animasi lompat
        animator.SetTrigger("isJumping");
        Debug.Log("Jumping");

        // Tunggu hingga animasi selesai sebelum melanjutkan ke game over
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Setelah lompatan selesai, tampilkan Canvas GameOver
        if (gameOverCanvas != null)
        {
            animator.ResetTrigger("isJumping");
            FindObjectOfType<FirstPersonController>().enabled = false; // Biar player ga bisa gerak
            Cursor.visible = true; // Biar cursor muncul
            Cursor.lockState = CursorLockMode.None; // Biar cursor bisa digerakin
            gameOverCanvas.SetActive(true);

            // Disable input untuk menekan tombol 'Esc' dengan memblokir fungsinya
            StartCoroutine(DisableEscapeKey());
        }

        Debug.Log("Game Over!");
    }

    IEnumerator DisableEscapeKey()
    {
        // Mencegah fungsi Escape selama Game Over
        while (gameOverCanvas.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Jangan lakukan apa-apa jika tombol 'Esc' ditekan
                yield return null;
            }
            yield return null;
        }
    }
}
