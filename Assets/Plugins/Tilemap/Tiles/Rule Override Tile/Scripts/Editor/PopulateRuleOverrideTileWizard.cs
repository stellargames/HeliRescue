using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
    public class PopulateRuleOverideTileWizard : ScriptableWizard
    {
        public Texture2D m_spriteSet;

        private RuleOverrideTile m_tileset;

        [MenuItem("CONTEXT/RuleOverrideTile/Populate From Sprite Sheet")]
        private static void MenuOption(MenuCommand menuCommand)
        {
            CreateWizard(menuCommand.context as RuleOverrideTile);
        }

        [MenuItem("CONTEXT/RuleOverrideTile/Populate From Sprite Sheet", true)]
        private static bool MenuOptionValidation(MenuCommand menuCommand)
        {
            var tile = menuCommand.context as RuleOverrideTile;
            return tile.m_Tile && !tile.m_Advanced;
        }

        //[MenuItem("Assets/Generate Rule Tile Override")]
        public static void CreateWizard(RuleOverrideTile target)
        {
            var wizard =
                DisplayWizard<PopulateRuleOverideTileWizard>("Populate Override",
                    "Populate");
            wizard.m_tileset = target;
        }

        public static void CloneWizard(PopulateRuleOverideTileWizard oldWizard)
        {
            var wizard =
                DisplayWizard<PopulateRuleOverideTileWizard>("Populate Override",
                    "Populate");
            wizard.m_tileset = oldWizard.m_tileset;
            wizard.m_spriteSet = oldWizard.m_spriteSet;
        }

        private void OnWizardUpdate()
        {
            isValid = m_tileset != null && m_spriteSet != null;
        }

        private void OnWizardCreate()
        {
            try
            {
                Populate();
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Auto-populate failed!", ex.Message, "Ok");
                CloneWizard(this);
            }
        }

        /// <summary>
        ///     Attempts to populate the selected override tile using the chosen sprite.
        ///     The assumption here is that the sprite set selected by the user has the same
        ///     naming scheme as the original sprite. That is to say, they should both have the same
        ///     number
        ///     of sprites, each sprite ends in an underscore followed by a number, and that they are
        ///     intended to be equivalent in function.
        /// </summary>
        private void Populate()
        {
            var spriteSheet = AssetDatabase.GetAssetPath(m_spriteSet);
            var overrideSprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
                .OfType<Sprite>().ToArray();

            var finished = false;

            try
            {
                Undo.RecordObject(m_tileset, "Auto-populate " + m_tileset.name);

                foreach (var rule in m_tileset.m_Tile.m_TilingRules)
                foreach (var originalSprite in rule.m_Sprites)
                {
                    var spriteName = originalSprite.name;
                    var spriteNumber = Regex.Match(spriteName, @"_\d+$").Value;

                    var matchingOverrideSprite =
                        overrideSprites.First(
                            sprite => sprite.name.EndsWith(spriteNumber));

                    m_tileset[originalSprite] = matchingOverrideSprite;
                }

                finished = true;
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentOutOfRangeException("Sprite sheet mismatch", ex);
            }
            finally
            {
                // We handle the undo like this in case we end up catching more exceptions.
                // We want this to ALWAYS happen unless we complete the population.
                if (!finished) Undo.PerformUndo();
            }
        }
    }
}