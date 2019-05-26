using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator
{
    public WaypointNavigator(List<Vector3> waypoints)
    {
        _waypoints = waypoints;
    }

    private int _index;
    private readonly List<Vector3> _waypoints;
    public Vector3 Current => _waypoints[_index];

    public bool MoveNext()
    {
        if (IsAtEnd) return false;
        _index++;
        return _index < _waypoints.Count;
    }

    public bool MovePrevious()
    {
        if (IsAtStart) return false;
        _index--;
        return _index >= 0;
    }

    public bool Move(bool backward = false)
    {
        return backward ? MovePrevious() : MoveNext();
    }

    public Vector3 Peek(bool backward = false)
    {
        return backward ? PeekPrevious() : PeekNext();
    }

    public Vector3 PeekNext()
    {
        return _waypoints[_index + 1];
    }

    public Vector3 PeekPrevious()
    {
        return _waypoints[_index - 1];
    }

    public void Reset()
    {
        _index = 0;
    }

    public void Select(int index)
    {
        if (index < 0 || index >= _waypoints.Count)
            throw new ArgumentOutOfRangeException();
        _index = index;
    }

    public bool IsAtEnd => _index == _waypoints.Count - 1;
    public bool IsAtStart => _index == 0;
}
