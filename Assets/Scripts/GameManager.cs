using System.Collections;
using Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const bool Debug = true;
    private GameState _gameData;
    private int _loadedLevelBuildIndex;
    private Player _player;
    private bool _alive = true;

    [SerializeField] private float spawnDelay = 3f;

    private static GameManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _player = GetComponentInChildren<Player>();
    }

    private void Start()
    {
        _gameData = new GameState(_player);
        _gameData.Load();
        _player.SpawnVehicle();
        _alive = true;
    }

    private void OnEnable()
    {
        Checkpoint.Reached += CheckpointOnReached;
        HelicopterCollision.Exploded += OnHelicopterExploded;
    }

    private void OnDisable()
    {
        HelicopterCollision.Exploded -= OnHelicopterExploded;
        Checkpoint.Reached -= CheckpointOnReached;
    }

    private void CheckpointOnReached(Checkpoint checkpoint)
    {
        if (_alive) _gameData.Save();
    }

    private void OnHelicopterExploded()
    {
        if (!_alive) return;
        _alive = false;
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        // Wait for explosions to finish before restarting.
        yield return new WaitForSeconds(spawnDelay);
        // Re-load the current scene.
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        // Start a new game.
        Start();
    }
}