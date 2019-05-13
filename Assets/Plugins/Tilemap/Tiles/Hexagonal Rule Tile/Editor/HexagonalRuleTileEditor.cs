using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(HexagonalRuleTile), true)]
    [CanEditMultipleObjects]
    internal class HexagonalRuleTileEditor : RuleTileEditor
    {
        private static readonly Vector2[] s_PointedTopPositions =
        {
            new Vector2(2f, 1f), new Vector2(1.5f, 2f), new Vector2(0.5f, 2f),
            new Vector2(0f, 1f), new Vector2(0.5f, 0f), new Vector2(1.5f, 0f)
        };

        private static readonly int[] s_PointedTopArrows = {5, 8, 6, 3, 0, 2};

        private static readonly Vector2[] s_FlatTopPositions =
        {
            new Vector2(1f, 0f), new Vector2(2f, 0.5f), new Vector2(2f, 1.5f),
            new Vector2(1f, 2f), new Vector2(0f, 1.5f), new Vector2(0f, 0.5f)
        };

        private static readonly int[] s_FlatTopArrows = {1, 2, 8, 7, 6, 0};

        internal override void RuleMatrixOnGUI(RuleTile ruleTile, Rect rect,
            RuleTile.TilingRule tilingRule)
        {
            var hexTile = (HexagonalRuleTile) ruleTile;
            var flatTop = hexTile.m_FlatTop;

            Handles.color = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.2f)
                : new Color(0f, 0f, 0f, 0.2f);
            var w = rect.width / 3f;
            var h = rect.height / 3f;

            // Grid
            if (flatTop)
                for (var x = 0; x <= 3; x++)
                {
                    var left = rect.xMin + x * w;
                    var offset = x % 3 > 0 ? 0 : h / 2;
                    Handles.DrawLine(new Vector3(left, rect.yMin + offset),
                        new Vector3(left, rect.yMax - offset));

                    if (x < 3)
                    {
                        var noOffset = x % 2 > 0;
                        for (var y = 0; y < (noOffset ? 4 : 3); y++)
                        {
                            var top = rect.yMin + y * h + (noOffset ? 0 : h / 2);
                            Handles.DrawLine(new Vector3(left, top),
                                new Vector3(left + w, top));
                        }
                    }
                }
            else
                for (var y = 0; y <= 3; y++)
                {
                    var top = rect.yMin + y * h;
                    var offset = y % 3 > 0 ? 0 : w / 2;
                    Handles.DrawLine(new Vector3(rect.xMin + offset, top),
                        new Vector3(rect.xMax - offset, top));

                    if (y < 3)
                    {
                        var noOffset = y % 2 > 0;
                        for (var x = 0; x < (noOffset ? 4 : 3); x++)
                        {
                            var left = rect.xMin + x * w + (noOffset ? 0 : w / 2);
                            Handles.DrawLine(new Vector3(left, top),
                                new Vector3(left, top + h));
                        }
                    }
                }

            // Icons
            Handles.color = Color.white;
            for (var index = 0; index < hexTile.neighborCount; ++index)
            {
                var position = flatTop
                    ? s_FlatTopPositions[index]
                    : s_PointedTopPositions[index];
                var arrowIndex =
                    flatTop ? s_FlatTopArrows[index] : s_PointedTopArrows[index];
                var r = new Rect(rect.xMin + position.x * w, rect.yMin + position.y * h,
                    w - 1, h - 1);
                RuleOnGUI(r, arrowIndex, tilingRule.m_Neighbors[index]);
                RuleNeighborUpdate(r, tilingRule, index);
            }

            // Center
            {
                var r = new Rect(rect.xMin + w, rect.yMin + h, w - 1, h - 1);
                RuleTransformOnGUI(r, tilingRule.m_RuleTransform);
                RuleTransformUpdate(r, tilingRule);
            }
        }
    }
}