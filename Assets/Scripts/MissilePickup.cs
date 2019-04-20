using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MissilePickup : MonoBehaviour
{
    private AudioSource _audioSource;
    private ParticleSystem _particleSystem;
    [SerializeField] private int amount = 3;
    [SerializeField] private Transform visual;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var inventory = other.gameObject.GetComponent<Inventory>();
            var missilesTransferred = inventory.AddMissiles(amount);

            PlayPickupSound(missilesTransferred);
            PlayPickupParticles(missilesTransferred);

            amount -= missilesTransferred;
            if (amount <= 0)
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

    private void PlayPickupSound(int amount)
    {
        if ((_audioSource != null) & (amount > 0))
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
}