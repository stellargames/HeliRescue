using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricBeam : MonoBehaviour
{
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[1];

    private LineRenderer _lineRenderer;
    private float _timer;
    private readonly float _timerTimeOut = 0.05f;
    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private float maximumDistance = 100f;
    [SerializeField] private float segmentLength = 2f;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if (layerMask == 0) layerMask = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _timerTimeOut)
        {
            _timer = 0;
            var beamLength = GetCollisionDistance();
            SetLineRendererPositions(beamLength);
        }
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
