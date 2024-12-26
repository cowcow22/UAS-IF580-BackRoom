using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class SCPCountManager : MonoBehaviour
{
    public static SCPCountManager Instance; // Singleton
    private int scpCount; // Jumlah SCP yang masih hidup
    public TextMeshProUGUI scpCountText; // Referensi ke UI Text
    public TextMeshProUGUI LevelText;
    public string scpTag = "SCP"; // Tag yang digunakan untuk mendeteksi SCP

    // Event untuk dipanggil ketika jumlah SCP berkurang
    public static event Action OnSCPDied;

    public GameObject winUICanvas; // Canvas WinUI yang muncul ketika game selesai
    private bool isWinUIActive = false; // Status untuk mendeteksi apakah WinUI aktif
    private FirstPersonController playerController; // Referensi ke FPSController

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ambil referensi FirstPersonController
        playerController = FindObjectOfType<FirstPersonController>();

        // Hitung jumlah awal SCP berdasarkan tag
        scpCount = GameObject.FindGameObjectsWithTag(scpTag).Length;
        Debug.Log("Jumlah SCP ditemukan: " + scpCount);
        UpdateSCPCountUI();

        // Berlangganan ke event SCP mati
        OnSCPDied += DecreaseSCPCount;

        // Pastikan WinUI tidak aktif saat awal
        if (winUICanvas != null)
        {
            winUICanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Perbarui jumlah SCP
        scpCount = GameObject.FindGameObjectsWithTag(scpTag).Length;
        UpdateSCPCountUI();
        LevelText.text = SceneManager.GetActiveScene().name; // Update level text

        // Jika WinUI aktif, blokir kontrol pemain
        if (isWinUIActive)
        {
            return;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe dari event saat objek dihancurkan
        OnSCPDied -= DecreaseSCPCount;
    }

    private void DecreaseSCPCount()
    {
        scpCount = Mathf.Max(0, scpCount - 1); // Pastikan tidak negatif
        Debug.Log("SCP mati. Jumlah SCP sekarang: " + scpCount);
        UpdateSCPCountUI();

        // Jika semua SCP mati, tampilkan WinUI
        if (scpCount <= 0)
        {
            ActivateWinUI();
        }
    }

    private void UpdateSCPCountUI()
    {
        if (scpCountText != null)
        {
            scpCountText.text = "SCPs Left: " + scpCount;
        }
    }

    // Metode statis untuk memanggil event dari luar kelas
    public static void NotifySCPDied()
    {
        OnSCPDied?.Invoke();
    }

    private void ActivateWinUI()
    {
        if (winUICanvas != null)
        {
            winUICanvas.SetActive(true); // Tampilkan Canvas WinUI
            isWinUIActive = true; // Tandai bahwa WinUI aktif

            // Nonaktifkan kontrol player saat WinUI aktif
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // Tampilkan dan bebaskan kursor
            Cursor.lockState = CursorLockMode.None; // Bebaskan kursor
            Cursor.visible = true; // Tampilkan kursor
        }
    }
}
