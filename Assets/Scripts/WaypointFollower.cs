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
        float step = speed * Time.deltaTime;
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
        if (_waypoints.IsAtEnd || _waypoints.IsAtStart) return;

        Vector3 direction;
        if (smoothTurns)
        {
            Vector3 target = CalculatePositionBetweenCurrentAndNextWaypoint();
            direction = target - transform.position;
        }
        else
        {
            direction = _waypoints.Current - transform.position;
        }

        transform.right = _movingBackward ? -direction : direction;
    }

    private Vector3 CalculatePositionBetweenCurrentAndNextWaypoint()
    {
        float progress = CalculateProgressOnCurrentSegment();
        Vector3 next = _movingBackward
            ? _waypoints.PeekPrevious()
            : _waypoints.PeekNext();
        Vector3 target = Vector3.Lerp(_waypoints.Current, next, progress);
        return target;
    }

    private float CalculateProgressOnCurrentSegment()
    {
        Vector3 previous = _movingBackward
            ? _waypoints.PeekNext()
            : _waypoints.PeekPrevious();
        float currentSegmentLength = Vector3.Distance(previous, _waypoints.Current);
        float distanceTraveled = Vector3.Distance(previous, transform.position);
        float progress = distanceTraveled / currentSegmentLength;
        return progress;
    }

    private void NextWaypoint()
    {
        bool moreWaypointsAvailable = _waypoints.Move(_movingBackward);
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