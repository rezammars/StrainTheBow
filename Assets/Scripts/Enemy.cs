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

    private int currentHealth;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    private float laneYPosition;
    private bool canMove = true;
    private bool diedByPlayer = false;

    public enum MovementType
    {
        StraightLeft,
        SineWave,
        ZigZag
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        laneYPosition = transform.position.y;
        canMove = true;
    }

    void Update()
    {
        if (isDead || !canMove) return;
        MoveAccordingToType();
    }

    public void EnableMovement() => canMove = true;
    public void DisableMovement() => canMove = false;

    void MoveAccordingToType()
    {
        if (!canMove) return;

        switch (movementType)
        {
            case MovementType.StraightLeft:
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
                LockToLane();
                break;
            case MovementType.SineWave:
                float wave = Mathf.Sin(Time.time * 2f) * 2f;
                Vector3 waveMovement = new Vector3(-moveSpeed, wave * 0.5f, 0) * Time.deltaTime;
                transform.Translate(waveMovement);
                break;
            case MovementType.ZigZag:
                float zigzag = Mathf.PingPong(Time.time * 3f, 2f) - 1f;
                Vector3 zigzagMovement = new Vector3(-moveSpeed, zigzag * 1f, 0) * Time.deltaTime;
                transform.Translate(zigzagMovement);
                break;
        }
    }

    void LockToLane()
    {
        Vector3 currentPos = transform.position;
        if (Mathf.Abs(currentPos.y - laneYPosition) > 0.01f)
        {
            transform.position = new Vector3(currentPos.x, laneYPosition, currentPos.z);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || !canMove) return;

        currentHealth -= damageAmount;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            diedByPlayer = true;
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

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

        DestroyEnemy();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !canMove) return;

        if (other.CompareTag("Base"))
        {
            diedByPlayer = false; // Tidak dapat score
            Die();
        }
    }

    void DestroyEnemy()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Destroy(gameObject);
    }

    public void SetMovementType(MovementType newType) => movementType = newType;
    public void SetLanePosition(float yPosition) => laneYPosition = yPosition;
    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
    }
}