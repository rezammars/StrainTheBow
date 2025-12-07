using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;
    
    void Start()
    {
        if (scoreText == null)
            scoreText = GetComponentInChildren<Text>();
        
        UpdateScoreDisplay();
    }
    
    void Update()
    {
        UpdateScoreDisplay();
    }
    
    public void UpdateScoreDisplay()
    {
        if (scoreText != null && GameManager.instance != null)
        {
            scoreText.text = GameManager.instance.currentScore.ToString();
        }
    }
}