using System;
using System.Collections.Generic;
using Persistence;
using UnityEngine;

public class PersistentCollection : MonoBehaviour
{
    private readonly Dictionary<Guid, IPersist> _items = new Dictionary<Guid, IPersist>();

    private void Awake()
    {
        var children = GetComponentsInChildren<IPersist>();
        foreach (var child in children)
        {
            var guid = child.GetGuid();
            _items.Add(guid, child);
        }
    }

    public void Load(GameDataReader reader)
    {
        Debug.Log("Loading persistent collection " + gameObject.name);
        var dictionary = reader.ReadDictionary<object>();
        foreach (var entry in dictionary)
            if (_items.TryGetValue(entry.Key, out var item))
                item.Load(entry.Value);
    }

    public void Save(GameDataWriter writer)
    {
        Debug.Log("Saving persistent collection " + gameObject.name);
        var dictionary = new Dictionary<Guid, object>();
        foreach (var entry in _items) dictionary.Add(entry.Key, entry.Value.Save());

        writer.WriteDictionary(dictionary);
    }
}