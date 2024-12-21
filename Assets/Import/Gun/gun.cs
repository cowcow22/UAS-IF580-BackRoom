using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 10f;
    public float nextTimeToFire = 0f;

    public int maxAmmo = 30;      // Maximum bullets in one magazine
    public int currentAmmo;      // Current bullets in the magazine
    public float reloadTime = 3.5f; // Time required to reload
    private bool isReloading = false;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Gun Audio")]
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip gunshotSound;  // Gunshot sound effect
    public AudioClip reloadSound;   // Reload sound effect

    [Header("Game Over Settings")]
    public GameObject gameOverCanvas; // Reference to the GameOverCanvas object

    void Start()
    {
        currentAmmo = maxAmmo; // Initialize the magazine with full ammo
    }

    void Update()
    {
        // Jika GameOverCanvas aktif, hentikan semua aksi
        if (gameOverCanvas != null && gameOverCanvas.activeSelf)
        {
            return;
        }

        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R)) // Reload manually with the 'R' key
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--; // Kurangi peluru
        muzzleFlash.Play(); // Putar muzzle flash ulang

        if (gunshotSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunshotSound); // Mainkan suara tembakan
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); // Buat efek impact
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound); // Play reload sound
        }

        yield return new WaitForSeconds(reloadTime); // Wait for reload time

        currentAmmo = maxAmmo; // Refill the magazine
        isReloading = false;
    }
}
