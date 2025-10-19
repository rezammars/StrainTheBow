using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panah : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 2;
    public float lifetime = 8f;
    public string targetTag = "Enemy";

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
