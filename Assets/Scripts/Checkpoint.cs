using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float blinkTimer = 1.5f;
    [SerializeField] private GameObject blinkLight = null;

    public bool Activated { get; private set; }

    private ParticleSystem _particle;
    private AudioSource _audioSource;

    public static event Action<Checkpoint> Reached = delegate(Checkpoint point) { };

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particle = GetComponent<ParticleSystem>();
        blinkLight.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other1)
    {
        if (!Activated)
        {
            Activate();
        }
    }

    private void Activate()
    {
        _particle.Play();
        _audioSource.Play();
        Activated = true;
        Reached(this);
        StartCoroutine(BlinkLight());
    }

    private IEnumerator BlinkLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkTimer);
            blinkLight.SetActive(!blinkLight.activeSelf);
        }
    }
}
