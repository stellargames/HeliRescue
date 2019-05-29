using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WaypointFollower : MonoBehaviour
{
    private enum EndBehaviour
    {
        JumpBack,
        Reverse,
        ReturnToStart,
        Destroy
    }

    private const float Precision = 0.01f;

    private bool _movingBackward;
    private WaypointNavigator _waypoints;

#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private WaypointComponent waypointComponent;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int startAtWaypoint = 0;
    [SerializeField] private EndBehaviour onLastWaypoint;
    [SerializeField] private bool lookAtTarget;
    [ConditionalHide("lookAtTarget", true)] [SerializeField]
    private bool smoothTurns;
#pragma warning restore 0649

    private void Awake()
    {
        if (waypointComponent == null)
        {
            waypointComponent = GetComponentInParent<WaypointComponent>();
        }

        if (waypointComponent == null) return;

        _waypoints = waypointComponent.GetNavigator();
        _waypoints.Select(startAtWaypoint);
        transform.position = _waypoints.Current;
    }

    private void Update()
    {
        if (_waypoints != null)
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        var step = speed * Time.deltaTime;
        transform.position =
            Vector3.MoveTowards(transform.position, _waypoints.Current, step);

        if (Vector3.Distance(transform.position, _waypoints.Current) < Precision)
        {
            NextWaypoint();
        }

        if (lookAtTarget)
        {
            LookAtTarget();
        }
    }

    private void LookAtTarget()
    {
        if (!smoothTurns || _waypoints.IsAtEnd || _waypoints.IsAtStart)
        {
            transform.right = _waypoints.Current - transform.position;
            return;
        }

        var target = CalculatePositionBetweenCurrentAndNextWaypoint();
        transform.right = target - transform.position;
    }

    private Vector3 CalculatePositionBetweenCurrentAndNextWaypoint()
    {
        var progress = CalculateProgressOnCurrentSegment();
        var next = _movingBackward
            ? _waypoints.PeekPrevious()
            : _waypoints.PeekNext();
        var target = Vector3.Lerp(_waypoints.Current, next, progress);
        return target;
    }

    private float CalculateProgressOnCurrentSegment()
    {
        var previous = _movingBackward
            ? _waypoints.PeekNext()
            : _waypoints.PeekPrevious();
        var currentSegmentLength = Vector3.Distance(previous, _waypoints.Current);
        var distanceTraveled = Vector3.Distance(previous, transform.position);
        var progress = distanceTraveled / currentSegmentLength;
        return progress;
    }

    private void NextWaypoint()
    {
        var moreWaypointsAvailable = _waypoints.Move(_movingBackward);
        if (moreWaypointsAvailable) return;

        switch (onLastWaypoint)
        {
            case EndBehaviour.Reverse:
                _movingBackward = !_movingBackward;
                break;

            case EndBehaviour.JumpBack:
                _waypoints.Reset();
                transform.position = _waypoints.Current;
                break;

            case EndBehaviour.ReturnToStart:
                _waypoints.Reset();
                break;

            case EndBehaviour.Destroy:
                Destroy(gameObject);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}