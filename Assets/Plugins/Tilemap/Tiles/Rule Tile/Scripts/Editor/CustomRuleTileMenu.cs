namespace UnityEditor
{
    internal static class CustomRuleTileMenu
    {
        [MenuItem("Assets/Create/Custom Rule Tile Script", false, 89)]
        private static void CreateCustomRuleTile()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                "Assets/Tilemap/Tiles/Rule Tile/ScriptTemplates/NewCustomRuleTile.cs.txt",
                "NewCustomRuleTile.cs");
        }
    }
}