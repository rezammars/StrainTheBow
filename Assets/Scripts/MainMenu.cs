using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Button continueButton;
    public Button playButton;
    public Button quitButton;
    
    [Header("Menu Sections")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    
    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip menuMusic;
    
    [Header("Continue Info")]
    public Text continueInfoText; // Optional: untuk menampilkan info progress
    
    [Header("Background Reference")]
    public SpriteRenderer backgroundSprite; // Reference ke Sprite Renderer background
    
    private AudioSource audioSource;
    private bool hasSaveData = false;
    private int savedWave = 0;
    private int savedScore = 0;
    private int savedLives = 0;

    void Start()
    {
        // Initialize audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Play menu music
        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Setup background (pastikan di belakang UI)
        SetupBackground();
        
        // Load saved progress
        LoadGameProgress();
        
        // Setup button listeners
        continueButton.onClick.AddListener(OnContinueClicked);
        playButton.onClick.AddListener(OnPlayClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        
        // Update UI berdasarkan save data
        UpdateContinueButton();
        
        // Show main menu, hide others
        mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        
        Debug.Log("🎮 Main Menu Ready - Sprite Background");
    }

    void SetupBackground()
    {
        if (backgroundSprite != null)
        {
            // Pastikan background di belakang UI
            backgroundSprite.sortingLayerName = "Background";
            backgroundSprite.sortingOrder = -1;
            
            // Scale background untuk fill screen (optional)
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Hitung scale agar sprite fill screen
                Sprite sprite = backgroundSprite.sprite;
                if (sprite != null)
                {
                    float cameraHeight = mainCamera.orthographicSize * 2f;
                    float cameraWidth = cameraHeight * mainCamera.aspect;
                    
                    Vector2 spriteSize = sprite.bounds.size;
                    
                    float scaleX = cameraWidth / spriteSize.x;
                    float scaleY = cameraHeight / spriteSize.y;
                    
                    // Gunakan scale yang lebih besar untuk cover seluruh screen
                    float scale = Mathf.Max(scaleX, scaleY);
                    backgroundSprite.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }
        }
    }

    void LoadGameProgress()
    {
        // Load dari PlayerPrefs (sesuai dengan GameManager)
        savedWave = PlayerPrefs.GetInt("CurrentWave", 0);
        savedScore = PlayerPrefs.GetInt("PlayerScore", 0);
        savedLives = PlayerPrefs.GetInt("BaseLives", 3);
        
        // Cek apakah ada progress yang bisa di-continue
        hasSaveData = savedWave > 0 && savedWave < 3; // Bisa continue jika wave 1 atau 2
        
        Debug.Log($"💾 Loaded progress - Wave: {savedWave}, Score: {savedScore}, Lives: {savedLives}");
    }

    void UpdateContinueButton()
    {
        // Set interaktifitas button
        continueButton.interactable = hasSaveData;
        
        // Update text info jika ada
        if (continueInfoText != null)
        {
            if (hasSaveData)
            {
                continueInfoText.text = $"Continue from Wave {savedWave}\nScore: {savedScore} | Lives: {savedLives}";
            }
            else
            {
                continueInfoText.text = "No saved game found";
            }
        }
        
        // Update button text
        Text buttonText = continueButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            if (hasSaveData)
            {
                buttonText.text = $"CONTINUE - WAVE {savedWave}";
            }
            else
            {
                buttonText.text = "CONTINUE";
                buttonText.color = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Grayed out
            }
        }
    }

    public void OnContinueClicked()
    {
        if (!hasSaveData) return;
        
        PlayButtonSound();
        Debug.Log($"🔄 Continuing from Wave {savedWave}");
        
        // Set flag untuk continue di GameManager
        PlayerPrefs.SetInt("ShouldContinue", 1);
        PlayerPrefs.Save();
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }

    public void OnPlayClicked()
    {
        PlayButtonSound();
        Debug.Log("🎮 Starting NEW game...");
        
        // Reset progress untuk new game
        PlayerPrefs.SetInt("ShouldContinue", 0);
        PlayerPrefs.SetInt("CurrentWave", 0);
        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.SetInt("BaseLives", 3);
        PlayerPrefs.Save();
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuitClicked()
    {
        PlayButtonSound();
        Debug.Log("🚪 Quitting game...");
        
        // Add slight delay for sound feedback
        Invoke("QuitGame", 0.3f);
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}