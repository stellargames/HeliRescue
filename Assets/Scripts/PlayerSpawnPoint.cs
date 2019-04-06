using Cinemachine;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private CinemachineVirtualCamera _camera;

    private void Awake()
    {
        _camera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        Checkpoint.Reached += CheckPointOnReached;
    }

    private void OnDisable()
    {
        Checkpoint.Reached -= CheckPointOnReached;
    }

    private void CheckPointOnReached(Checkpoint checkpoint)
    {
        transform.position = checkpoint.transform.position;
    }

    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        _camera.m_Follow = player.transform;
        _camera.m_LookAt = player.transform;
    }

}
