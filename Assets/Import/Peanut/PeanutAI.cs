using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PeanutAI : MonoBehaviour
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

    [SerializeField] private GameObject gameOverCanvas;
    private bool isGameOver = false;
    public TextMeshProUGUI LevelText;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chaseSound;        // Suara ketika AI mengejar
    public AudioClip lookAtSound;      // Suara ketika AI dilihat
    public AudioClip startChaseSound;  // Suara ketika AI mulai mengejar
    public AudioClip attackSound; // Suara ketika AI menyerang pemain


    private bool isChasingSoundPlaying = false;
    private bool isLookAtSoundPlaying = false;
    private bool isStartChaseSoundPlayed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("FPSController");
        boxCollider = GetComponent<BoxCollider>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        LevelText.text = SceneManager.GetActiveScene().name; // Update level text
    }

    void Update()
    {
        if (isGameOver) return; // Hentikan semua logika jika game over

        CheckIfPlayerIsLooking();

        if (isPlayerLookingAtAI)
        {
            StopMoving();
            PlayLookAtSound();
        }
        else
        {
            ChasePlayer();
            PlayStartChaseSound();
        }

        CheckAttackPlayer();
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
            PlayStartChaseSound();
        }
    }

    void CheckAttackPlayer()
    {



        // Jika AI berada dalam jangkauan serangan
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        // make the camera lookat to scp, with y position same as player
        Camera.main.transform.LookAt(transform.position + new Vector3(0, Camera.main.transform.position.y - 1, 0));
        isGameOver = true;

        // Hentikan pergerakan AI dan pemain
        agent.isStopped = true;

        var playerController = player.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Mainkan suara serangan jika ada
        if (attackSound != null)
        {
            audioSource.clip = attackSound;
            audioSource.loop = false; // Suara ini tidak diulang
            audioSource.Play();
        }

        // Tampilkan canvas game over setelah sedikit delay
        StartCoroutine(ShowGameOverCanvasAfterDelay());
    }

    // Coroutine untuk menunggu hingga suara serangan selesai (opsional)
    IEnumerator ShowGameOverCanvasAfterDelay()
    {
        if (attackSound != null)
        {
            yield return new WaitForSeconds(attackSound.length); // Tunggu suara selesai
        }

        if (gameOverCanvas != null)
        {
            Cursor.visible = true; // Biar cursor muncul
            Cursor.lockState = CursorLockMode.None; // Biar cursor bisa digerakin
            gameOverCanvas.SetActive(true);
            StartCoroutine(DisableEscapeKey()); // Disable Escape key during Game Over
        }
    }

    void StopMoving()
    {
        agent.SetDestination(transform.position);
        StopChaseSound();
    }

    void OnMouseOver()
    {
        isPlayerLookingAtAI = true;
    }

    void OnMouseExit()
    {
        isPlayerLookingAtAI = false;
    }

    void PlayStartChaseSound()
    {
        // Hanya mainkan suara startChaseSound ketika AI mulai mengejar dan belum dimainkan
        if (Vector3.Distance(transform.position, player.transform.position) <= sightRange && !isStartChaseSoundPlayed)
        {
            audioSource.clip = startChaseSound;
            audioSource.volume = 1f;
            audioSource.loop = false; // Suara ini hanya dimainkan sekali
            audioSource.Play();
            isStartChaseSoundPlayed = true;

            // Setelah selesai memutar suara "start chase", lanjutkan dengan suara "chase"
            StartCoroutine(PlayChaseSoundAfterStartChase());
        }
    }

    IEnumerator PlayChaseSoundAfterStartChase()
    {
        // Tunggu hingga suara startChaseSound selesai diputar
        yield return new WaitForSeconds(startChaseSound.length);

        // Pastikan suara chase hanya diputar setelah suara start chase selesai
        PlayChaseSound();
    }

    void PlayChaseSound()
    {
        // Mainkan suara chase hanya jika AI sedang mengejar pemain
        if (!isChasingSoundPlaying)
        {
            audioSource.clip = chaseSound;
            audioSource.loop = true; // Suara ini berulang
            audioSource.Play();
            isChasingSoundPlaying = true;
        }
    }

    void StopChaseSound()
    {
        // Matikan suara chase jika AI berhenti mengejar
        if (isChasingSoundPlaying)
        {
            audioSource.Stop();
            isChasingSoundPlaying = false;
        }
    }

    void PlayLookAtSound()
    {
        // Mainkan suara saat dilihat hanya jika suara lainnya tidak diputar
        if (!audioSource.isPlaying || !isLookAtSoundPlaying)
        {
            audioSource.clip = lookAtSound;
            audioSource.loop = false; // Tidak perlu diulang
            audioSource.Play();
            isLookAtSoundPlaying = true;

            // Reset status lainnya agar suara mengejar bisa diputar lagi jika AI kembali mengejar
            isStartChaseSoundPlayed = false;
            isChasingSoundPlaying = false;
        }
    }

    void OnDestroy()
    {
        // Hentikan semua audio yang terkait dengan AI
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    IEnumerator DisableEscapeKey()
        {
            // Mencegah fungsi Escape selama Game Over
            while (gameOverCanvas.activeSelf)
            {
                GameObject pauseGameManagerCanvas = GameObject.Find("Pause Game Manager");
                if (pauseGameManagerCanvas != null)
                {
                    pauseGameManagerCanvas.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Jangan lakukan apa-apa jika tombol 'Esc' ditekan
                    yield return null;
                }
                yield return null;
            }
        }
}
