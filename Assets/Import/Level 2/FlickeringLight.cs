using UnityEngine;
using System.Collections;

public class ConsistentFlickeringLight : MonoBehaviour
{
    public Light pointLight; // Assign your Point Light here
    public AudioClip electricBuzzClip; // Drag your MP3 or audio clip here
    public float maxIntensity = 25f; // Maximum intensity when light is on
    public float minIntensity = 4f; // Minimum intensity when light is off
    public float flickerDuration = 0.05f; // Duration of a single flicker cycle
    public float delayBetweenFlickers = 0.1f; // Delay between each flicker cycle

    private AudioSource audioSource;

    void Start()
    {
        // Assign Point Light component if not manually set
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }

        // Set up AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        if (electricBuzzClip != null)
        {
            audioSource.clip = electricBuzzClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.Play();
        }

        // Start the flickering effect
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            // Randomize intensity for flickering effect
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerDuration);
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(delayBetweenFlickers);
        }
    }
}
