#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class SnapToGround : MonoBehaviour
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private LayerMask layerMask = 0;

    private void Update()
    {
        if (layerMask == 0) layerMask = LayerMask.GetMask("Default");

        var position = transform.position;

        var origin = new Vector2(position.x, position.y);
        var direction = -transform.up;
        var hitInfo = Physics2D.Raycast(origin, direction, distance, layerMask);
        if (hitInfo)
            transform.position = new Vector3(position.x, hitInfo.point.y, position.z);
    }
}
#endif
