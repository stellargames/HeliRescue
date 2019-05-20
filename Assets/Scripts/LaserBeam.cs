using UnityEngine;

[ExecuteInEditMode]
public class LaserBeam : MonoBehaviour
{
    private const float ScaleFactor = 1f / Mathf.PI;
    private const float PositionAdjustment = -1f / 14f;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[1];
    [SerializeField] private GameObject beam;
    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private float maximumDistance = 100f;

    private void Start()
    {
        if (layerMask == 0) layerMask = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        var myTransform = transform;
        var origin = myTransform.position + myTransform.up;
        var hitCount = Physics2D.RaycastNonAlloc(origin, myTransform.up, _hits,
            maximumDistance, layerMask);

        if (hitCount > 0)
        {
            var distance = _hits[0].distance + 1f;
            beam.transform.localScale = new Vector3(distance * ScaleFactor, 1, 1);
            var pos = beam.transform.localPosition;
            beam.transform.localPosition =
                new Vector3(pos.x, distance * PositionAdjustment, pos.z);
        }
    }

    public void SetBeamStatus(bool status)
    {
        beam.SetActive(status);
    }

    public bool GetBeamStatus()
    {
        return beam.activeSelf;
    }
}
