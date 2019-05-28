using Persistence;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const bool Debug = true;
    private GameState _gameData;
    private int _loadedLevelBuildIndex;
    private Player _player;

    [SerializeField] private float spawnDelay = 3f;

    private static GameManager Instance { get; set; }

    private void Awake()
    {
        Assert.raiseExceptions = true;
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
        _gameData.Save();
    }

    private void OnHelicopterExploded()
    {
        Invoke(nameof(Restart), spawnDelay);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Start();
    }
}