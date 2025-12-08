using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    public float moveSpeed = 2f;
    public int scoreValue = 10;

    [Header("Movement Settings")]
    public MovementType movementType = MovementType.StraightLeft;
    public enum MovementType { StraightLeft, SineWave, ZigZag }

    [Header("Walk Animation")]
    public Sprite[] walkFrames;        // Sprite frames untuk animasi jalan
    public float walkFrameRate = 0.15f; // Kecepatan ganti frame
    public bool randomStartFrame = true; // Acak frame awal

    [Header("Visual Effects")]
    public Color hitFlashColor = Color.red;
    public float hitFlashDuration = 0.1f;
    public GameObject deathEffect;

    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    
    // Private variables
    private int currentHealth;
    private bool isDead = false;
    private float laneYPosition;
    private bool canMove = true;
    private bool diedByPlayer = false;
    
    // Animation variables
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Color originalColor;
    
    // Movement pattern variables
    private float sineWaveTimer = 0f;
    private float zigzagTimer = 0f;

    void Start()
    {
        InitializeComponents();
        SetupEnemy();
        SetupWalkAnimation();
    }

    void InitializeComponents()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                // Tambahkan SpriteRenderer jika tidak ada
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                Debug.Log("➕ Added SpriteRenderer to Enemy");
            }
        }
        
        enemyCollider = GetComponent<Collider2D>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void SetupEnemy()
    {
        currentHealth = maxHealth;
        laneYPosition = transform.position.y;
        canMove = true;
    }

    void SetupWalkAnimation()
    {
        // Acak frame awal jika diaktifkan
        if (randomStartFrame && walkFrames.Length > 0)
        {
            currentFrame = Random.Range(0, walkFrames.Length);
            UpdateSpriteFrame();
        }
        
        // Jika tidak ada walk frames, gunakan sprite saat ini
        if (walkFrames.Length == 0 && spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Buat array dengan sprite saat ini
            walkFrames = new Sprite[] { spriteRenderer.sprite };
            Debug.Log("⚠️ No walk frames set, using current sprite");
        }
    }

    void Update()
    {
        if (isDead || !canMove) return;
        
        HandleMovement();
        UpdateWalkAnimation();
    }

    void HandleMovement()
    {
        if (!canMove) return;

        Vector3 movement = Vector3.zero;
        
        switch (movementType)
        {
            case MovementType.StraightLeft:
                movement = Vector3.left * moveSpeed * Time.deltaTime;
                LockToLane();
                break;
                
            case MovementType.SineWave:
                sineWaveTimer += Time.deltaTime * 2f;
                float wave = Mathf.Sin(sineWaveTimer) * 2f;
                movement = new Vector3(-moveSpeed, wave * 0.5f, 0) * Time.deltaTime;
                break;
                
            case MovementType.ZigZag:
                zigzagTimer += Time.deltaTime * 3f;
                float zigzag = Mathf.PingPong(zigzagTimer, 2f) - 1f;
                movement = new Vector3(-moveSpeed, zigzag * 1f, 0) * Time.deltaTime;
                break;
        }
        
        transform.Translate(movement);
    }

    void LockToLane()
    {
        Vector3 currentPos = transform.position;
        if (Mathf.Abs(currentPos.y - laneYPosition) > 0.01f)
        {
            transform.position = new Vector3(currentPos.x, laneYPosition, currentPos.z);
        }
    }

    void UpdateWalkAnimation()
    {
        // Update walk animation jika ada walk frames
        if (walkFrames.Length == 0 || !canMove) return;
        
        frameTimer += Time.deltaTime;
        
        if (frameTimer >= walkFrameRate)
        {
            frameTimer = 0f;
            NextWalkFrame();
        }
    }

    void NextWalkFrame()
    {
        if (walkFrames.Length == 0) return;
        
        currentFrame = (currentFrame + 1) % walkFrames.Length;
        UpdateSpriteFrame();
    }

    void UpdateSpriteFrame()
    {
        if (spriteRenderer != null && walkFrames.Length > 0 && currentFrame < walkFrames.Length)
        {
            spriteRenderer.sprite = walkFrames[currentFrame];
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || !canMove) return;

        currentHealth -= damageAmount;
        
        // Flash effect
        StartCoroutine(FlashHitColor());

        if (currentHealth <= 0)
        {
            diedByPlayer = true;
            Die();
        }
    }

    IEnumerator FlashHitColor()
    {
        if (spriteRenderer == null) yield break;
        
        // Simpan sprite saat ini
        Sprite currentSprite = spriteRenderer.sprite;
        
        // Flash merah
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        
        // Kembali ke warna normal
        spriteRenderer.color = originalColor;
        
        // Kembalikan sprite (jika berubah)
        spriteRenderer.sprite = currentSprite;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Stop movement
        canMove = false;
        
        // Notify game systems
        NotifyGameSystems();
        
        // Visual effects
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Fade out and destroy
        StartCoroutine(FadeOutAndDestroy());
    }

    void NotifyGameSystems()
    {
        if (diedByPlayer)
        {
            // Mati oleh player - dapat score
            if (GameManager.instance != null)
                GameManager.instance.EnemyKilledByPlayer();
        }
        else
        {
            // Mati karena sampai base - tidak dapat score
            if (GameManager.instance != null)
                GameManager.instance.EnemyReachedBase();
        }

        if (WaveManager.instance != null)
            WaveManager.instance.OnEnemyKilled();
    }

    IEnumerator FadeOutAndDestroy()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }
        
        // Fade out
        float fadeTime = 0.3f;
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            // Scale down sedikit
            float scale = Mathf.Lerp(1f, 0.5f, elapsedTime / fadeTime);
            transform.localScale = new Vector3(scale, scale, 1f);
            
            yield return null;
        }
        
        // Destroy
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !canMove) return;

        if (other.CompareTag("Base"))
        {
            diedByPlayer = false;
            Die();
        }
    }

    // Public methods untuk WaveManager
    public void SetMovementType(MovementType newType) 
    { 
        movementType = newType; 
    }
    
    public void SetLanePosition(float yPosition) 
    { 
        laneYPosition = yPosition; 
    }
    
    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
    }
    
    public void EnableMovement() 
    { 
        canMove = true; 
    }
    
    public void DisableMovement() 
    { 
        canMove = false; 
    }
    
    // Method untuk setup walk animation frames
    public void SetupWalkFrames(Sprite[] frames, float frameRate = 0.15f)
    {
        walkFrames = frames;
        walkFrameRate = frameRate;
        
        if (walkFrames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = walkFrames[0];
        }
    }
    
    // Method untuk set single sprite (jika tidak mau animasi)
    public void SetStaticSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            walkFrames = new Sprite[0]; // Kosongkan walk frames
        }
    }
}