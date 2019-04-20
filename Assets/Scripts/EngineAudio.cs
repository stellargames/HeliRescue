using Interfaces;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(IHaveThrottle))]
public class EngineAudio : MonoBehaviour
{
    private AudioSource _audioSource;
    private IHaveThrottle _vehicle;
    [SerializeField] private float pitchMultiplier = 0.01f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _vehicle = GetComponent<IHaveThrottle>();
    }

    private void Update()
    {
        _audioSource.pitch = 1 + _vehicle.Throttle * pitchMultiplier;
    }

    private void OnDisable()
    {
        _audioSource.Stop();
    }
}