using System;
using System.Collections;
using Persistence;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(GuidComponent))]
public class MissilePickup : MonoBehaviour, IPersist
{
    private AudioSource _audioSource;
    private ParticleSystem _particleSystem;

    [SerializeField] private int amountAvailable = 3;
    [SerializeField] private Transform visual;

    public Guid GetGuid()
    {
        return GetComponent<GuidComponent>().GetGuid();
    }

    public object Save()
    {
        return new MissilePickupData {amountAvailable = amountAvailable};
    }

    public void Load(object obj)
    {
        var data = (MissilePickupData) obj;
        amountAvailable = data.amountAvailable;
        if (amountAvailable <= 0) Destroy(gameObject);
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var inventory = other.gameObject.GetComponentInParent<Inventory>();
            var itemsTransferred = inventory.AddMissiles(amountAvailable);

            PlayMultiplePickupSound(itemsTransferred);
            PlayPickupParticles(itemsTransferred);

            amountAvailable -= itemsTransferred;
            if (amountAvailable <= 0)
            {
                visual.gameObject.SetActive(false);
                Destroy(gameObject, 2f);
            }
        }
    }

    private void PlayPickupParticles(int amount)
    {
        if (_particleSystem != null && amount > 0) _particleSystem.Play();
    }

    private void PlayMultiplePickupSound(int amount)
    {
        if (_audioSource != null && amount > 0)
        {
            _audioSource.loop = true;
            var duration = amount * _audioSource.clip.length * 0.9f;
            StartCoroutine(StopAudioSourceAfterDelay(duration));
            _audioSource.Play();
        }
    }

    private IEnumerator StopAudioSourceAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        _audioSource.loop = false;
    }

    [Serializable]
    private struct MissilePickupData
    {
        public int amountAvailable;
    }
}