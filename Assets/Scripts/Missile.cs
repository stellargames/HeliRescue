using UnityEngine;

public class Missile : MonoBehaviour
{
    private Rigidbody2D _missile;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform body;
    [SerializeField] private ParticleSystem exhaust;
    [SerializeField] private DestroyEffect[] explosionPrefabs;
    [SerializeField] private float thrust = 20f;

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

        GetComponent<Collider2D>().enabled = false;
        body.gameObject.SetActive(false);
        exhaust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(gameObject, 3f);
    }

    private void InstantiateExplosionPrefab(ContactPoint2D contact)
    {
        var explosionPrefab = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
        var explosionInstance =
            Instantiate(explosionPrefab, contact.point, Quaternion.identity);
        explosionInstance.transform.up = contact.normal;
    }

    private void PlayExplosionAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource || audioClips.Length == 0) return;

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
