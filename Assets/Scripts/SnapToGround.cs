#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGround : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private float distance = 5f;

    private void Update()
    {
        if (layerMask == 0)
        {
            layerMask = LayerMask.GetMask("Default");
        }

        var t = transform;
        var p = t.position;
        t.rotation = Quaternion.identity;

        var origin = new Vector2(p.x, p.y);
        var hitInfo = Physics2D.Raycast(origin, Vector2.down, distance, layerMask);
        if (hitInfo)
        {
            transform.position = new Vector3(p.x, hitInfo.point.y, p.z);
        }
    }
}
#endif
