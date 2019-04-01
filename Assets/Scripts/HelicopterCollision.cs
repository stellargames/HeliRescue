using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterCollision : MonoBehaviour
{
    [SerializeField] private DestroyEffect[] explosionPrefabs = null;
    [SerializeField] private AudioClip[] audioClips;

    public static event Action HelicopterDestroyed = delegate {  };

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsBottomCollision(other)) return;

        var explosion = explosionPrefabs[Random.Range(0, explosionPrefabs.Length - 1)];
        Instantiate(explosion, transform.position, Quaternion.identity);

        DisableComponents();
        DisableChildren();

        GetComponent<Collider2D>().enabled = false;

        PlayExplosionAudio();

        Destroy(gameObject, 2f);

        HelicopterDestroyed();
    }

    private static bool IsBottomCollision(Collision2D other)
    {
        var contactNormal = other.GetContact(0).normal;
//        Debug.Log("Other: " + other.gameObject.name);
//        Debug.Log("contactNormal: " + contactNormal);
        return contactNormal.x <= 0.3f && contactNormal.y >= 0.9f;
    }

    private void PlayExplosionAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource || audioClips.Length == 0) return;

        audioSource.enabled = true;
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length - 1)]);
    }

    private void DisableComponents()
    {
        foreach (var component in gameObject.GetComponents<MonoBehaviour>())
        {
            component.enabled = false;
        }
    }

    private void DisableChildren()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
