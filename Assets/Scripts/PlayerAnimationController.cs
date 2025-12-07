using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Player player;
    
    // Animation parameter names
    private const string IS_MOVING = "IsMoving";
    private const string ATTACK_TRIGGER = "Attack";
    
    // Cache movement state
    private bool wasMoving = false;
    private float attackCooldownTimer = 0f;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        
        if (animator == null)
        {
            Debug.LogError("❌ Animator component not found on Player!");
        }
        
        if (player == null)
        {
            Debug.LogError("❌ Player script not found!");
        }
    }
    
    void Update()
    {
        UpdateMovementAnimation();
        UpdateAttackCooldown();
    }
    
    void UpdateMovementAnimation()
    {
        // Check if player is moving (gunakan input dari Player script)
        bool isMoving = false;
        
        // Cara 1: Check vertical input (jika menggunakan keyboard/input system)
        if (player != null)
        {
            // Anda perlu mengekspos verticalInput dari Player script
            // Atau tambahkan method public di Player: public bool IsMoving()
            isMoving = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        }
        
        // Cara 2: Check velocity (jika menggunakan Rigidbody)
        // Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // isMoving = rb.velocity.magnitude > 0.1f;
        
        // Update animator parameter
        if (animator != null)
        {
            animator.SetBool(IS_MOVING, isMoving);
            
            // Optional: Play footstep sound when starting to move
            if (isMoving && !wasMoving)
            {
                // AudioManager.instance.PlayFootstep();
            }
        }
        
        wasMoving = isMoving;
    }
    
    void UpdateAttackCooldown()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }
    
    // ⭐ CALL THIS METHOD WHEN PLAYER SHOOTS
    public void PlayAttackAnimation()
    {
        if (animator != null && attackCooldownTimer <= 0f)
        {
            animator.SetTrigger(ATTACK_TRIGGER);
            attackCooldownTimer = 0.3f; // Cooldown antara attack animations
            
            // Optional: Play attack sound
            // AudioManager.instance.PlayAttackSound();
        }
    }
    
    // ⭐ METHOD UNTUK DISABLE/ENABLE ANIMATIONS (saat game over)
    public void EnableAnimations()
    {
        if (animator != null)
            animator.enabled = true;
    }
    
    public void DisableAnimations()
    {
        if (animator != null)
            animator.enabled = false;
    }
    
    // ⭐ ANIMATION EVENT CALLBACK (optional)
    public void OnAttackAnimationStart()
    {
        Debug.Log("🎯 Attack animation started");
    }
    
    public void OnAttackAnimationEnd()
    {
        Debug.Log("🎯 Attack animation ended");
    }
}