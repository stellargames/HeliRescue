using Cinemachine;
using Skytanet.SimpleDatabase;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject _vehicle;

    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void Load(SaveFile file)
    {
        var data = file.Get<PlayerData>("playerData");

        transform.position = data.position;
        transform.rotation = data.rotation;
    }

    public void Save(SaveFile file)
    {
        var data = new PlayerData();
        {
            data.position = _vehicle == null
                ? transform.position
                : _vehicle.transform.position;
            data.rotation = _vehicle == null
                ? transform.rotation
                : _vehicle.transform.rotation;
        }

        file.Set("playerData", data);
    }

    public void SpawnVehicle()
    {
        _vehicle = Instantiate(vehiclePrefab, transform);
        virtualCamera.m_Follow = _vehicle.transform;
        virtualCamera.m_LookAt = _vehicle.transform;
    }
}