using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image gameOverImage;
    public Text scoreText;
    public Button restartBtn;
    public Button mainMenuBtn;
    
    void Start()
    {
        // ⭐ JIKA BUTTON EVENTS TIDAK DI-SET DI INSPECTOR,
        // TAMBAH LISTENER DI CODE
        SetupButtonListeners();
        
        // Hide panel di awal
        gameObject.SetActive(false);
        
        // Set sprite
        if (gameOverImage != null)
        {
            gameOverImage.SetNativeSize();
        }
    }
    
    // ⭐ SETUP BUTTON LISTENERS (SAFETY NET)
    void SetupButtonListeners()
    {
        // Restart Button
        if (restartBtn != null)
        {
            restartBtn.onClick.RemoveAllListeners(); // Clear existing
            restartBtn.onClick.AddListener(RestartGame);
        }
        
        // Main Menu Button  
        if (mainMenuBtn != null)
        {
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(GoToMainMenu);
        }
    }
    
    // ⭐ METHOD HARUS PUBLIC UNTUK BISA DIPANGGIL DARI INSPECTOR
    public void ShowGameOver(bool isWin = false)
    {
        // Update score
        if (scoreText != null && GameManager.instance != null)
            scoreText.text = $"Score: {GameManager.instance.currentScore}";
        
        // Show panel
        gameObject.SetActive(true);
    }
    
    // ⭐ PUBLIC METHOD UNTUK BUTTON
    public void RestartGame()
    {
        Debug.Log("🔄 Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // ⭐ PUBLIC METHOD UNTUK BUTTON  
    public void GoToMainMenu()
    {
        Debug.Log("🏠 Going to main menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Ganti dengan nama scene main menu
    }
}