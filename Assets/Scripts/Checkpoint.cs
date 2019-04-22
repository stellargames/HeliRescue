using System;
using Persistence;
using UnityEngine;

[RequireComponent(typeof(GuidComponent))]
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

    public void Load(object obj)
    {
        var data = (CheckpointData) obj;
        _activated = data.activated;
    }

    public object Save()
    {
        return new CheckpointData {activated = _activated};
    }

    public Guid GetGuid()
    {
        return GetComponent<GuidComponent>().GetGuid();
    }

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
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Activate();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
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

    [Serializable]
    private struct CheckpointData
    {
        public bool activated;
    }
}