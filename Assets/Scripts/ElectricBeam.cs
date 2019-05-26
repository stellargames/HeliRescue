using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class ElectricBeam : MonoBehaviour
{
    private const float TimerTimeOut = 0.05f;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[1];

    private LineRenderer _lineRenderer;
    private BoxCollider2D _collider;
    private float _timer;

    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private float maximumDistance = 100f;
    [SerializeField] private float segmentLength = 2f;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (layerMask == 0) layerMask = LayerMask.GetMask("Default");
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer > TimerTimeOut)
        {
            _timer = 0;
            var beamLength = GetCollisionDistance();
            SetLineRendererPositions(beamLength);
            SetColliderShape(beamLength);
        }
    }

    private void SetColliderShape(float beamLength)
    {
        if (!_collider) return;

        _collider.size = new Vector2(_collider.size.x, beamLength);
        _collider.offset = new Vector2(_collider.offset.x, beamLength * 0.5f);
    }

    private float GetCollisionDistance()
    {
        var hitCount = Physics2D.RaycastNonAlloc(transform.position, transform.up, _hits,
            maximumDistance, layerMask);
        return hitCount > 0 ? _hits[0].distance : maximumDistance;
    }

    private void SetLineRendererPositions(float distance)
    {
        var numberOfSegments = Mathf.RoundToInt(distance / segmentLength);
        var segmentVector = distance / numberOfSegments * Vector3.up;

        _lineRenderer.positionCount = numberOfSegments + 1;
        _lineRenderer.SetPosition(0, Vector3.zero);

        var nextPosition = Vector3.zero;
        for (var i = 1; i < _lineRenderer.positionCount; i++)
        {
            nextPosition += segmentVector;
            var randomization = segmentLength * 0.5f * Random.insideUnitSphere;
            _lineRenderer.SetPosition(i, nextPosition + randomization);
        }
    }
}
