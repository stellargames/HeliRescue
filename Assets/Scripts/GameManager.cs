using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 3f;

    private PlayerSpawnPoint _playerSpawnPoint;

    private void Awake()
    {
        _playerSpawnPoint = FindObjectOfType<PlayerSpawnPoint>();
    }


    private void Start()
    {
        SpawnPlayer();
    }

    private void OnEnable()
    {
        HelicopterCollision.HelicopterDestroyed += OnHelicopterDestroyed;
    }

    private void OnDisable()
    {
        HelicopterCollision.HelicopterDestroyed -= OnHelicopterDestroyed;
    }

    private void OnHelicopterDestroyed()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        _playerSpawnPoint.SpawnPlayer();
    }
}
