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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chaseSound;        // Suara ketika AI mengejar
    public AudioClip lookAtSound;      // Suara ketika AI dilihat
    public AudioClip startChaseSound;  // Suara ketika AI mulai mengejar

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
    }

    void Update()
    {
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
}
