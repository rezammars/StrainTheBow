using UnityEngine;

public class HeartsDisplay : MonoBehaviour
{
    public GameObject[] heartObjects;
    public int maxHearts = 3;
    
    void Start()
    {
        InitializeHearts();
        UpdateHeartsDisplay();
    }
    
    void InitializeHearts()
    {
        if (heartObjects == null || heartObjects.Length == 0)
        {
            heartObjects = new GameObject[maxHearts];
            for (int i = 0; i < maxHearts; i++)
            {
                string heartName = "Heart" + (i + 1);
                Transform heart = transform.Find(heartName);
                if (heart != null)
                    heartObjects[i] = heart.gameObject;
            }
        }
    }
    
    public void UpdateHeartsDisplay()
    {
        if (GameManager.instance != null && heartObjects != null)
        {
            int currentLives = GameManager.instance.currentBaseLives;
            for (int i = 0; i < heartObjects.Length; i++)
            {
                if (heartObjects[i] != null)
                    heartObjects[i].SetActive(i < currentLives);
            }
        }
    }
}