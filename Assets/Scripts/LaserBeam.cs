using UnityEngine;

[ExecuteInEditMode]
public class LaserBeam : MonoBehaviour
{
    private const float ScaleFactor = 1f / Mathf.PI;
    private const float PositionAdjustment = -1f / 14f;
    [SerializeField] private GameObject beam;
    [SerializeField] private LayerMask layerMask = 0;

    private void Start()
    {
        beam.SetActive(Application.isEditor);
        if (layerMask == 0) layerMask = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        var hits = new RaycastHit2D[1];

        var origin = transform.position + transform.up;
        var hitCount = Physics2D.RaycastNonAlloc(origin, transform.up, hits,
            100f, layerMask);

        if (hitCount > 0)
        {
            var distance = hits[0].distance + 1f;
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
}
