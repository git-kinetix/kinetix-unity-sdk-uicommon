// // ----------------------------------------------------------------------------
// // <copyright file="KinetixTranslationImporter.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Kinetix.UI.Common.Translation
{
    public class KinetixTranslationImporter : EditorWindow
    {
        private TextAsset csvFile;


        [MenuItem("Kinetix/UI/Kinetix Translation Importer")]
        private static void Open()
        {
            GetWindow<KinetixTranslationImporter>(true, "KinetixTranslationImporter", true);
        }

        private void OnGUI()
        {
            csvFile = EditorGUILayout.ObjectField("Select CSV File (tabs)", csvFile, typeof(TextAsset), false) as TextAsset;

            GUI.enabled = false;

            if (csvFile != null)
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Import"))
            {
                if (!Directory.Exists("Assets/Resources")) {
                    Directory.CreateDirectory("Assets/Resources");
                }

                if (!Directory.Exists("Assets/Resources/Kinetix")) {
                    Directory.CreateDirectory("Assets/Resources/Kinetix");
                }
                    
                if (!Directory.Exists("Assets/Resources/Kinetix/Translations")) {
                    Directory.CreateDirectory("Assets/Resources/Kinetix/Translations");
                }

                string moveResult = AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(csvFile), "Assets/Resources/" + TranslationConst.OVERRIDE_FILE_PATH + ".csv");

                if (moveResult == string.Empty) {
                    moveResult = "Successfully moved file";
                }

                Debug.Log(moveResult);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = csvFile;
            }
        }
    }
}
