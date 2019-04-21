using System;
using Persistence;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Checkpoint : MonoBehaviour
{
    private bool _activated;
    private AudioSource _audioSource;
    private float _blinkTimer;
    private float _busyTimer;
    private ParticleSystem _particle;
    [SerializeField] private float blinkDelay = 1.5f;
    [SerializeField] private GameObject blinkLight;
    [SerializeField] private float minimumTimeBetweenLandings = 5f;

    public static event Action<Checkpoint> Reached = delegate { };

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particle = GetComponent<ParticleSystem>();
        blinkLight.SetActive(false);
    }

    private void Update()
    {
        if (!_activated) return;

        _blinkTimer += Time.deltaTime;
        if (_blinkTimer > blinkDelay)
        {
            blinkLight.SetActive(!blinkLight.activeSelf);
            _blinkTimer = 0;
        }

        if (_busyTimer > 0f) _busyTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_busyTimer > 0f) return;
        if (!other.gameObject.CompareTag("Player")) return;

        Activate();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _busyTimer = minimumTimeBetweenLandings;
    }

    private void Activate()
    {
        if (_activated) return;

        _particle.Play();
        _audioSource.Play();
        _activated = true;
        Reached.Invoke(this);
    }

    public void Load(GameDataReader reader)
    {
        _activated = reader.ReadBool();
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(_activated);
    }
}