using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterCollision : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private DestroyEffect[] explosionPrefabs;

    public static event Action Exploded = delegate { };

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsLandingGearCollision(other) && !IsEnemy(other.gameObject)) return;

        var explosion = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
        Instantiate(explosion, transform.position, Quaternion.identity);
        PlayExplosionAudio();

        GetComponent<HelicopterController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        DisableChildren();
        Destroy(gameObject, 2f);

        Exploded.Invoke();
    }

    private static bool IsEnemy(GameObject gameObject)
    {
        return gameObject.CompareTag("Enemy");
    }

    private static bool IsLandingGearCollision(Collision2D other)
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
        audioSource.Stop();

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }

    private void DisableChildren()
    {
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
