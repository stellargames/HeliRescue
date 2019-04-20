using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private SpriteRenderer _bodySprite;
    private Coroutine _currentFadeRoutine;
    [SerializeField] private float fadeSpeed = 0.5f;

    private void Awake()
    {
        _bodySprite = GetComponent<SpriteRenderer>();
        MakeBodyInvisible();
    }

    private void MakeBodyInvisible()
    {
        var bodySpriteColor = _bodySprite.color;
        bodySpriteColor.a = 0;
        _bodySprite.color = bodySpriteColor;

        transform.localScale = Vector3.zero;
    }

    public void StartFadeIn()
    {
        if (_currentFadeRoutine != null) StopCoroutine(_currentFadeRoutine);
        _currentFadeRoutine = StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        if (_currentFadeRoutine != null) StopCoroutine(_currentFadeRoutine);
        _currentFadeRoutine = StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        if (_currentFadeRoutine != null) StopCoroutine(_currentFadeRoutine);

        var currentColor = _bodySprite.color;
        var currentScale = transform.localScale;

        while (currentScale.sqrMagnitude < 3f)
        {
            currentScale += Time.deltaTime * fadeSpeed * Vector3.one;
            transform.localScale = currentScale;

            currentColor.a += Time.deltaTime;
            _bodySprite.color = currentColor;

            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        if (_currentFadeRoutine != null) StopCoroutine(_currentFadeRoutine);

        var currentColor = _bodySprite.color;
        var currentScale = transform.localScale;

        while (currentScale.sqrMagnitude > 0.1f)
        {
            currentScale -= Time.deltaTime * fadeSpeed * Vector3.one;
            transform.localScale = currentScale;

            currentColor.a -= Time.deltaTime;
            _bodySprite.color = currentColor;

            yield return null;
        }
    }
}