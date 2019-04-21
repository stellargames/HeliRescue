using System;
using System.Collections.Generic;
using Persistence;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private readonly Dictionary<Guid, Checkpoint> _checkpoints = new Dictionary<Guid, Checkpoint>();

    private void Awake()
    {
        var checkpoints = GetComponentsInChildren<Checkpoint>();
        foreach (var checkpoint in checkpoints)
        {
            var guidComponent = checkpoint.GetComponent<GuidComponent>();
            if (guidComponent != null) _checkpoints.Add(guidComponent.GetGuid(), checkpoint);
        }
    }

    public void Load(GameDataReader reader)
    {
        var count = reader.ReadInt();
        while (count-- > 0)
        {
            var guid = reader.ReadGuid();
            _checkpoints.TryGetValue(guid, out var checkpoint);
            if (checkpoint == null) checkpoint = new Checkpoint();
            checkpoint.Load(reader);
        }
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(_checkpoints.Count);
        foreach (var entry in _checkpoints)
        {
            writer.Write(entry.Key);
            entry.Value.Save(writer);
        }
    }
}