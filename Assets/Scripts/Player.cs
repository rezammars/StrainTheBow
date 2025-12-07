using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float verticalInput;
    public float batasAtas = 30f;
    public float batasBawah = -30f;

    [Header("Shooting")]
    public GameObject prefabPanah;
    public Transform titikTembak;
    public float tenagaPanah = 80f;
    public float cooldownTembak = 0.5f;

    [Header("Input Control")]
    private bool isInputEnabled = true; // ⭐ FLAG UNTUK ENABLE/DISABLE INPUT
    private PlayerInput playerInput; // ⭐ REFERENCE KE PLAYERINPUT COMPONENT

    [Header("Booster System")]
    public float redPotionCooldown = 10f;
    public float bluePotionCooldown = 12f;
    public float greenPotionCooldown = 10f;

    // BOOSTER
    private bool isRedPotionActive = false;
    private bool isBluePotionActive = false;
    private bool isGreenPotionActive = false;
    
    private float redPotionTimer = 0f;
    private float bluePotionTimer = 0f;
    private float greenPotionTimer = 0f;
    
    private float redPotionCooldownTimer = 0f;
    private float bluePotionCooldownTimer = 0f;
    private float greenPotionCooldownTimer = 0f;

    // DEFAULT STATS UNTUK RESET
    private float defaultCooldownTembak = 0.5f;
    private int defaultDamage = 2;

    private float waktuCooldown = 0f;
    private Camera mainCamera;

    private bool isMovingUp = false;
    private bool isMovingDown = false;
    private PlayerAnimationController animController;

    void Start()
    {
        mainCamera = Camera.main;
        defaultCooldownTembak = cooldownTembak;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning("⚠️ PlayerInput component not found!");
        }
        
        EnableInput(); // Pastikan input enabled di awal
    }

    void Update()
    {
        if (!isInputEnabled) return;
        HandleMovement();
        HandleCooldown();
        UpdateBoosterTimers();
    }

    void HandleMovement()
    {
        verticalInput = 0f;

        if (isMovingUp) verticalInput = 1f;
        if (isMovingDown) verticalInput = -1f;

        Vector3 movement = new Vector3(0, verticalInput * moveSpeed * Time.deltaTime, 0);
        transform.Translate(movement);

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, batasBawah, batasAtas);
        transform.position = pos;
    }

    void HandleCooldown()
    {
        if (waktuCooldown > 0f)
        {
            waktuCooldown -= Time.deltaTime;
        }
    }

    // ⭐ METHOD: UPDATE BOOSTER TIMERS
    void UpdateBoosterTimers()
    {
        // Update active booster timers
        if (isRedPotionActive)
        {
            redPotionTimer -= Time.deltaTime;
            if (redPotionTimer <= 0f)
            {
                DeactivateRedPotion();
            }
        }
        
        if (isBluePotionActive)
        {
            bluePotionTimer -= Time.deltaTime;
            if (bluePotionTimer <= 0f)
            {
                DeactivateBluePotion();
            }
        }
        
        if (isGreenPotionActive)
        {
            greenPotionTimer -= Time.deltaTime;
            if (greenPotionTimer <= 0f)
            {
                DeactivateGreenPotion();
            }
        }
        
        // Update cooldown timers
        if (redPotionCooldownTimer > 0f) redPotionCooldownTimer -= Time.deltaTime;
        if (bluePotionCooldownTimer > 0f) bluePotionCooldownTimer -= Time.deltaTime;
        if (greenPotionCooldownTimer > 0f) greenPotionCooldownTimer -= Time.deltaTime;
    }

    public void EnableInput()
    {
        isInputEnabled = true;
        
        if (playerInput != null)
            playerInput.enabled = true;
            
        Debug.Log("🎮 Player input ENABLED");
    }

    public void DisableInput()
    {
        isInputEnabled = false;
        
        // Reset movement flags
        isMovingUp = false;
        isMovingDown = false;
        verticalInput = 0f;
        
        if (playerInput != null)
            playerInput.enabled = false;
            
        Debug.Log("🎮 Player input DISABLED");
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (context.started)
            isMovingUp = true;
        else if (context.canceled)
            isMovingUp = false;
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (context.started)
            isMovingDown = true;
        else if (context.canceled)
            isMovingDown = false;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (waktuCooldown <= 0f)
        {
            if (isBluePotionActive)
            {
                ShootTriple();
            }
            else
            {
                Shoot();
            }
        }
    }

    // ⭐ METHOD BARU: BOOSTER INPUT
    public void OnRedPotion(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (context.started)
        {
            if (redPotionCooldownTimer <= 0f && !isRedPotionActive)
            {
                ActivateRedPotion();
            }
            else
            {
                Debug.Log($"⏳ Red Potion cooldown: {redPotionCooldownTimer:F1}s");
            }
        }
    }

    public void OnBluePotion(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (context.started)
        {
            if (bluePotionCooldownTimer <= 0f && !isBluePotionActive)
            {
                ActivateBluePotion();
            }
            else
            {
                Debug.Log($"⏳ Blue Potion cooldown: {bluePotionCooldownTimer:F1}s");
            }
        }
    }

    public void OnGreenPotion(InputAction.CallbackContext context)
    {
        if (!isInputEnabled) return;
        if (context.started)
        {
            if (greenPotionCooldownTimer <= 0f && !isGreenPotionActive)
            {
                ActivateGreenPotion();
            }
            else
            {
                Debug.Log($"⏳ Green Potion cooldown: {greenPotionCooldownTimer:F1}s");
            }
        }
    }

    void Shoot()
    {
        if (prefabPanah == null || titikTembak == null)
            return;
        
        if (animController != null)
        {
            animController.PlayAttackAnimation();
        }

        GameObject panah = Instantiate(prefabPanah, titikTembak.position, Quaternion.identity);
        Rigidbody2D rb = panah.GetComponent<Rigidbody2D>();

        // ⭐ APPLY GREEN POTION DAMAGE BOOST
        Panah panahController = panah.GetComponent<Panah>();
        if (panahController != null && isGreenPotionActive)
        {
            panahController.damage = defaultDamage * 2; // ⭐ DOUBLE DAMAGE
        }

        Vector2 arahTembak = Vector2.right;
        rb.velocity = arahTembak * tenagaPanah;
        panah.transform.rotation = Quaternion.identity;

        waktuCooldown = cooldownTembak;
        
        Debug.Log($"🔫 Normal shot - Cooldown: {cooldownTembak}");
    }

    // ⭐ METHOD BARU: TRIPLE SHOT
    void ShootTriple()
    {
        if (prefabPanah == null || titikTembak == null)
            return;
        
        if (animController != null)
        {
            animController.PlayAttackAnimation();
        }

        // Tembak 3 panah dengan sudut berbeda
        float[] angles = { 15f, 0f, -15f }; // Atas, tengah, bawah

        foreach (float angle in angles)
        {
            GameObject panah = Instantiate(prefabPanah, titikTembak.position, Quaternion.identity);
            Rigidbody2D rb = panah.GetComponent<Rigidbody2D>();

            // ⭐ APPLY GREEN POTION DAMAGE BOOST
            Panah panahController = panah.GetComponent<Panah>();
            if (panahController != null && isGreenPotionActive)
            {
                panahController.damage = defaultDamage * 2; // ⭐ DOUBLE DAMAGE
            }

            // Hitung arah berdasarkan angle
            Vector2 arahTembak = Quaternion.Euler(0, 0, angle) * Vector2.right;
            rb.velocity = arahTembak * tenagaPanah;
            panah.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        waktuCooldown = cooldownTembak;
        
        Debug.Log($"🔫🔫🔫 Triple shot - Cooldown: {cooldownTembak}");
    }

    // ⭐ RED POTION - FIRING RATE BOOST
    void ActivateRedPotion()
    {
        isRedPotionActive = true;
        redPotionTimer = 8f; // ⭐ DURASI 8 DETIK
        redPotionCooldownTimer = redPotionCooldown;
        
        // ⭐ EFFECT: Kurangi cooldown tembak
        cooldownTembak = defaultCooldownTembak * 0.4f; // 60% lebih cepat
        
        Debug.Log("🔴 RED POTION ACTIVATED! Firing rate increased for 8 seconds!");
    }

    void DeactivateRedPotion()
    {
        isRedPotionActive = false;
        // ⭐ RESET ke default
        cooldownTembak = defaultCooldownTembak;
        
        Debug.Log("🔴 Red Potion expired. Firing rate back to normal.");
    }

    // ⭐ BLUE POTION - TRIPLE SHOT
    void ActivateBluePotion()
    {
        isBluePotionActive = true;
        bluePotionTimer = 5f; // ⭐ DURASI 5 DETIK
        bluePotionCooldownTimer = bluePotionCooldown;
        
        Debug.Log("🔵 BLUE POTION ACTIVATED! Triple shot for 5 seconds!");
    }

    void DeactivateBluePotion()
    {
        isBluePotionActive = false;
        Debug.Log("🔵 Blue Potion expired. Back to single shot.");
    }

    // ⭐ GREEN POTION - PIERCING DAMAGE
    void ActivateGreenPotion()
    {
        isGreenPotionActive = true;
        greenPotionTimer = 5f; // ⭐ DURASI 5 DETIK
        greenPotionCooldownTimer = greenPotionCooldown;
        
        Debug.Log("🟢 GREEN POTION ACTIVATED! Double damage for 5 seconds!");
    }

    void DeactivateGreenPotion()
    {
        isGreenPotionActive = false;
        Debug.Log("🟢 Green Potion expired. Damage back to normal.");
    }

    public bool IsMoving()
    {
        return isMovingUp || isMovingDown;  
    }
    
    public float GetVerticalInput()
    {
        return verticalInput;
    }

    // ⭐ METHOD UNTUK UI (Optional)
    public float GetRedPotionCooldown() { return Mathf.Max(0f, redPotionCooldownTimer); }
    public float GetBluePotionCooldown() { return Mathf.Max(0f, bluePotionCooldownTimer); }
    public float GetGreenPotionCooldown() { return Mathf.Max(0f, greenPotionCooldownTimer); }
    
    public bool IsRedPotionActive() { return isRedPotionActive; }
    public bool IsBluePotionActive() { return isBluePotionActive; }
    public bool IsGreenPotionActive() { return isGreenPotionActive; }
}