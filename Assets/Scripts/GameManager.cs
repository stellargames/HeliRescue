using System.Collections;
using Persistence;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 3f;

    public static GameManager Instance { get; private set; }

    private int _loadedLevelBuildIndex;
    private string _saveFile;
    private GameData _gameData;

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
            Debug.Log("GameManager Awake");
        }
    }

    private void Start()
    {
        Debug.Log("GameManager Start");
        _gameData = new GameData();
        _gameData.Load();
        SpawnPlayer();
    }

    private void OnEnable()
    {
        Debug.Log("GameManager OnEnable");
        Checkpoint.Reached += CheckpointOnReached;
        HelicopterCollision.Exploded += OnHelicopterExploded;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            _gameData.Save();
        }
    }

    private void OnDisable()
    {
        Debug.Log("GameManager OnDisable");
        HelicopterCollision.Exploded -= OnHelicopterExploded;
        Checkpoint.Reached -= CheckpointOnReached;
    }

    private void CheckpointOnReached(Checkpoint checkpoint)
    {
        _gameData.Save();
    }

    private void OnHelicopterExploded()
    {
        Debug.Log("GameManager HelicopterExploded");
        StartCoroutine(DelayedRestart());
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(spawnDelay);
        yield return SceneManager.LoadSceneAsync(0);
        Start();
    }

    private void SpawnPlayer()
    {
        _gameData.PlayerSpawnPoint.SpawnPlayer();
    }
}
