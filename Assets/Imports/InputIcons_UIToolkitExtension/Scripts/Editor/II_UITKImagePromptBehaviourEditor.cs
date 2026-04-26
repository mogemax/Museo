using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputIcons.II_ImagePrompt;

namespace InputIcons
{
    [CustomEditor(typeof(II_UITKImagePromptBehaviour))]
    public class II_UITKImagePromptBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            II_UITKImagePromptBehaviour imagePromptBehaviour = (II_UITKImagePromptBehaviour)target;
            List<II_UITKImagePromptBehaviour.UITKImagePromptData> savedPromptData = II_UITKImagePromptBehaviour.UITKImagePromptData.CloneList(imagePromptBehaviour.spritePromptDatas);

            EditorGUI.BeginChangeCheck();

            foreach (II_UITKImagePromptBehaviour.UITKImagePromptData sData in savedPromptData.ToList())
            {
                if (sData.actionReference != null)
                {
                    EditorGUILayout.LabelField(sData.actionReference.action.name, II_SpritePromptEditor.HeaderStyle.Get(), GUILayout.Width(150));
                    EditorGUILayout.Space(5);
                }

                sData.actionReference = (InputActionReference)EditorGUILayout.ObjectField("Action Reference", sData.actionReference, typeof(InputActionReference), false);

                if (sData.actionReference != null)
                {
                    sData.bindingSearchStrategy = (ImagePromptData.BindingSearchStrategy)EditorGUILayout.EnumPopup("Search Binding By", sData.bindingSearchStrategy);

                    if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingType)
                    {
                        sData.deviceType = (InputIconsUtility.DeviceType)EditorGUILayout.EnumPopup("Device Type", sData.deviceType);
                        sData.advancedMode = EditorGUILayout.Toggle("Advanced Mode", sData.advancedMode);

                        if (sData.advancedMode)
                        {
                            DrawAdvancedModeControls(sData);
                        }
                        else
                        {
                            II_SpritePromptEditor.DrawNormalModeControls(sData);
                            II_SpritePromptEditor.DrawDeviceSpritesNormalMode(sData);
                        }
                    }
                    else if (sData.bindingSearchStrategy == ImagePromptData.BindingSearchStrategy.BindingIndex)
                    {
                        II_SpritePromptEditor.DrawSearchByBindingIndexFields(sData);
                    }
                }

                sData.rootElementID = EditorGUILayout.TextField("(Optional) Root Element Name", sData.rootElementID);
                sData.imageID = EditorGUILayout.TextField("Visual Element Name", sData.imageID);

                if (string.IsNullOrEmpty(sData.imageID))
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.HelpBox("Assign a Visual Element name to display binding in scene", MessageType.Warning);
                }

                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                {
                    sData.optionalKeyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("(Optional) Keyboard Icon Set", sData.optionalKeyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                }

                if (sData.deviceType == InputIconsUtility.DeviceType.Auto || sData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                {
                    sData.optionalGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("(Optional) Gamepad Icon Set", sData.optionalGamepadIconSet, typeof(InputIconSetGamepadSO), true);
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    savedPromptData.Remove(sData);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);
                II_SpritePromptEditor.DrawUILine(Color.grey);
            }

            if (GUILayout.Button("Add"))
            {
                if (savedPromptData.Count > 0)
                    savedPromptData.Add(new II_UITKImagePromptBehaviour.UITKImagePromptData(savedPromptData[savedPromptData.Count - 1]));
                else
                    savedPromptData.Add(new II_UITKImagePromptBehaviour.UITKImagePromptData());
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Prompt Data Changed");
                imagePromptBehaviour.spritePromptDatas = new List<II_UITKImagePromptBehaviour.UITKImagePromptData>(savedPromptData);
                imagePromptBehaviour.OnValidate();
                EditorUtility.SetDirty(imagePromptBehaviour);
            }
        }

        private void DrawAdvancedModeControls(II_UITKImagePromptBehaviour.UITKImagePromptData sData)
        {
            int widthTitles = 140;
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetDefaultColor()));
            EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
            EditorGUILayout.LabelField("Control Scheme Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
            EditorGUILayout.LabelField("Composite Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
            EditorGUILayout.LabelField("Binding Types", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
            EditorGUILayout.LabelField("Available Binding Index", EditorStyles.boldLabel, GUILayout.Width(widthTitles));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetKeyboardColor()));
            II_SpritePromptEditor.DrawKeyboardAdvanced(sData);
            II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteKeyboardAdvanced(), widthTitles, sData, false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(II_SpritePromptEditor.BackgroundStyle.Get(II_SpritePromptEditor.BackgroundStyle.GetGamepadColor()));
            II_SpritePromptEditor.DrawGamepadAdvanced(sData);
            II_SpritePromptEditor.DrawSpriteField(sData.GetSpriteGamepadAdvanced(), widthTitles, sData, true);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.Space(50);
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
        }
    }
}