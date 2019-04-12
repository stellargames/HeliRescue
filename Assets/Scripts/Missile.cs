using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float thrust = 20f;
    [SerializeField] private Transform body = null;
    [SerializeField] private ParticleSystem exhaust = null;
    [SerializeField] private DestroyEffect[] explosionPrefabs = null;
    [SerializeField] private AudioClip[] audioClips = null;

    private Rigidbody2D _missile;

    private void Awake()
    {
        _missile = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var direction = transform.right.normalized;
        _missile.AddForce(direction * thrust, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var contact = other.GetContact(0);

        InstantiateExplosionPrefab(contact);
        PlayExplosionAudio();

        var delay = 64f / contact.relativeVelocity.sqrMagnitude;
        StartCoroutine(DelayedDestroy(delay));
    }

    private void InstantiateExplosionPrefab(ContactPoint2D contact)
    {
        var explosionPrefab = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
        Debug.Log(explosionPrefab.name);
        var explosionInstance =
            Instantiate(explosionPrefab, contact.point, Quaternion.identity);
        var explosionTransform = explosionInstance.transform;
        explosionTransform.up = contact.normal;
        explosionTransform.position += Vector3.back;
    }

    private void PlayExplosionAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource || audioClips.Length == 0) return;

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        GetComponent<Collider2D>().enabled = false;
        body.gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);
        exhaust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(gameObject, 3f);
    }
}
