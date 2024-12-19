using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public GameObject CursorHover;       // UI Hover indicator
    public GameObject BookInformation;  // Canvas to display book information
    private bool isBookOpen = false;    // Track whether the book is currently open

    private void OnMouseOver()
    {
        if (PlayerCasting.DistanceFromTarget < 4) // If the player IS close enough to the Book
        {
            CursorHover.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E)) // If the player presses E
            {
                ToggleBookInformation();
            }
        }
        else // If the player is NOT close enough to the Book
        {
            CursorHover.SetActive(false);
        }
    }

    private void OnMouseExit() // Activates when the player looks away from the Book
    {
        CursorHover.SetActive(false);
    }

    private void ToggleBookInformation()
    {
        isBookOpen = !isBookOpen; // Toggle the state of the book
        BookInformation.SetActive(isBookOpen); // Show or hide the BookInformation canvas
    }
}
