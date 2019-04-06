using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint[] _checkpoints;

    private void Start()
    {
        _checkpoints = GetComponentsInChildren<Checkpoint>();
    }

    public Checkpoint GetLastActivatedCheckpoint()
    {
        var lastCheckpoint = _checkpoints.LastOrDefault(t => t.Activated);
        Assert.IsNotNull(lastCheckpoint, "There needs to be at least one passed checkpoint.");
        return lastCheckpoint;
    }
}
