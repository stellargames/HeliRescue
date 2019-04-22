using Persistence;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : MonoBehaviour
{
    private Inventory _inventory;
    private GameObject _vehicle;

    [SerializeField] private GameObject helicopterPrefab;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }

    public void SpawnHelicopter()
    {
        _vehicle = Instantiate(helicopterPrefab, transform);
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(_vehicle.transform.position);
        _inventory.Save(writer);
    }

    public void Load(GameDataReader reader)
    {
        transform.position = reader.ReadVector3();
        _inventory.Load(reader);
    }
}