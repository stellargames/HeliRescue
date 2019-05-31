using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTilemap : MonoBehaviour
{
#pragma warning disable 0649   // Backing fields are assigned through the Inspector
    [SerializeField] private LayerMask layerMask;
#pragma warning restore 0649

    private Tilemap _tilemap;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!InLayerMask(collision)) return;
        var hitPosition = Vector3.zero;
        foreach (var hit in collision.contacts)
        {
            hitPosition.x = hit.point.x + 0.1f * hit.normal.x;
            hitPosition.y = hit.point.y + 0.1f * hit.normal.y;
            var tilePosition = _tilemap.WorldToCell(hitPosition);
            _tilemap.SetTile(tilePosition, null);
        }
    }

    private bool InLayerMask(Collision2D collision)
    {
        return ((1 << collision.gameObject.layer) & layerMask) != 0;
    }
}
