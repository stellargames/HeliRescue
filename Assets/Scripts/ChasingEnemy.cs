using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CircleCollider2D))]
public class ChasingEnemy : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 0.05f;
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private Transform body = null;

    private bool _activated;
    private Transform _currentTarget;
    private Coroutine _currentFadeRoutine;

    private void Awake()
    {
        SetChildrenActive(false);
        Assert.IsNotNull(body,
            $"Please assign a body to the ChasingEnemy {gameObject.name}");
        body.localScale = Vector3.zero;
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
        _currentFadeRoutine = StartCoroutine(FadeBodyIn());
    }

    private IEnumerator Deactivate()
    {
        _activated = false;
        if (_currentFadeRoutine != null) StopCoroutine(_currentFadeRoutine);
        yield return FadeBodyOut();
        SetChildrenActive(false);
    }

    private IEnumerator FadeBodyIn()
    {
        var currentScale = body.transform.localScale;
        while (currentScale.sqrMagnitude < 3f)
        {
            currentScale += Time.deltaTime * fadeSpeed * Vector3.one;
            body.transform.localScale = currentScale;
            yield return null;
        }
    }

    private IEnumerator FadeBodyOut()
    {
        var currentScale = body.transform.localScale;
        while (currentScale.sqrMagnitude > 0.1f)
        {
            currentScale -= Time.deltaTime * fadeSpeed * Vector3.one;
            body.transform.localScale = currentScale;
            yield return null;
        }
    }

    private void SetChildrenActive(bool value)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}
