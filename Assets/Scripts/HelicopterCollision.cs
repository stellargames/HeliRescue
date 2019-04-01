using UnityEngine;

public class HelicopterCollision : MonoBehaviour
{
    [SerializeField] private DestroyEffect[] explosionPrefabs = null;
    [SerializeField] private AudioClip[] audioClips;

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject);
        var contactNormal = other.GetContact(0).normal;
        Debug.Log("At : " + contactNormal);

        if (contactNormal.x <= 0.3f && contactNormal.y >= 0.9f) return;

        var explosion = explosionPrefabs[Random.Range(0, explosionPrefabs.Length - 1)];
        Instantiate(explosion, transform.position, Quaternion.identity);

        DisableComponents();
        DisableChildren();

        PlayExplosionAudio();

        Destroy(gameObject, 3f);
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
