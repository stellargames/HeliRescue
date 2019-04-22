using System.Collections;
using Persistence;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameData _gameData;

    private int _loadedLevelBuildIndex;
    private string _saveFile;
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
    }

    private void Start()
    {
        _gameData = new GameData();
        _gameData.Load();
        _gameData.Player.SpawnHelicopter();
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
        StartCoroutine(DelayedRestart());
    }

    private IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(spawnDelay);
        yield return SceneManager.LoadSceneAsync(0);
        Start();
    }
}