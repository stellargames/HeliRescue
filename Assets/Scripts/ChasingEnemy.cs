using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ChasingEnemy : MonoBehaviour
{
    private bool _activated;
    private AudioSource _audioSource;
    private Transform _currentTarget;
    private Fader _fader;
    [SerializeField] private Transform body;
    [SerializeField] private float chaseSpeed = 0.05f;

    private void Awake()
    {
        SetChildrenActive(false);
        if (body != null) _fader = body.GetComponent<Fader>();

        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_activated) return;

        if (_currentTarget == null)
            StartCoroutine(Deactivate());
        else
            transform.position =
                Vector3.MoveTowards(transform.position, _currentTarget.position,
                    chaseSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_activated && other.CompareTag("Player"))
        {
            _currentTarget = other.transform;
            Activate();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_activated) StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return Deactivate();
        Destroy(gameObject);
    }

    private void Activate()
    {
        _activated = true;
        SetChildrenActive(true);
        if (_fader) _fader.StartFadeIn();
        if (_audioSource) _audioSource.Play();
    }

    private IEnumerator Deactivate()
    {
        _activated = false;
        if (_fader) yield return _fader.FadeOut();
        SetChildrenActive(false);
    }

    private void SetChildrenActive(bool value)
    {
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(value);
    }
}