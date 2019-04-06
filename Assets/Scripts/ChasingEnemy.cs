using System.Collections;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
    private bool _activated;
    [SerializeField] private float fadeTime = 2f;

    private void Awake()
    {
        SetChildrenActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_activated && collider.CompareTag("Player"))
        {
            Activate();
        }
    }

    private void Activate()
    {
        _activated = true;
        SetChildrenActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        transform.localScale = Vector3.zero;
        var elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            yield return null;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
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
