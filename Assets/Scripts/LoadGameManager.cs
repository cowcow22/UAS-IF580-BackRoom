using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LoadLevelText; // preview level buat di load
    [SerializeField] Button LoadButton; // button buat load level

    void Awake()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("loaded level")))
        {
            LoadButton.interactable = false;
        }
        else
        {
            LoadButton.interactable = true;
            LoadLevelText.text = PlayerPrefs.GetString("loaded level", " ");
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("loaded level"));
    }
}
