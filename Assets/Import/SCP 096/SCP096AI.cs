using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

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
    [SerializeField] AudioSource ManScreaming;
    [SerializeField] GameObject gameOverCanvas;
    private bool gameOverTriggered = false;
    public TextMeshProUGUI LevelText;


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

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        LevelText.text = SceneManager.GetActiveScene().name; // Update level text
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
                    animator.SetBool("stopAttack", false);
                    StartCoroutine(HandleAttack());
                }
                else if (!playerInAttackRange)
                {
                    animator.SetBool("attack", false);
                    animator.SetBool("stopAttack", true);
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
        // AI diam sementara untuk menyelesaikan animasi "panicend"
        isChasing = true;

        // Hentikan pergerakan AI selama animasi berlangsung
        agent.isStopped = true;

        // Tunggu hingga animasi "scp096_skeleton|panicend" selesai
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("scp096_skeleton|panicend"))
        {
            Debug.Log("Waiting for panicend animation...");
            yield return null;
        }

        // Tunggu hingga animasi selesai
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f)
        {
            yield return null;
        }

        Debug.Log("Animation complete. Starting to chase.");

        // Setelah animasi selesai, mulai mengejar pemain
        animator.SetBool("idle", false);
        animator.SetBool("chasing", true);
        agent.isStopped = false;

        // Mulai suara mengejar
        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Play();
            Debug.Log("Chase audio started.");
        }

        while (isChasing)
        {
            ChasePlayer();

            // Periksa apakah jarak ke pemain sudah terlalu jauh untuk mengejar atau player sudah dalam attack range
            if (Vector3.Distance(transform.position, player.transform.position) > sightRange || playerInAttackRange)
            {
                isChasing = false;
            }

            yield return null;
        }

        // Jika pemain tidak lagi terlihat, AI kembali ke keadaan idle
        animator.SetBool("chasing", false);
        animator.SetBool("idle", true);
        agent.isStopped = true;

        // Hentikan suara mengejar
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
        }

        // Periksa ulang apakah pemain masih melihat AI
        CheckIfPlayerIsLooking();
    }

    private IEnumerator HandleAttack()
    {
        // Pastikan animasi serangan dimulai
        animator.SetBool("attack", true);

        // Disable Gun Object in FirstPersonCharacter
        GameObject gun = GameObject.Find("Gun");
        Debug.Log("Gun: " + gun);
        if (gun != null)
        {
            gun.SetActive(false);
        }

        // Nonaktifkan gerakan player
        FindObjectOfType<FirstPersonController>().enabled = false;

        // Buat kamera lock ke scp
        Camera.main.transform.LookAt(transform.position + new Vector3(0, Camera.main.transform.position.y - 1, 0));

        animator.SetBool("stopAttack", false);

        // Tunggu hingga animasi selesai
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("scp096_skeleton|attack1"))
        {
            Debug.Log("Waiting for attack animation...");
            yield return null;
        }

        // Tunggu hingga animasi selesai
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f)
        {
            yield return null;
        }

        Debug.Log("Animation complete. Starting to attack.");

        // Setelah animasi selesai, lakukan serangan
        Attack();
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
                attackAudioSource.volume = 1f;
                ManScreaming.Play();
                attackAudioSource.Play();
            }
            if (!gameOverTriggered)
            {
                GameOver();
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

    void GameOver()
    {
        gameOverTriggered = true;

        // Hentikan pergerakan AI dan pemain
        agent.isStopped = true;

        // Tampilkan canvas GameOver
        if (gameOverCanvas != null)
        {
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

    void OnDestroy()
    {
        // Hentikan semua audio yang terkait dengan AI
        if (idleAudioSource != null)
        {
            idleAudioSource.Stop();
        }
        if (chaseAudioSource != null)
        {
            chaseAudioSource.Stop();
        }
        if (attackAudioSource != null)
        {
            attackAudioSource.Stop();
        }
        if (chaseStartAudioSource != null)
        {
            chaseStartAudioSource.Stop();
        }
    }
}
