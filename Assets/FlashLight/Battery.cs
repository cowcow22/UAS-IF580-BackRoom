using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Battery Settings")]
    [Tooltip("The amount of battery that the player can collect")][SerializeField] int BatteryWeight;

    [Tooltip("Key to collect the battery")][SerializeField] KeyCode CollectKey = KeyCode.E;

    [Header("Battery Events")]
    [Tooltip("The object that is shown when the player hover")][SerializeField] GameObject[] HoverObject;

    public void OnMouseOver()
    {
        foreach (GameObject obj in HoverObject)
        {
            obj.SetActive(true);
        }
        if (Input.GetKeyDown(CollectKey))
        {
            FindObjectOfType<FlashLightManager>().GainBattery(BatteryWeight); // Add the battery weight to the flashlight

            Destroy(this.gameObject); // Destroy the battery
        }
    }

    public void OnMouseExit()
    {
        foreach (GameObject obj in HoverObject)
        {
            obj.SetActive(false);
        }
    }
}
