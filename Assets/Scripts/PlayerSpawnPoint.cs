using Cinemachine;
using Persistence;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        virtualCamera.m_Follow = player.transform;
        virtualCamera.m_LookAt = player.transform;
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(transform.position);
    }

    public void Load(GameDataReader reader)
    {
        transform.position = reader.ReadVector3();
    }
}
