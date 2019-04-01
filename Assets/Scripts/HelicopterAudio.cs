using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(HelicopterController))]
public class HelicopterAudio : MonoBehaviour
{
    [SerializeField] private float pitchMultiplier = 0.01f;

    private AudioSource _audioSource;
    private HelicopterController _helicopterController;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _helicopterController = GetComponent<HelicopterController>();
    }

    private void Update()
    {
        _audioSource.pitch = 1 + _helicopterController.Throttle * pitchMultiplier;
    }
}
