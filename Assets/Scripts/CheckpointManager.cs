using Persistence;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint[] _checkpoints;

    private void Awake()
    {
        _checkpoints = GetComponentsInChildren<Checkpoint>();
    }

    public void Load(GameDataReader reader)
    {
        foreach (var checkpoint in _checkpoints) checkpoint.Load(reader);
    }

    public void Save(GameDataWriter writer)
    {
        foreach (var checkpoint in _checkpoints) checkpoint.Save(writer);
    }
}