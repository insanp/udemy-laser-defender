using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damage = collision.gameObject.GetComponent<DamageDealer>();
        ProcessHit(damage);
    }

    private void ProcessHit(DamageDealer damage)
    {
        health -= damage.GetDamage();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
