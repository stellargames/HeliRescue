using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class CheckPoint : MonoBehaviour
{
    private ParticleSystem _particle;
    private AudioSource _audioSource;

    public static event Action<CheckPoint> Reached = delegate(CheckPoint point) { };

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particle = GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _particle.Play();
        _audioSource.Play();
        Reached(this);
    }
}
