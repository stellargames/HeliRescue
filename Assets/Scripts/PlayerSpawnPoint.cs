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

    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        _camera.m_Follow = player.transform;
        _camera.m_LookAt = player.transform;
    }

}
