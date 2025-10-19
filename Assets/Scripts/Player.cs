using System.Collections;
using System.Collections.Generic;
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

    private float waktuCooldown = 0f;
    private Camera mainCamera;

    private bool isMovingUp = false;
    private bool isMovingDown = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleCooldown();
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

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        if (context.started)
            isMovingUp = true;
        else if (context.canceled)
            isMovingUp = false;
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if (context.started)
            isMovingDown = true;
        else if (context.canceled)
            isMovingDown = false;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (waktuCooldown <= 0f)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (prefabPanah == null || titikTembak == null)
            return;

        GameObject panah = Instantiate(prefabPanah, titikTembak.position, Quaternion.identity);
        Rigidbody2D rb = panah.GetComponent<Rigidbody2D>();

        Vector2 arahTembak = Vector2.right;
        rb.velocity = arahTembak * tenagaPanah;
        panah.transform.rotation = Quaternion.identity;

        waktuCooldown = cooldownTembak;
    }
}