using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPatrol : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator; // Referensi ke Animator
    [SerializeField] LayerMask groundLayer, playerLayer;

    // patrol
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Ambil referensi Animator
        player = GameObject.Find("FPSController");
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        UpdateAnimation();
        UpdateRotation();
    }

    void Patrol()
    {
        if (!walkPointSet) SearchForDest();
        if (walkPointSet) agent.SetDestination(destPoint);
        // if (Vector3.Distance(transform.position, destPoint) < 10) walkPointSet = false;

        // Periksa apakah AI telah mencapai tujuan menggunakan stoppingDistance
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            walkPointSet = false;
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
        // Periksa apakah AI sedang bergerak
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving); // Atur parameter isWalking
    }

    void UpdateRotation()
    {
        // Jika AI bergerak, update rotasi untuk menghadap ke arah tujuan
        if (agent.velocity.sqrMagnitude > 0.1f) // Kecepatan lebih dari 0
        {
            Vector3 direction = agent.velocity.normalized; // Arah gerakan
            Quaternion targetRotation = Quaternion.LookRotation(direction); // Rotasi target

            // Koreksi rotasi jika model tidak menghadap ke depan (misalnya perlu rotasi 180 derajat)
            Quaternion correction = Quaternion.Euler(0, 180, 0); // Ganti 180 dengan sudut yang sesuai jika model Anda salah orientasi
            targetRotation *= correction;

            // Terapkan rotasi secara halus
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

}
