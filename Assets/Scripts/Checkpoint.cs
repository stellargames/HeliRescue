using System;
using Persistence;
using Skytanet.SimpleDatabase;
using UnityEngine;

[RequireComponent(typeof(PersistenceComponent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Checkpoint : MonoBehaviour, IPersist
{
    private bool _activated;
    private AudioSource _audioSource;
    private float _blinkTimer;
    private Guid _guid;
    private float _lastActivated;
    private ParticleSystem _particle;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private float blinkDelay = 1.5f;
    [SerializeField] private GameObject blinkLight;
    [SerializeField] private float minimumTimeBetweenLandings = 5f;
#pragma warning restore 0649

    public void Load(SaveFile file)
    {
        _activated = file.Get(_guid.ToString(), false);
        if (_activated) _lastActivated = Time.time;
    }

    public void Save(SaveFile file)
    {
        file.Set(_guid.ToString(), _activated);
    }

    public static event Action<Checkpoint> Reached = delegate { };

    private void Awake()
    {
        _guid = GetComponent<GuidComponent>().GetGuid();
        _audioSource = GetComponent<AudioSource>();
        _particle = GetComponent<ParticleSystem>();
        blinkLight.SetActive(false);
        _lastActivated = Time.time - minimumTimeBetweenLandings;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Activate();
        _lastActivated = Time.time;
    }

    private void Activate()
    {
        if (Time.time - _lastActivated < minimumTimeBetweenLandings) return;

        _particle.Play();
        _audioSource.Play();
        _activated = true;
        Reached.Invoke(this);
    }
}