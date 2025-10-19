using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    public float moveSpeed = 2f;
    public int scoreValue = 10;

    [Header("References")]
    public Transform baseTarget;

    private int currentHealth;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    private Vector3 originalScale;
    private float laneYPosition;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;

        if (baseTarget == null)
        {
            GameObject baseObj = GameObject.FindGameObjectWithTag("Base");
            if (baseObj != null) baseTarget = baseObj.transform;
        }

        transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        
        laneYPosition = transform.position.y;
    }

    void Update()
    {
        if (isDead) return;

        MoveHorizontalInLane();
    }

    void MoveHorizontalInLane()
    {
        if (isDead) return;

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        LockToLane();
    }

    void LockToLane()
    {
        Vector3 currentPos = transform.position;

        if (Mathf.Abs(currentPos.y - laneYPosition) > 0.1f)
        {
            transform.position = new Vector3(currentPos.x, laneYPosition, currentPos.z);
        }
    }

    public void SetLanePosition(float yPosition)
    {
        laneYPosition = yPosition;

        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, laneYPosition, currentPos.z);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
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

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
            GameManager.instance.EnemyDefeated();
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Base"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.EnemyReachedBase();
            }
            Die();
        }
    }

    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
    }
}