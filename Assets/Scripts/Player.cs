using Cinemachine;
using Skytanet.SimpleDatabase;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MobileCameraDistance = 80;

    private GameObject _vehicle;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
#pragma warning restore 0649

    public void Load(SaveFile file)
    {
        var playerData = file.Get<PlayerData>("playerData");

        Transform playerTransform = transform;
        playerTransform.position = playerData.position;
        playerTransform.rotation = playerData.rotation;
    }

    public void Save(SaveFile file)
    {
        Transform playerTransform = transform;
        var playerData = new PlayerData();
        {
            playerData.position = _vehicle == null
                ? playerTransform.position
                : _vehicle.transform.position;
            playerData.rotation = _vehicle == null
                ? playerTransform.rotation
                : _vehicle.transform.rotation;
        }

        file.Set("playerData", playerData);
    }

    public void SpawnVehicle()
    {
        if (_vehicle == null)
            _vehicle = Instantiate(vehiclePrefab, transform);

        virtualCamera.m_Follow = _vehicle.transform;
        virtualCamera.m_LookAt = _vehicle.transform;

        if (Application.isMobilePlatform)
        {
            var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framingTransposer != null)
            {
                framingTransposer.m_CameraDistance = MobileCameraDistance;
            }
        }
    }
}