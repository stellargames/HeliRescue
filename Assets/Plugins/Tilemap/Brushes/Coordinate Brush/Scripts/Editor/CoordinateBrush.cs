using UnityEngine;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "Coordinate Brush")]
    [CreateAssetMenu(fileName = "New Coordinate Brush",
        menuName = "Brushes/Coordinate Brush")]
    public class CoordinateBrush : GridBrush
    {
        public int z;

        public override void Paint(GridLayout grid, GameObject brushTarget,
            Vector3Int position)
        {
            var zPosition = new Vector3Int(position.x, position.y, z);
            base.Paint(grid, brushTarget, zPosition);
        }

        public override void Erase(GridLayout grid, GameObject brushTarget,
            Vector3Int position)
        {
            var zPosition = new Vector3Int(position.x, position.y, z);
            base.Erase(grid, brushTarget, zPosition);
        }

        public override void FloodFill(GridLayout grid, GameObject brushTarget,
            Vector3Int position)
        {
            var zPosition = new Vector3Int(position.x, position.y, z);
            base.FloodFill(grid, brushTarget, zPosition);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position)
        {
            var zPosition = new Vector3Int(position.x, position.y, z);
            position.position = zPosition;
            base.BoxFill(gridLayout, brushTarget, position);
        }
    }

    [CustomEditor(typeof(CoordinateBrush))]
    public class CoordinateBrushEditor : GridBrushEditor
    {
        private CoordinateBrush coordinateBrush => target as CoordinateBrush;

        public override void PaintPreview(GridLayout grid, GameObject brushTarget,
            Vector3Int position)
        {
            var zPosition = new Vector3Int(position.x, position.y, coordinateBrush.z);
            base.PaintPreview(grid, brushTarget, zPosition);
        }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget,
            BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            if (coordinateBrush.z != 0)
            {
                var zPosition = new Vector3Int(position.min.x, position.min.y,
                    coordinateBrush.z);
                var newPosition = new BoundsInt(zPosition, position.size);
                Vector3[] cellLocals =
                {
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.min.y,
                        newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.min.y,
                        newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.max.y,
                        newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.max.y,
                        newPosition.min.z))
                };

                Handles.color = Color.blue;
                var i = 0;
                for (var j = cellLocals.Length - 1; i < cellLocals.Length; j = i++)
                    Handles.DrawLine(cellLocals[j], cellLocals[i]);
            }

            var labelText = "Pos: " +
                            new Vector3Int(position.x, position.y, coordinateBrush.z);
            if (position.size.x > 1 || position.size.y > 1)
                labelText += " Size: " + new Vector2Int(position.size.x, position.size.y);

            Handles.Label(
                grid.CellToWorld(
                    new Vector3Int(position.x, position.y, coordinateBrush.z)),
                labelText);
        }
    }
}
