using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseGameManager : MonoBehaviour
{
    public TextMeshProUGUI LevelText; // text level yg lagi dimainin
    public KeyCode PauseKey = KeyCode.Escape; // key untuk pause game
    bool isPaused;
    public GameObject PauseCanvas;

    private void Update()
    {
        LevelText.text = SceneManager.GetActiveScene().name;

        if (Input.GetKeyUp(PauseKey))
        {
            isPaused = !isPaused;

            if (!isPaused)
            {
                ResumeGame();
            }
        }

        if (isPaused)
        {
            PauseCanvas.SetActive(true);
            Time.timeScale = 0;
            AudioListener.pause = true;

            // Biar cursornya bisa digerakkin
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // matiin gerakan player
            FindObjectOfType<FirstPersonController>().enabled = false;
        }
        else
        {
            PauseCanvas.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        // matiin pause
        isPaused = false;

        Time.timeScale = 1;
        AudioListener.pause = false;

        // Biar cursornya bisa digerakkin
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // nyalain gerakan player
        FindObjectOfType<FirstPersonController>().enabled = true;

    }

    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // matiin pause
        isPaused = false;

        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene("Main Menu");
    }
}
