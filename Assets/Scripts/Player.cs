using System;
using Cinemachine;
using Persistence;
using Skytanet.SimpleDatabase;
using UnityEngine;

public class Player : MonoBehaviour, IPersist
{
    private GameObject _vehicle;
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public Guid Guid => new Guid("563946d3-999e-5634-188d-faac16ed370f");

    public void Load(SaveFile file)
    {
        var data = file.Get<PlayerData>(Guid.ToString());
        if (data.inventoryJson == null) return;

        transform.position = data.position;
        transform.rotation = data.rotation;
        JsonUtility.FromJsonOverwrite(data.inventoryJson, inventory);
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
            data.inventoryJson = JsonUtility.ToJson(inventory);
        }
        ;

        file.Set(Guid.ToString(), data);
    }

    public void SpawnVehicle()
    {
        _vehicle = Instantiate(vehiclePrefab, transform);
        virtualCamera.m_Follow = _vehicle.transform;
        virtualCamera.m_LookAt = _vehicle.transform;
    }
}