using Cinemachine;
using Skytanet.SimpleDatabase;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject _vehicle;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
#pragma warning restore 0649

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
        if (_vehicle == null) _vehicle = Instantiate(vehiclePrefab, transform);

        virtualCamera.m_Follow = _vehicle.transform;
        virtualCamera.m_LookAt = _vehicle.transform;
        if (Application.isMobilePlatform)
        {
            var framingTransposer =
                virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framingTransposer != null)
            {
                framingTransposer.m_CameraDistance = 80;
            }
        }
    }
}