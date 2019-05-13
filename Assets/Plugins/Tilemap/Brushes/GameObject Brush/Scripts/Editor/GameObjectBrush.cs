using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "GameObject Brush")]
    public class GameObjectBrush : GridBrushBase
    {
        [SerializeField] [HideInInspector] private Vector3Int m_Size;

        public GameObjectBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        public Vector3Int size
        {
            get => m_Size;
            set
            {
                m_Size = value;
                SizeUpdated();
            }
        }

        [field: SerializeField]
        [field: HideInInspector]
        public Vector3Int pivot { get; set; }

        [field: SerializeField]
        [field: HideInInspector]
        public BrushCell[] cells { get; private set; }

        public int cellCount => cells != null ? cells.Length : 0;

        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            this.pivot = pivot;
            SizeUpdated();
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget,
            Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            var min = position - pivot;
            var bounds = new BoundsInt(min, m_Size);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent,
            BrushCell cell)
        {
            if (cell.gameObject != null)
                SetSceneCell(grid, parent, position, cell.gameObject, cell.offset,
                    cell.scale, cell.orientation);
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget,
            Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            var min = position - pivot;
            var bounds = new BoundsInt(min, m_Size);
            BoxErase(gridLayout, brushTarget, bounds);
        }

        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            ClearSceneCell(grid, parent, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (brushTarget == null)
                return;

            foreach (var location in position.allPositionsWithin)
            {
                var local = location - position.min;
                var cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                PaintCell(gridLayout, location, brushTarget.transform, cell);
            }
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (brushTarget == null)
                return;

            foreach (var location in position.allPositionsWithin)
                EraseCell(gridLayout, location, brushTarget.transform);
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget,
            Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        public override void Rotate(RotationDirection direction,
            GridLayout.CellLayout layout)
        {
            var oldSize = m_Size;
            var oldCells = cells.Clone() as BrushCell[];
            size = new Vector3Int(oldSize.y, oldSize.x, oldSize.z);
            var oldBounds = new BoundsInt(Vector3Int.zero, oldSize);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newX = direction == RotationDirection.Clockwise
                    ? oldSize.y - oldPos.y - 1
                    : oldPos.y;
                var newY = direction == RotationDirection.Clockwise
                    ? oldPos.x
                    : oldSize.x - oldPos.x - 1;
                var toIndex = GetCellIndex(newX, newY, oldPos.z);
                var fromIndex = GetCellIndex(oldPos.x, oldPos.y, oldPos.z, oldSize.x,
                    oldSize.y, oldSize.z);
                cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotX = direction == RotationDirection.Clockwise
                ? oldSize.y - pivot.y - 1
                : pivot.y;
            var newPivotY = direction == RotationDirection.Clockwise
                ? pivot.x
                : oldSize.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, newPivotY, pivot.z);

            var rotation = Matrix4x4.TRS(Vector3.zero,
                Quaternion.Euler(0f, 0f,
                    direction == RotationDirection.Clockwise ? 90f : -90f), Vector3.one);
            var orientation = Quaternion.Euler(0f, 0f,
                direction == RotationDirection.Clockwise ? 90f : -90f);
            foreach (var cell in cells)
            {
                cell.offset = rotation * cell.offset;
                cell.orientation = cell.orientation * orientation;
            }
        }

        public override void Flip(FlipAxis flip, GridLayout.CellLayout layout)
        {
            if (flip == FlipAxis.X)
                FlipX();
            else
                FlipY();
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position, Vector3Int pickStart)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1),
                new Vector3Int(pickStart.x, pickStart.y, 0));

            foreach (var pos in position.allPositionsWithin)
            {
                var brushPosition =
                    new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCell(pos, brushPosition, gridLayout, brushTarget.transform);
            }
        }

        private void PickCell(Vector3Int position, Vector3Int brushPosition,
            GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var cellCenter = grid.LocalToWorld(
                    grid.CellToLocalInterpolated(position + new Vector3(.5f, .5f, .5f)));
                var go = GetObjectInCell(grid, parent, position);

                if (go != null)
                {
                    Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);

                    if (prefab)
                    {
                        SetGameObject(brushPosition, (GameObject) prefab);
                    }
                    else
                    {
                        var newInstance = Instantiate(go);
                        newInstance.hideFlags = HideFlags.HideAndDontSave;
                        SetGameObject(brushPosition, newInstance);
                    }

                    SetOffset(brushPosition, go.transform.position - cellCenter);
                    SetScale(brushPosition, go.transform.localScale);
                    SetOrientation(brushPosition, go.transform.localRotation);
                }
            }
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1),
                Vector3Int.zero);

            if (brushTarget != null)
                foreach (var pos in position.allPositionsWithin)
                {
                    var brushPosition =
                        new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                    PickCell(pos, brushPosition, gridLayout, brushTarget.transform);
                    ClearSceneCell(gridLayout, brushTarget.transform, brushPosition);
                }
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget,
            BoundsInt position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Paint(gridLayout, brushTarget, position.min);
            Reset();
        }

        public void Reset()
        {
            foreach (var cell in cells)
                if (cell.gameObject != null &&
                    !EditorUtility.IsPersistent(cell.gameObject))
                    DestroyImmediate(cell.gameObject);
            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
        }

        private void FlipX()
        {
            var oldCells = cells.Clone() as BrushCell[];
            var oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newX = m_Size.x - oldPos.x - 1;
                var toIndex = GetCellIndex(newX, oldPos.y, oldPos.z);
                var fromIndex = GetCellIndex(oldPos);
                cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotX = m_Size.x - pivot.x - 1;
            pivot = new Vector3Int(newPivotX, pivot.y, pivot.z);
            var flip = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(-1f, 1f, 1f));
            var orientation = Quaternion.Euler(0f, 0f, -180f);

            foreach (var cell in cells)
            {
                var oldOffset = cell.offset;
                cell.offset = flip * oldOffset;
                cell.orientation = cell.orientation * orientation;
            }
        }

        private void FlipY()
        {
            var oldCells = cells.Clone() as BrushCell[];
            var oldBounds = new BoundsInt(Vector3Int.zero, m_Size);

            foreach (var oldPos in oldBounds.allPositionsWithin)
            {
                var newY = m_Size.y - oldPos.y - 1;
                var toIndex = GetCellIndex(oldPos.x, newY, oldPos.z);
                var fromIndex = GetCellIndex(oldPos);
                cells[toIndex] = oldCells[fromIndex];
            }

            var newPivotY = m_Size.y - pivot.y - 1;
            pivot = new Vector3Int(pivot.x, newPivotY, pivot.z);
            var flip = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(1f, -1f, 1f));
            var orientation = Quaternion.Euler(0f, 0f, -180f);
            foreach (var cell in cells)
            {
                var oldOffset = cell.offset;
                cell.offset = flip * oldOffset;
                cell.orientation = cell.orientation * orientation;
            }
        }

        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            this.pivot = pivot;
            SizeUpdated();
        }

        public void SetGameObject(Vector3Int position, GameObject go)
        {
            if (ValidateCellPosition(position))
                cells[GetCellIndex(position)].gameObject = go;
        }

        public void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                cells[GetCellIndex(position)].offset = offset;
        }

        public void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                cells[GetCellIndex(position)].orientation = orientation;
        }

        public void SetScale(Vector3Int position, Vector3 scale)
        {
            if (ValidateCellPosition(position))
                cells[GetCellIndex(position)].scale = scale;
        }

        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_Size.x * y + m_Size.x * m_Size.y * z;
        }

        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }

        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return x % m_Size.x + m_Size.x * (y % m_Size.y) +
                   m_Size.x * m_Size.y * (z % m_Size.z);
        }

        private static GameObject GetObjectInCell(GridLayout grid, Transform parent,
            Vector3Int position)
        {
            var childCount = parent.childCount;
            var min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
            var max =
                grid.LocalToWorld(
                    grid.CellToLocalInterpolated(position + Vector3Int.one));

            // Infinite bounds on Z for 2D convenience
            min = new Vector3(min.x, min.y, float.MinValue);
            max = new Vector3(max.x, max.y, float.MaxValue);

            var bounds = new Bounds((max + min) * .5f, max - min);

            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                    return child.gameObject;
            }

            return null;
        }

        private bool ValidateCellPosition(Vector3Int position)
        {
            var valid =
                position.x >= 0 && position.x < size.x &&
                position.y >= 0 && position.y < size.y &&
                position.z >= 0 && position.z < size.z;
            if (!valid)
                throw new ArgumentException(string.Format(
                    "Position {0} is an invalid cell position. Valid range is between [{1}, {2}).",
                    position, Vector3Int.zero, size));
            return valid;
        }

        private void SizeUpdated()
        {
            cells = new BrushCell[m_Size.x * m_Size.y * m_Size.z];
            var bounds = new BoundsInt(Vector3Int.zero, m_Size);
            foreach (var pos in bounds.allPositionsWithin)
                cells[GetCellIndex(pos)] = new BrushCell();
        }

        private static void SetSceneCell(GridLayout grid, Transform parent,
            Vector3Int position, GameObject go, Vector3 offset, Vector3 scale,
            Quaternion orientation)
        {
            if (parent == null || go == null)
                return;

            GameObject instance = null;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject) PrefabUtility.InstantiatePrefab(go);
            }
            else
            {
                instance = Instantiate(go);
                instance.hideFlags = HideFlags.None;
                instance.name = go.name;
            }

            Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");
            instance.transform.SetParent(parent);
            instance.transform.position = grid.LocalToWorld(
                grid.CellToLocalInterpolated(
                    new Vector3Int(position.x, position.y, position.z) +
                    new Vector3(.5f, .5f, .5f)));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }

        private static void ClearSceneCell(GridLayout grid, Transform parent,
            Vector3Int position)
        {
            if (parent == null)
                return;

            var erased = GetObjectInCell(grid, parent,
                new Vector3Int(position.x, position.y, position.z));
            if (erased != null)
                Undo.DestroyObjectImmediate(erased);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            unchecked
            {
                foreach (var cell in cells) hash = hash * 33 + cell.GetHashCode();
            }

            return hash;
        }

        [Serializable]
        public class BrushCell
        {
            [SerializeField] private GameObject m_GameObject;

            [SerializeField] private Vector3 m_Offset = Vector3.zero;

            [SerializeField] private Quaternion m_Orientation = Quaternion.identity;

            [SerializeField] private Vector3 m_Scale = Vector3.one;

            public GameObject gameObject
            {
                get => m_GameObject;
                set => m_GameObject = value;
            }

            public Vector3 offset
            {
                get => m_Offset;
                set => m_Offset = value;
            }

            public Vector3 scale
            {
                get => m_Scale;
                set => m_Scale = value;
            }

            public Quaternion orientation
            {
                get => m_Orientation;
                set => m_Orientation = value;
            }

            public override int GetHashCode()
            {
                var hash = 0;
                unchecked
                {
                    hash = gameObject != null ? gameObject.GetInstanceID() : 0;
                    hash = hash * 33 + m_Offset.GetHashCode();
                    hash = hash * 33 + m_Scale.GetHashCode();
                    hash = hash * 33 + m_Orientation.GetHashCode();
                }

                return hash;
            }
        }
    }

    [CustomEditor(typeof(GameObjectBrush))]
    public class GameObjectBrushEditor : GridBrushEditorBase
    {
        public GameObjectBrush brush => target as GameObjectBrush;

        public override void OnPaintSceneGUI(GridLayout gridLayout,
            GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool,
            bool executing)
        {
            var gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
                gizmoRect = new BoundsInt(position.min - brush.pivot, brush.size);

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        public override void OnPaintInspectorGUI()
        {
            GUILayout.Label("Pick, paint and erase GameObject(s) in the scene.");
            GUILayout.Label("Limited to children of the currently selected GameObject.");
        }
    }
}