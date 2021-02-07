using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configurations params
    [Header("Player")]
    [SerializeField] int health = 300;
    [SerializeField] float maxMoveSpeed = 0.5f;
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

    Vector3 touchStartPosFinger;
    Vector3 touchStartPosPlayer;

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
        if (Input.touchCount == 1)
        {
            // latest touch
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            //Debug.Log("Player : " + transform.position + " Touch : " + touchStartPosPlayer);
    
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Record initial touch position
                    touchStartPosFinger = touchPosition;
                    touchStartPosPlayer = transform.position;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    var newXPos = Mathf.Clamp(touchStartPosPlayer.x + touchPosition.x - touchStartPosFinger.x, xMin, xMax);
                    var newYPos = Mathf.Clamp(touchStartPosPlayer.y + touchPosition.y - touchStartPosFinger.y, yMin, yMax);

                    // if clamped at edge, update player new position so it won't stuck
                    if (newXPos == xMin || newXPos == xMax)
                    {
                        touchStartPosPlayer.x = transform.position.x;
                    }
                    if (newYPos == yMin || newYPos == yMax)
                    {
                        touchStartPosPlayer.y = transform.position.y;
                    }

                    var newPos = new Vector2(newXPos, newYPos);
                    transform.position = Vector2.MoveTowards(transform.position, newPos, maxMoveSpeed);
                    break;

                case TouchPhase.Ended:
                    break;
            }
            
        }
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
