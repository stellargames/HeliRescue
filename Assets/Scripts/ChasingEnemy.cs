using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CircleCollider2D))]
public class ChasingEnemy : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 0.05f;
    [SerializeField] private Transform body = null;

    private bool _activated;
    private Transform _currentTarget;
    private Fader _fader;

    private void Awake()
    {
        SetChildrenActive(false);
        Assert.IsNotNull(body,
            $"Please assign a body to the ChasingEnemy {gameObject.name}");
        _fader = body.GetComponent<Fader>();
    }

    private void Update()
    {
        if (!_activated) return;

        if (_currentTarget == null)
        {
            StartCoroutine(Deactivate());
        }
        else
        {
            transform.position =
                Vector3.MoveTowards(transform.position, _currentTarget.position,
                    chaseSpeed);
        }
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
        if (_activated)
        {
            StartCoroutine(Die());
        }
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
        _fader.StartFadeIn();
    }

    private IEnumerator Deactivate()
    {
        _activated = false;
        yield return _fader.FadeOut();
        SetChildrenActive(false);
    }

    private void SetChildrenActive(bool value)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}
