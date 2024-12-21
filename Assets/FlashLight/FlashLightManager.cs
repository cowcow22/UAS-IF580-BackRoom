using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlashLightState
{
    On, // The flashlight is on
    Off, // The flashlight is off
    Dead // The flashlight is dead (No battery)
}

[RequireComponent(typeof(AudioSource))]
public class FlashLightManager : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [Tooltip("The rate of battery drain")][Range(0.0f, 2f)][SerializeField] float batteryDrainRate = 0.5f;
    [Tooltip("The amount of battery the flashlight has")][SerializeField] int startBattery = 100;
    [Tooltip("The amount of battery the player has")] public int currentBattery;
    [Tooltip("The current state of flashlight")] public FlashLightState state;
    [Tooltip("Is the flashlight on?")] private bool flasLightIsOn;
    [Tooltip("The key that required to pressed on/off")][SerializeField] KeyCode ToggleKey = KeyCode.F;

    [Header("Flashlight Events")]
    [Tooltip("The event that will be called when the flashlight is turned on")][SerializeField] GameObject FlashLightLight;
    [Tooltip("Audio")][SerializeField] AudioClip FlashLightOn_FX, FlashLightOff_FX;
    private AudioSource audioSource;

    private void Start()
    {
        currentBattery = startBattery; // Set the current battery to the start battery
        InvokeRepeating(nameof(LoseBattery), 0, batteryDrainRate); // Start draining the battery
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(ToggleKey)) // If the player pressed the toggle key
        {
            ToggleFlashLight(); // Toggle the flashlight
        }

        // Check the flashlight state
        if (state == FlashLightState.On) // If the flashlight is on
        {
            FlashLightLight.SetActive(true); // Turn on the flashlight light
        }
        else if (state == FlashLightState.Off) // If the flashlight is off
        {
            FlashLightLight.SetActive(false); // Turn off the flashlight light
        }
        else if (state == FlashLightState.Dead) // If the flashlight is dead
        {
            FlashLightLight.SetActive(false); // Turn off the flashlight light
        }

        // handle the flashlight state dead
        if (currentBattery <= 0) // If the current battery is less than or equal to 0
        {
            currentBattery = 0; // Set the current battery to 0
            state = FlashLightState.Dead; // Set the state to dead
            flasLightIsOn = false; // Turn off the flashlight
        }
    }

    // Gaining flashlight battery
    public void GainBattery(int amount)
    {
        if (currentBattery == 0)
        {
            state = FlashLightState.On;
            flasLightIsOn = true;
        }
        if (currentBattery + amount > startBattery) currentBattery = startBattery; // If the current battery + amount is greater than the start battery, set the current battery to the start battery
        else currentBattery += amount; // Add the amount to the current battery
    }

    // Losing flashlight battery
    private void LoseBattery()
    {
        if (state == FlashLightState.On) currentBattery--; // If the flashlight is on
    }

    // Toggle the flashlight on/off
    private void ToggleFlashLight()
    {
        flasLightIsOn = !flasLightIsOn; // Toggle the flashlight
        if (state == FlashLightState.Dead) flasLightIsOn = false; // If the flashlight is dead, turn it off

        // Play the flashlight sound
        if (flasLightIsOn)
        {
            if (FlashLightOn_FX != null) audioSource.PlayOneShot(FlashLightOn_FX); // Play the flashlight on sound
            state = FlashLightState.On; // If the flashlight is on, set the state to on
        }
        else
        {
            if (FlashLightOff_FX != null) audioSource.PlayOneShot(FlashLightOff_FX); // Play the flashlight off sound
            state = FlashLightState.Off; // If the flashlight is off, set the state to off
        }
    }
}
