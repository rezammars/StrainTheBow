using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PotionImageUI : MonoBehaviour
{
    [Header("Potion Settings")]
    public PotionType potionType;
    public enum PotionType { Red, Blue, Green }

    [Header("UI References")]
    public Image potionImage;
    public Image cooldownOverlay;

    [Header("Colors")]
    public Color readyColor = Color.white;
    public Color activeColor = Color.yellow;
    public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private Player player;
    private Vector3 originalScale;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // Cari player
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError($"PotionImageUI: Player tidak ditemukan! ({potionType})");
            return;
        }

        // Get potion image jika belum diassign
        if (potionImage == null)
        {
            potionImage = GetComponent<Image>();
        }

        // Simpan scale asli
        originalScale = transform.localScale;

        // Setup cooldown overlay
        if (cooldownOverlay != null)
        {
            cooldownOverlay.type = Image.Type.Filled;
            cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
            cooldownOverlay.fillClockwise = false;
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || player == null) return;

        UpdatePotionStatus();
    }

    void UpdatePotionStatus()
    {
        bool isActive = false;
        float currentTimer = 0f;

        // Get status dari Player
        switch (potionType)
        {
            case PotionType.Red:
                isActive = player.IsRedPotionActive();
                currentTimer = player.GetRedPotionCooldown();
                break;
            case PotionType.Blue:
                isActive = player.IsBluePotionActive();
                currentTimer = player.GetBluePotionCooldown();
                break;
            case PotionType.Green:
                isActive = player.IsGreenPotionActive();
                currentTimer = player.GetGreenPotionCooldown();
                break;
        }

        // Update visual berdasarkan status
        if (isActive)
        {
            SetActiveState(currentTimer);
        }
        else if (currentTimer > 0)
        {
            SetCooldownState(currentTimer);
        }
        else
        {
            SetReadyState();
        }
    }

    void SetReadyState()
    {
        if (potionImage != null)
        {
            potionImage.color = readyColor;
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
        
        transform.localScale = originalScale;
    }

    void SetActiveState(float remainingTime)
    {
        if (potionImage != null)
        {
            potionImage.color = activeColor;
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            float activeDuration = GetActiveDuration();
            float fillAmount = Mathf.Clamp01(remainingTime / activeDuration);
            cooldownOverlay.fillAmount = fillAmount;
            cooldownOverlay.color = new Color(1f, 1f, 0f, 0.6f);
        }
        
        transform.localScale = originalScale;
    }

    void SetCooldownState(float remainingTime)
    {
        if (potionImage != null)
        {
            potionImage.color = cooldownColor;
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            float maxCooldown = GetMaxCooldown();
            float fillAmount = Mathf.Clamp01(remainingTime / maxCooldown);
            cooldownOverlay.fillAmount = fillAmount;
            cooldownOverlay.color = new Color(1f, 0f, 0f, 0.6f);
        }
        
        transform.localScale = originalScale * 0.9f;
    }

    float GetActiveDuration()
    {
        switch (potionType)
        {
            case PotionType.Red: return 8f;
            case PotionType.Blue: return 5f;
            case PotionType.Green: return 5f;
            default: return 5f;
        }
    }

    float GetMaxCooldown()
    {
        if (player == null) return 10f;

        switch (potionType)
        {
            case PotionType.Red: return player.redPotionCooldown;
            case PotionType.Blue: return player.bluePotionCooldown;
            case PotionType.Green: return player.greenPotionCooldown;
            default: return 10f;
        }
    }

    // Public methods
    public void FlashPotion()
    {
        StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        if (potionImage == null) yield break;

        Vector3 originalScale = transform.localScale;
        Color originalColor = potionImage.color;

        // Scale up + putih
        transform.localScale = originalScale * 1.3f;
        potionImage.color = Color.white;
        
        yield return new WaitForSeconds(0.1f);

        // Kembali ke normal
        transform.localScale = originalScale;
        potionImage.color = originalColor;
    }
}