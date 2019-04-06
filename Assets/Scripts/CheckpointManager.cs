using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint[] _checkpoints;

    private void Start()
    {
        _checkpoints = GetComponentsInChildren<Checkpoint>();
    }

}
