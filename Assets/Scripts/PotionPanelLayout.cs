using UnityEngine;
using UnityEngine.UI;

public class PotionPanelLayout : MonoBehaviour
{
    [Header("Potion References")]
    public PotionImageUI redPotion;
    public PotionImageUI bluePotion;
    public PotionImageUI greenPotion;

    [Header("Layout Settings")]
    public PanelPosition panelPosition = PanelPosition.Left;
    public enum PanelPosition { Left, Center, Right }
    
    public float horizontalMargin = 20f;
    public float verticalMargin = 50f;
    public float spacing = 80f;
    public Vector2 iconSize = new Vector2(70f, 70f);

    [Header("Background")]
    public Image backgroundPanel;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.2f);

    void Start()
    {
        SetupPanel();
    }

    void SetupPanel()
    {
        // Setup panel position berdasarkan setting
        PositionPanel();
        
        // Setup background
        if (backgroundPanel != null)
        {
            backgroundPanel.color = backgroundColor;
            
            // Adjust background size based on content
            float panelWidth = iconSize.x + 20f; // Lebar cukup untuk 1 icon + padding
            float panelHeight = (iconSize.y * 3) + (spacing * 2) + 20f; // Tinggi untuk 3 icon + spacing
            backgroundPanel.rectTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
        }

        // Arrange potions secara vertikal
        ArrangePotionsVertically();
    }

    void PositionPanel()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) return;

        // Set anchor berdasarkan posisi yang dipilih
        switch (panelPosition)
        {
            case PanelPosition.Left:
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0f, 0f);
                rect.anchoredPosition = new Vector2(horizontalMargin, verticalMargin);
                break;
                
            case PanelPosition.Center:
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, verticalMargin);
                break;
                
            case PanelPosition.Right:
                rect.anchorMin = new Vector2(1f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(1f, 0f);
                rect.anchoredPosition = new Vector2(-horizontalMargin, verticalMargin);
                break;
        }
    }

    void ArrangePotionsVertically()
    {
        // Hitung posisi Y untuk setiap potion (dari atas ke bawah)
        float totalHeight = (iconSize.y * 3) + (spacing * 2);
        float startY = totalHeight / 2 - iconSize.y / 2; // Mulai dari atas
        
        if (redPotion != null)
        {
            PositionPotion(redPotion.gameObject, 0f, startY);
            redPotion.potionType = PotionImageUI.PotionType.Red;
        }
        
        if (bluePotion != null)
        {
            PositionPotion(bluePotion.gameObject, 0f, 0f);
            bluePotion.potionType = PotionImageUI.PotionType.Blue;
        }
        
        if (greenPotion != null)
        {
            PositionPotion(greenPotion.gameObject, 0f, -startY);
            greenPotion.potionType = PotionImageUI.PotionType.Green;
        }
    }

    void PositionPotion(GameObject potion, float xPosition, float yPosition)
    {
        RectTransform rect = potion.GetComponent<RectTransform>();
        if (rect == null) return;

        // Set anchor ke center left
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);

        // Set position
        rect.anchoredPosition = new Vector2(xPosition, yPosition);

        // Set size
        rect.sizeDelta = iconSize;
    }

    // Public methods
    public void FlashPotion(string potionType)
    {
        Debug.Log($"⚡ Flashing {potionType} potion");
        
        switch (potionType.ToLower())
        {
            case "red":
                if (redPotion != null) redPotion.FlashPotion();
                break;
            case "blue":
                if (bluePotion != null) bluePotion.FlashPotion();
                break;
            case "green":
                if (greenPotion != null) greenPotion.FlashPotion();
                break;
        }
    }

    public void SetPanelPosition(PanelPosition newPosition)
    {
        panelPosition = newPosition;
        PositionPanel();
    }

    public void SetMargins(float horizontal, float vertical)
    {
        horizontalMargin = horizontal;
        verticalMargin = vertical;
        PositionPanel();
    }

    public void SetSpacing(float newSpacing)
    {
        spacing = newSpacing;
        ArrangePotionsVertically();
        
        // Update background size
        if (backgroundPanel != null)
        {
            float panelHeight = (iconSize.y * 3) + (spacing * 2) + 20f;
            backgroundPanel.rectTransform.sizeDelta = new Vector2(iconSize.x + 20f, panelHeight);
        }
    }
}