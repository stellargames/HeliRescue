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
    private float _lastActivated;
    private ParticleSystem _particle;
    [SerializeField] private float blinkDelay = 1.5f;
    [SerializeField] private GameObject blinkLight;
    [SerializeField] private float minimumTimeBetweenLandings = 5f;

    public Guid Guid { get; private set; }

    public void Load(SaveFile file)
    {
        _activated = file.Get(Guid.ToString(), false);
        if (_activated) _lastActivated = Time.time;
    }

    public void Save(SaveFile file)
    {
        file.Set(Guid.ToString(), _activated);
    }

    public static event Action<Checkpoint> Reached = delegate { };

    private void Awake()
    {
        Guid = GetComponent<GuidComponent>().GetGuid();
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Activate();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
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