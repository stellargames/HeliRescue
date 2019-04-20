using Pooling;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Missile : PooledMonoBehaviour
{
    private Rigidbody2D _physicsBody;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform body;
    [SerializeField] private ParticleSystem exhaust;
    [SerializeField] private Explosion[] explosionPrefabs;
    [SerializeField] private float thrust = 20f;

    private void Awake()
    {
        _physicsBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        GetComponent<Collider2D>().enabled = true;
        body.gameObject.SetActive(true);
        exhaust.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        var direction = transform.right.normalized;
        _physicsBody.AddForce(direction * thrust, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var contact = other.GetContact(0);

        InstantiateExplosionPrefab(contact);
        PlayExplosionAudio();

        GetComponent<Collider2D>().enabled = false;
        body.gameObject.SetActive(false);
        exhaust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ReturnToPool(3f);
    }

    public void Launch(Vector2 direction, Vector2 velocity)
    {
        transform.right = direction;
        _physicsBody.AddForce(velocity * 0.5f, ForceMode2D.Impulse);
    }

    private void InstantiateExplosionPrefab(ContactPoint2D contact)
    {
        var explosionPrefab = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
        var explosionInstance =
            explosionPrefab.Get<Explosion>(contact.point, Quaternion.identity);
        explosionInstance.transform.up = contact.normal;
        explosionInstance.ReturnToPool(5f);
    }

    private void PlayExplosionAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (!audioSource || audioClips.Length == 0) return;

        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}