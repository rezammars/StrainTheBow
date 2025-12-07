using UnityEngine;
using UnityEngine.UI;

public class WaveDisplay : MonoBehaviour
{
    public GameObject wavePanel;
    public Text waveText;
    public float showTime = 2f;
    
    void Start()
    {
        if (wavePanel != null)
            wavePanel.SetActive(false);
    }
    
    public void ShowWave(int waveNumber)
    {
        if (wavePanel != null)
        {
            UpdateWaveDisplay();
            wavePanel.SetActive(true);
            Invoke("HideWave", showTime);
        }
    }
    
    public void HideWave()
    {
        if (wavePanel != null)
            wavePanel.SetActive(false);
    }
    
    public void UpdateWaveDisplay()
    {
        if (waveText != null && GameManager.instance != null)
        {
            waveText.text = $"WAVE {GameManager.instance.currentWave}/3";
        }
    }
}