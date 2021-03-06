﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterCollision : MonoBehaviour
{
#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Explosion[] explosionPrefabs;
    [SerializeField] private Collider2D landingGear;
#pragma warning restore 0649

    public static event Action Exploded = delegate { };

    private bool _colliding;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_colliding) return;
        if (other.gameObject.CompareTag("Pickup")) return;
        if (other.otherCollider == landingGear &&
            !other.gameObject.CompareTag("Enemy")) return;

        _colliding = true;

        var explosion = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
        explosion.Get<Explosion>(transform.position, Quaternion.identity);
        PlayExplosionAudio();

        GetComponent<HelicopterController>().enabled = false;

        DisableChildren();
        Destroy(gameObject, 2f);

        Exploded.Invoke();
    }

    private void PlayExplosionAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource || audioClips.Length == 0) return;
        audioSource.Stop();

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }

    private void DisableChildren()
    {
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}