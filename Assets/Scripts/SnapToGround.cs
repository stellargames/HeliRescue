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

        var position = new Vector2(transform.position.x, transform.position.y);


        transform.rotation = Quaternion.identity;

        var hitInfo = Physics2D.Raycast(position, Vector2.down, distance, layerMask);
        if (hitInfo)
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y,
                transform.position.z);
        }
    }
}
#endif
