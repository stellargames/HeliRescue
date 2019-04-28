using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : MonoBehaviour
{
    private GameObject _vehicle;

    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public Position Position
    {
        get =>
            _vehicle == null
                ? new Position
                {
                    position = transform.position,
                    rotation = transform.rotation
                }
                : new Position
                {
                    position = _vehicle.transform.position,
                    rotation = _vehicle.transform.rotation
                };
        set
        {
            transform.position = value.position;
            transform.rotation = value.rotation;
        }
    }

    public void SpawnVehicle()
    {
        _vehicle = Instantiate(vehiclePrefab, transform);
        virtualCamera.m_Follow = _vehicle.transform;
        virtualCamera.m_LookAt = _vehicle.transform;
    }
}