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
        HelicopterCollision.Exploded += OnHelicopterExploded;
        CheckPoint.Reached += CheckPointOnReached;
    }

    private void CheckPointOnReached(CheckPoint checkpoint)
    {
        _playerSpawnPoint.transform.position = checkpoint.transform.position;
    }

    private void OnDisable()
    {
        CheckPoint.Reached -= CheckPointOnReached;
        HelicopterCollision.Exploded -= OnHelicopterExploded;
    }

    private void OnHelicopterExploded()
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
