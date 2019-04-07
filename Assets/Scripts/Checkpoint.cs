using System;
using Persistence;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float blinkDelay = 1.5f;
    [SerializeField] private GameObject blinkLight = null;

    private bool _activated;
    private ParticleSystem _particle;
    private AudioSource _audioSource;
    private float _blinkTimer;

    public static event Action<Checkpoint> Reached = delegate(Checkpoint point) { };

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
    }

    private void OnCollisionEnter2D(Collision2D other1)
    {
        if (!_activated)
        {
            Activate();
        }
    }

    private void Activate()
    {
        _particle.Play();
        _audioSource.Play();
        _activated = true;
        Reached(this);
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
