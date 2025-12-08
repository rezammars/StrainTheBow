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
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        
        if (animator == null)
        {
            Debug.LogError("❌ Animator component not found on Player!");
        }
        else
        {
            Debug.Log("✅ Animator found: " + animator.name);
        }
        
        if (player == null)
        {
            Debug.LogError("❌ Player script not found!");
        }
        else
        {
            Debug.Log("✅ Player script found");
        }
    }
    
    void Update()
    {
        UpdateMovementAnimation();
    }
    
    void UpdateMovementAnimation()
    {
        if (player != null && animator != null)
        {
            bool isMoving = player.IsMoving();
            animator.SetBool(IS_MOVING, isMoving);
            
            // Optional: Log for debugging
            if (isMoving != wasMoving)
            {
                Debug.Log($"🎬 Movement animation: {isMoving}");
                wasMoving = isMoving;
            }
        }
    }
    
    public void PlayAttackAnim()
    {
        if (animator != null)
        {
            Debug.Log("🎯 Setting Attack trigger on Animator");
            animator.SetTrigger(ATTACK_TRIGGER);
            
            // Force animator update to ensure trigger is processed
            animator.Update(0f);
        }
        else
        {
            Debug.LogError("❌ Animator is null in PlayAttackAnim!");
        }
    }
    
    public void EnableAnimations()
    {
        if (animator != null)
        {
            animator.enabled = true;
            Debug.Log("✅ Animations enabled");
        }
    }
    
    public void DisableAnimations()
    {
        if (animator != null)
        {
            animator.enabled = false;
            Debug.Log("⏸️ Animations disabled");
        }
    }
    
    // ⭐ Animation Event Methods (optional - bisa dihubungkan di Animation Clip)
    public void OnAttackStart()
    {
        Debug.Log("🎬 Attack animation STARTED");
    }
    
    public void OnAttackEnd()
    {
        Debug.Log("🎬 Attack animation ENDED");
    }
    
    public void OnFootstep()
    {
        // Bisa dipanggil dari Animation Event
        Debug.Log("👣 Footstep sound should play");
        // AudioManager.instance.PlayFootstep();
    }
}