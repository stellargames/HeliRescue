using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 3f;

    public static GameManager Instance { get; private set; }

    private int _loadedLevelBuildIndex;

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
        SpawnPlayer();
    }

    private void OnEnable()
    {
        HelicopterCollision.Exploded += OnHelicopterExploded;
    }

    private void OnDisable()
    {
        HelicopterCollision.Exploded -= OnHelicopterExploded;
    }

    private void OnHelicopterExploded()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        yield return SceneManager.LoadSceneAsync(0);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        FindObjectOfType<PlayerSpawnPoint>().SpawnPlayer();
    }
}
