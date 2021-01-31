using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] AudioClip soundSFX;
    [SerializeField] [Range(0, 1)] float soundSFXVolume = 0.5f;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(soundSFX, Camera.main.transform.position, soundSFXVolume);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
