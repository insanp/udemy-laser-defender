using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configurations params
    [Header("Player")]
    [SerializeField] int health = 300;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float padding = 0.2f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;


    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 25f;

    [SerializeField] float laserFiringPeriod = 0.1f;
    [SerializeField] Coroutine firingCo;

    [Header("Sounds")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume = 0.5f;

    Vector2 touchStartPos;
    Vector2 touchDeltaPos;

    float xMin, xMax, yMin, yMax;

    internal object GetHealth()
    {
        return health;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCo = StartCoroutine(FireContinuously());
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCo);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            yield return new WaitForSeconds(laserFiringPeriod); 
        }
    }

    private void Move()
    {
        if (Input.touchCount > 0)
        {
            // first touch
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Record initial touch position
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    touchDeltaPos = touch.position - touchStartPos;
                    touchStartPos = Vector2.MoveTowards(touchStartPos, touch.position, 1.5f);
                    break;

                case TouchPhase.Stationary:
                    touchStartPos = touch.position;
                    touchDeltaPos = new Vector2(0, 0);
                    break;

                case TouchPhase.Ended:
                    touchStartPos = touch.position;
                    touchDeltaPos = new Vector2(0, 0);
                    break;
            }

            
        }
        var deltaX = Mathf.Clamp(touchDeltaPos.x * Time.deltaTime * moveSpeed, -0.1f, 0.1f);
        var deltaY = Mathf.Clamp(touchDeltaPos.y * Time.deltaTime * moveSpeed, -0.1f, 0.1f);

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) return;
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        Destroy(gameObject);
        FindObjectOfType<Level>().LoadGameOver();
    }
}
