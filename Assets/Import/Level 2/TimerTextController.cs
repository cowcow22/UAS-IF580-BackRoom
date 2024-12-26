using UnityEngine;
using TMPro;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class TimerTextController : MonoBehaviour
{
    public TextMeshProUGUI Timer; // Assign your TextMeshPro UI element here
    public float startTime = 60f; // Starting time in seconds
    public GameObject gameOverCanvas; // Assign your GameOverCanvas here

    private float currentTime;

    void Start()
    {
        currentTime = startTime; // Initialize timer
        UpdateTimerText(); // Update the initial text
        gameOverCanvas.SetActive(false); // Ensure GameOverCanvas is hidden at the start
    }

    void Update()
    {
        if (gameOverCanvas.activeSelf)
        {
            return; // Ignore input if game is over
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Decrease time
            UpdateTimerText();
        }
        else
        {
            currentTime = 0; // Ensure it doesn't go below 0
            UpdateTimerText();
            TriggerGameOver(); // Show GameOverCanvas
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(currentTime % 60); // Calculate seconds

        Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format as MM:SS
    }

    void TriggerGameOver()
    {
        Debug.Log("TriggerGameOver called");
        gameOverCanvas.SetActive(true); // Show the GameOverCanvas
        
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
        enabled = false; // Stop updating the timer
        StartCoroutine(DisableEscapeKey()); // Disable Escape key during Game Over

        // Disable player controls or other interactive components
        var playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerController.enabled = false;
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
