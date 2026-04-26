using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using TextAsset = UnityEngine.TextCore.Text.TextAsset;

namespace InputIcons
{
    [CustomEditor(typeof(InputIconsUITKManagerSO))]
    public class InputIconsUITKManagerSOEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            /*if (GUILayout.Button("Prepare Style Sheet (then manually make a change and undo to save the style sheet)"))
            {
                InputIconsUITKManagerSO.HandleStylesPrepared();
            }

            if (GUILayout.Button("Add styles to Style Sheet"))
            {
                InputIconsUITKManagerSO.HandleStylesPrepared();
            }*/

            if (GUILayout.Button("Open Setup Window"))
            {
                InputIconsUITKSetupWindow.ShowWindow();
            }

            InputIconsUITKManagerSO manager = (InputIconsUITKManagerSO)target;

            EditorGUILayout.LabelField("UI Toolkit settings", EditorStyles.boldLabel);
            manager.uitkPanelSettings = (PanelSettings)EditorGUILayout.ObjectField("Panel Settings", manager.uitkPanelSettings, typeof(PanelSettings), false);
            manager.uitkTextSettings = (TextSettings)EditorGUILayout.ObjectField("Text Settings", manager.uitkTextSettings, typeof(TextSettings), false);
            manager.uitkTextStyleSheet = (TextStyleSheet)EditorGUILayout.ObjectField("Text Style Sheet", manager.uitkTextStyleSheet, typeof(TextStyleSheet), false);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Created Text Assets - Must have the same names as Sprite Assets", EditorStyles.boldLabel);
            manager.keyboardTextAsset = (TextAsset)EditorGUILayout.ObjectField("Keyboard Text Asset", manager.keyboardTextAsset, typeof(TextAsset), false);
            manager.nintendoProTextAsset = (TextAsset)EditorGUILayout.ObjectField("Nintendo Pro Text Asset", manager.nintendoProTextAsset, typeof(TextAsset), false);
            manager.ps3TextAsset = (TextAsset)EditorGUILayout.ObjectField("PS3 Text Asset", manager.ps3TextAsset, typeof(TextAsset), false);
            manager.ps4TextAsset = (TextAsset)EditorGUILayout.ObjectField("PS4 Text Asset", manager.ps4TextAsset, typeof(TextAsset), false);
            manager.ps5TextAsset = (TextAsset)EditorGUILayout.ObjectField("PS5 Text Asset", manager.ps5TextAsset, typeof(TextAsset), false);
            manager.xBoxTextAsset = (TextAsset)EditorGUILayout.ObjectField("XBox Text Asset", manager.xBoxTextAsset, typeof(TextAsset), false);

            manager.uiDocumentUpdateOptions = (InputIconsManagerSO.TextUpdateOptions)EditorGUILayout.EnumPopup(new GUIContent("Toolkit Update Setting",
                "Search and Update is reliable and updates all UI documents. This is recommended.\n\n" +
                "Via Input Icons Text Components requires you to add the II_UIDocument component to UI Documents which display Input Icons. " +
                "This method is more performant."),manager.uiDocumentUpdateOptions);
        }
    }
}
