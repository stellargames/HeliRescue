using System;
using System.Linq;
using UnityEngine;

public class LaserSequence : MonoBehaviour
{
    private int _currentIndex;
    private float _currentTime;
    [SerializeField] private float intervalSeconds = 3f;
    [SerializeField] private LaserBeam[] laserBeams;
    [SerializeField] private int modulo = 2;

    [SerializeField] private SequenceType sequenceType;

    private void Start()
    {
        SwitchBeams();
    }

    private void SwitchBeams()
    {
        switch (sequenceType)
        {
            case SequenceType.Modulo:
                SwitchModulo();
                break;

            case SequenceType.Sequential:
                SwitchSequential();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SwitchSequential()
    {
        var currentStatus = laserBeams.Last().GetBeamStatus();
        foreach (var laserBeam in laserBeams)
        {
            var nextStatus = laserBeam.GetBeamStatus();
            laserBeam.SetBeamStatus(currentStatus);
            currentStatus = nextStatus;
        }
    }

    private void SwitchModulo()
    {
        for (var i = 0; i < laserBeams.Length; i++)
            laserBeams[i].SetBeamStatus((i + _currentIndex) % modulo == 0);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= intervalSeconds)
        {
            _currentTime = 0f;
            _currentIndex = (_currentIndex + 1) % laserBeams.Length;
            SwitchBeams();
        }
    }

    private enum SequenceType
    {
        Sequential,
        Modulo
    }
}
