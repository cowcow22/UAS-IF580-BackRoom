using UnityEngine;
using UnityEngine.AI; // Jika menggunakan NavMeshAgent

public class Target : MonoBehaviour
{
    public float health = 1500f;

    [Header("Audio")]
    public AudioSource audioSource;   // AudioSource untuk memainkan suara
    public AudioClip deathSound;      // Klip suara mati

    [Header("AI Movement")]
    public NavMeshAgent navMeshAgent;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Nonaktifkan gerakan AI
        DisableAIMovement();

        // Mainkan suara mati sebelum menghancurkan objek
        PlayDeathSound();

        // Panggil metode statis untuk memberi tahu bahwa SCP mati
        SCPCountManager.NotifySCPDied();

        // Hancurkan GameObject ini setelah suara selesai dimainkan
        Destroy(gameObject, deathSound != null ? deathSound.length : 0f);
    }

    void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    void DisableAIMovement()
    {
        // Nonaktifkan NavMeshAgent jika digunakan
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }
    }
}
