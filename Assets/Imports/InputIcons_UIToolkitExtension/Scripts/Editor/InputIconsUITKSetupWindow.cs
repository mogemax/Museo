using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine.TextCore.Text;
using System;
using TextAsset = UnityEngine.TextCore.Text.TextAsset;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.TextCore;
using System.Linq;

namespace InputIcons
{
    public class InputIconsUITKSetupWindow : EditorWindow
    {
        private enum SetupStep
        {
            Prerequisites,
            UIToolkitConfiguration,
            SpriteAssetCreation,
            TestingValidation,
            Help
        }

        private SetupStep currentStep = SetupStep.Prerequisites;
        private Vector2 scrollPos;
        private float leftPanelWidth = 250f;
        private bool showHelpText = false;

        // Styles
        private GUIStyle headerStyle;
        private GUIStyle stepButtonStyle;
        private GUIStyle textStyle;
        private GUIStyle textStyleYellow;
        private GUIStyle textStyleBold;
        private GUIStyle textStyleGreen;

        private InputIconsUITKManagerSO manager;

        [MenuItem("Tools/Input Icons/Input Icons UI Toolkit Setup", priority = 2)]
        public static void ShowWindow()
        {
            const int width = 1000;
            const int height = 600;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            EditorWindow window = GetWindow<InputIconsUITKSetupWindow>("Input Icons UI Toolkit Setup");
            window.position = new Rect(x, y, width, height);
        }

        protected void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            manager = InputIconsUITKManagerSO.Instance;

            // Load saved values
            var data = EditorPrefs.GetString("InputIconsUITKSetupWindow", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
        }

        protected void OnDisable()
        {
            // Save values
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("InputIconsUITKSetupWindow", data);
        }


        private void InitializeStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    margin = new RectOffset(10, 10, 10, 10),
                    wordWrap = true
                };

                stepButtonStyle = new GUIStyle(EditorStyles.miniButton)
                {
                    fixedHeight = 40,
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(5, 5, 2, 2),
                    padding = new RectOffset(10, 10, 5, 5),
                    wordWrap = true
                };

                textStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    richText = true
                };

                textStyleYellow = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    normal = { textColor = Color.yellow }
                };

                textStyleBold = new GUIStyle(EditorStyles.boldLabel)
                {
                    wordWrap = true
                };

                textStyleGreen = new GUIStyle(EditorStyles.boldLabel)
                {
                    wordWrap = true,
                    normal = { textColor = new Color(0.2f, 0.8f, 0.2f) }
                };
            }
        }

        private void OnGUI()
        {
            InitializeStyles();

            EditorGUILayout.BeginHorizontal();

            // Left navigation panel
            DrawNavigationPanel();

            // Vertical line separator
            DrawUILineVertical(Color.gray);

            // Main content area
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Manager field at the top
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("UI Toolkit Manager", EditorStyles.toolbarButton);
            InputIconsUITKManagerSO newManagerSelection = (InputIconsUITKManagerSO)EditorGUILayout.ObjectField(manager, typeof(InputIconsUITKManagerSO), false);
            if (newManagerSelection != manager && newManagerSelection != null)
            {
                InputIconsUITKManagerSO.Instance = newManagerSelection;
                manager = newManagerSelection;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Help text toggle
            showHelpText = EditorGUILayout.ToggleLeft("Show Additional Help Text", showHelpText);
            EditorGUILayout.Space(10);

            // Draw current section content
            switch (currentStep)
            {
                case SetupStep.Prerequisites:
                    DrawPrerequisitesContent();
                    break;
                case SetupStep.UIToolkitConfiguration:
                    DrawUIToolkitConfigurationContent();
                    break;
                case SetupStep.SpriteAssetCreation:
                    DrawSpriteAssetCreationContent();
                    break;
                case SetupStep.TestingValidation:
                    DrawTestingValidationContent();
                    break;
                case SetupStep.Help:
                    DrawHelpContent();
                    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (manager != null)
            {
                EditorUtility.SetDirty(manager);
            }
        }

        private void DrawNavigationPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(leftPanelWidth));

            GUILayout.Label("UI Toolkit Setup", headerStyle);
            EditorGUILayout.Space(10);

            DrawStepButton("1. Prerequisites", SetupStep.Prerequisites,
                "Verify main Input Icons setup and dependencies");

            DrawStepButton("2. UI Toolkit Config", SetupStep.UIToolkitConfiguration,
                "Configure UI Toolkit assets and settings");

            DrawStepButton("3. Sprite Creation", SetupStep.SpriteAssetCreation,
                "Create and configure sprite assets");

            DrawStepButton("4. Testing", SetupStep.TestingValidation,
                "Test setup and validate functionality");

            EditorGUILayout.Space(5);
            DrawUILine(Color.gray);
            EditorGUILayout.Space(5);

            DrawStepButton("Help & Support", SetupStep.Help,
                "Documentation and troubleshooting");

            GUILayout.FlexibleSpace();

            EditorGUILayout.Space(10);
            DrawUILine(Color.gray);
            EditorGUILayout.Space(5);

            // Asset Store Review section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Enjoying Input Icons?", EditorStyles.boldLabel);
            if (GUILayout.Button("Leave a Review on Asset Store"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/gui/input-icons-for-input-system-213736#reviews");
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        private void DrawStepButton(string title, SetupStep step, string tooltip)
        {
            bool isCurrentStep = currentStep == step;
            GUI.backgroundColor = isCurrentStep ? Color.gray : Color.white;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (GUILayout.Button(new GUIContent(title, tooltip), stepButtonStyle))
                currentStep = step;

            EditorGUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
        }

        private void DrawPrerequisitesContent()
        {
            GUILayout.Label("Prerequisites Check", headerStyle);

            if (showHelpText)
            {
                EditorGUILayout.HelpBox(
                    "The UI Toolkit extension requires the main Input Icons package to be properly configured first. " +
                    "We'll verify all dependencies are met before proceeding with the UI Toolkit setup.",
                    MessageType.Info);
            }

            // Check main Input Icons setup
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Main Input Icons Setup", textStyleBold);

            bool hasMainManager = InputIconsManagerSO.Instance != null;
            bool hasConfigurator = InputIconSetConfiguratorSO.Instance != null;
            bool hasSpritePacker = HasSpriteAssets();

            DrawStatusCheck("Input Icons Manager", hasMainManager);
            DrawStatusCheck("Icon Set Configurator", hasConfigurator);
            DrawStatusCheck("Sprite Assets Created", hasSpritePacker);

            if (!hasMainManager || !hasConfigurator)
            {
                EditorGUILayout.HelpBox(
                    "Main Input Icons setup is incomplete. Please complete the main Input Icons setup first using " +
                    "Tools → Input Icons → Input Icons Setup",
                    MessageType.Warning);

                if (GUILayout.Button("Open Main Input Icons Setup"))
                {
                    InputIconsSetupWindow.ShowWindow();
                }
            }
            else if (!hasSpritePacker)
            {
                EditorGUILayout.HelpBox(
                    "Sprite assets haven't been created yet. Please create them in the main Input Icons setup.",
                    MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("All prerequisites met! Ready to proceed.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            // Manager assignment
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("UI Toolkit Manager", textStyleBold);

            if (manager == null)
            {
                EditorGUILayout.HelpBox("Please assign or create a UI Toolkit Manager.", MessageType.Warning);

                if (GUILayout.Button("Create New UI Toolkit Manager"))
                {
                    manager = CreateUITKManager();
                    InputIconsUITKManagerSO.Instance = manager;
                }
            }
            else
            {
                EditorGUILayout.LabelField("UI Toolkit Manager assigned successfully.", textStyle);

                // Update options
                if (showHelpText)
                {
                    EditorGUILayout.HelpBox(
                        "Update Setting determines how UI Documents are refreshed when input devices change:\n" +
                        "• Search and Update: Automatically finds all UI Documents (recommended)\n" +
                        "• Via Components: Requires II_UIDocument component on each UI Document (more performant)",
                        MessageType.Info);
                }

                manager.uiDocumentUpdateOptions = (InputIconsManagerSO.TextUpdateOptions)EditorGUILayout.EnumPopup(
                    new GUIContent("Update Setting", "How UI Documents should be updated when input devices change"),
                    manager.uiDocumentUpdateOptions);
            }

            EditorGUILayout.EndVertical();

            // Navigation hint
            if (hasMainManager && hasConfigurator && hasSpritePacker && manager != null)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("✓ Prerequisites Complete", textStyleBold);
                EditorGUILayout.LabelField("Click 'UI Toolkit Config' to continue setup.", textStyle);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawUIToolkitConfigurationContent()
        {
            GUILayout.Label("UI Toolkit Configuration", headerStyle);

            if (showHelpText)
            {
                EditorGUILayout.HelpBox(
                    "Configure the UI Toolkit assets that will handle text styling and panel settings. " +
                    "These assets work together to display input icons in UI Toolkit interfaces.",
                    MessageType.Info);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("UI Toolkit Assets", textStyleBold);
            EditorGUILayout.LabelField("These are example assets - you can create and use your own. The key requirement is that your Text Settings (referenced in Panel Settings) must reference your Text Style Sheet for icons to appear in text.", textStyle);

            EditorGUILayout.Space(5);
            manager.uitkThemeStyleSheet = (ThemeStyleSheet)EditorGUILayout.ObjectField(
                "Theme Style Sheet", manager.uitkThemeStyleSheet, typeof(ThemeStyleSheet), false);

            manager.uitkPanelSettings = (PanelSettings)EditorGUILayout.ObjectField(
                "Panel Settings", manager.uitkPanelSettings, typeof(PanelSettings), false);

            // Highlight Text Settings as critical
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Settings", textStyleGreen, GUILayout.Width(120));
            manager.uitkTextSettings = (TextSettings)EditorGUILayout.ObjectField(
                manager.uitkTextSettings, typeof(TextSettings), false);
            EditorGUILayout.EndHorizontal();

            // Highlight Text Style Sheet as most critical
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Style Sheet", textStyleGreen, GUILayout.Width(120));
            manager.uitkTextStyleSheet = (TextStyleSheet)EditorGUILayout.ObjectField(
                manager.uitkTextStyleSheet, typeof(TextStyleSheet), false);
            EditorGUILayout.EndHorizontal();

            if (showHelpText && manager.uitkTextStyleSheet != null)
            {
                EditorGUILayout.HelpBox(
                    "The Text Style Sheet stores the styles for each input binding (like <style=gameplay/jump>). " +
                    "It must be referenced in your Text Settings for icons to appear.",
                    MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Sprite Asset Path", manager.defaultSpriteAssetPath);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Configuration button
            bool canConfigure = manager.uitkPanelSettings != null && manager.uitkTextSettings != null && manager.uitkTextStyleSheet != null;

            EditorGUI.BeginDisabledGroup(!canConfigure);
            if (GUILayout.Button("Configure Assets (Link Everything Together)"))
            {
                SetupPanelTextAndTextStyleSheet();
                EditorUtility.DisplayDialog("Configuration Complete",
                    "UI Toolkit assets have been configured and linked together successfully.", "OK");
            }
            EditorGUI.EndDisabledGroup();

            if (!canConfigure)
            {
                EditorGUILayout.HelpBox(
                    "Please assign all required assets above before configuring. Remember: The Text Settings assigned to your Panel Settings must reference the Text Style Sheet for input icons to display properly in text elements.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "This will cross-reference the assets to ensure they're properly linked together for input icon display.", MessageType.Info);
            }

            // Style setup
            EditorGUILayout.Space(15);
            DrawStyleSetupSection();
        }

        private void DrawStyleSetupSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Input Action Styles", textStyleBold);
            EditorGUILayout.LabelField(
                "Create styles for each input action so you can use tags like <style=gameplay/jump> in your UI Toolkit text.",
                textStyle);

            if (manager.uitkTextStyleSheet == null)
            {
                EditorGUILayout.HelpBox("Assign a Text Style Sheet first.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(5);

            // Quick setup option
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Quick Setup (Recommended)", textStyleBold);
            EditorGUILayout.LabelField("Automatic setup with recompilation - no manual steps needed.", textStyle);

            if (GUILayout.Button("1. Prepare Style Sheet"))
            {
                InputIconsUITKManagerSO.HandleStylesPrepared();
                CompilationPipeline.RequestScriptCompilation();
            }

            if (!EditorApplication.isCompiling)
            {
                if (GUILayout.Button("2. Add Style Entries"))
                {
                    InputIconsUITKManagerSO.HandleStylesAdded();
                    CompilationPipeline.RequestScriptCompilation();
                    EditorUtility.DisplayDialog("Styles Added",
                        "Input action styles have been added to your Text Style Sheet successfully!", "OK");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Waiting for compilation...", textStyleYellow);
            }
            EditorGUILayout.EndVertical();

            // Manual setup option (advanced)
            if (showHelpText)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Manual Setup (Advanced)", textStyleBold);
                EditorGUILayout.LabelField("Faster but requires manual style sheet update between steps.", textStyle);

                if (GUILayout.Button("1. Prepare Style Sheet (Manual)"))
                {
                    InputIconsUITKManagerSO.HandleStylesPrepared();
                    EditorGUIUtility.PingObject(manager.uitkTextStyleSheet);
                }

                EditorGUILayout.HelpBox(
                    "After clicking the button above, make a small change in the Text Style Sheet inspector and undo it to save.",
                    MessageType.Info);

                if (GUILayout.Button("2. Add Style Entries (Manual)"))
                {
                    InputIconsUITKManagerSO.HandleStylesAdded();
                    EditorUtility.DisplayDialog("Styles Added",
                        "Input action styles have been added successfully!", "OK");
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSpriteAssetCreationContent()
        {
            GUILayout.Label("Sprite Asset Creation", headerStyle);

            if (showHelpText)
            {
                EditorGUILayout.HelpBox(
                    "UI Toolkit needs special Text Sprite Assets that contain your input icons. " +
                    "These are created from the sprite atlases generated in the main Input Icons setup.",
                    MessageType.Info);
            }

            // Prerequisites check
            if (!HasSpriteAssets())
            {
                EditorGUILayout.HelpBox(
                    "Sprite atlases not found. Please complete the main Input Icons setup and create sprite assets first.",
                    MessageType.Warning);

                if (GUILayout.Button("Open Main Input Icons Setup"))
                {
                    InputIconsSetupWindow.ShowWindow();
                }
                return;
            }

            // Asset creation section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Text Sprite Asset Creation", textStyleBold);
            EditorGUILayout.LabelField(
                "Convert your sprite atlases into UI Toolkit-compatible text sprite assets.", textStyle);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Create Text Sprite Assets"))
            {
                InputIconsUITKSpritePacker.CreateSpriteAssetsOfIconSets();
                EditorUtility.DisplayDialog("Assets Created",
                    "Text sprite assets have been created successfully!", "OK");
            }

            if (GUILayout.Button("Auto-Assign Created Assets"))
            {
                TryToAutomaticallyAssignSpriteAssets();
                EditorUtility.DisplayDialog("Auto-Assignment Complete",
                    "Attempted to automatically assign text sprite assets. Check the assignments below.", "OK");
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Asset assignments
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Text Sprite Asset Assignments", textStyleBold);
            EditorGUILayout.LabelField("These assets correspond to your different controller types:", textStyle);

            EditorGUILayout.Space(5);
            manager.keyboardTextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "Keyboard", manager.keyboardTextAsset, typeof(TextAsset), false);
            manager.ps3TextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "PlayStation 3", manager.ps3TextAsset, typeof(TextAsset), false);
            manager.ps4TextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "PlayStation 4", manager.ps4TextAsset, typeof(TextAsset), false);
            manager.ps5TextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "PlayStation 5", manager.ps5TextAsset, typeof(TextAsset), false);
            manager.nintendoProTextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "Nintendo Switch", manager.nintendoProTextAsset, typeof(TextAsset), false);
            manager.xBoxTextAsset = (TextAsset)EditorGUILayout.ObjectField(
                "Xbox", manager.xBoxTextAsset, typeof(TextAsset), false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Organization tools
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Asset Organization", textStyleBold);

            if (GUILayout.Button("Organize Assets (Move & Rename)"))
            {
                MoveAndRenameSpriteAssets();
                EditorUtility.DisplayDialog("Organization Complete",
                    "Text sprite assets have been moved and renamed to match your icon set names.", "OK");
            }

            if (GUILayout.Button("Optimize Glyph Positions"))
            {
                CorrectPositionOfGlyphs();
                EditorUtility.DisplayDialog("Optimization Complete",
                    "Glyph positions have been optimized for better alignment.", "OK");
            }

            if (showHelpText)
            {
                EditorGUILayout.HelpBox(
                    "• Organize Assets: Moves assets to the correct folder and renames them to match icon set names\n" +
                    "• Optimize Glyph Positions: Improves vertical alignment of icons in text",
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTestingValidationContent()
        {
            GUILayout.Label("Testing & Validation", headerStyle);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Setup Validation", textStyleBold);

            // Check setup completeness
            bool hasManager = manager != null;
            bool hasAssets = manager != null && manager.uitkTextStyleSheet != null && manager.uitkTextSettings != null;
            bool hasSpriteAssets = HasTextSpriteAssets();

            DrawStatusCheck("UI Toolkit Manager", hasManager);
            DrawStatusCheck("UI Toolkit Assets Configured", hasAssets);
            DrawStatusCheck("Text Sprite Assets Created", hasSpriteAssets);

            if (hasManager && hasAssets && hasSpriteAssets)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("✓ Setup Complete! UI Toolkit is ready to display input icons.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Setup incomplete. Please complete the previous steps.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Usage instructions
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("How to Use Input Icons in UI Toolkit", textStyleBold);
            EditorGUILayout.LabelField(
                "There are two main ways to display input icons in UI Toolkit:\n\n" +
                "1. In Text Elements (using style tags):\n" +
                "   Example: Press <style=gameplay/jump> to jump\n" +
                "   Result: Press [SPACE] to jump\n" +
                "   Format: <style=ActionMapName/ActionName>\n" +
                "   For composite bindings: <style=ActionMapName/ActionName/PartName>\n\n" +
                "2. On Visual Elements (using components):\n" +
                "   Add the II_UITKImagePromptBehaviour component to the GameObject\n" +
                "   that has the UI Document attached. Configure it to target specific\n" +
                "   visual elements by their element IDs to display input prompts as images.",
                textStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Test scene button
            if (GUILayout.Button("Open Test Scene (if available)"))
            {
                // Try to find and open a test scene
                string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
                bool foundTestScene = false;

                foreach (string guid in sceneGuids)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath).ToLower();

                    if (sceneName.Contains("uitk") || sceneName.Contains("toolkit") || sceneName.Contains("test"))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                        foundTestScene = true;
                        break;
                    }
                }

                if (!foundTestScene)
                {
                    EditorUtility.DisplayDialog("No Test Scene Found",
                        "No UI Toolkit test scene was found in the project.", "OK");
                }
            }
        }

        private void DrawHelpContent()
        {
            GUILayout.Label("Help & Support", headerStyle);

            // Common issues
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Common Issues & Solutions", textStyleBold);

            EditorGUILayout.LabelField("Icons not showing:", textStyleBold);
            EditorGUILayout.LabelField("• Verify Text Style Sheet is assigned to Text Settings", textStyle);
            EditorGUILayout.LabelField("• Check that sprite assets are properly assigned", textStyle);
            EditorGUILayout.LabelField("• Ensure style names match your Input Action names", textStyle);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Setup errors:", textStyleBold);
            EditorGUILayout.LabelField("• Complete main Input Icons setup first", textStyle);
            EditorGUILayout.LabelField("• Check console for missing reference errors", textStyle);
            EditorGUILayout.LabelField("• Try recreating Text Sprite Assets", textStyle);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Advanced tools
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Advanced Tools", textStyleBold);

            EditorGUILayout.HelpBox(
                "Use this button to remove all Input Icon styles from the Text Style Sheet if you need to start over.",
                MessageType.Warning);

            if (GUILayout.Button("Remove All Input Icon Styles"))
            {
                if (EditorUtility.DisplayDialog("Remove Styles",
                    "This will remove all Input Icon styles from the Text Style Sheet. Continue?",
                    "Yes, Remove", "Cancel"))
                {
                    InputIconsUITKManagerSO.RemoveAllStyleSheetEntries();
                    EditorUtility.DisplayDialog("Styles Removed",
                        "All Input Icon styles have been removed from the Text Style Sheet.", "OK");
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Support section
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Additional Support", textStyleBold);

            if (GUILayout.Button("Open Main Input Icons Setup"))
            {
                InputIconsSetupWindow.ShowWindow();
            }

            if (GUILayout.Button("Contact Support"))
            {
                string subject = UnityEngine.Networking.UnityWebRequest.EscapeURL("Input Icons UI Toolkit Support").Replace("+", "%20");
                string body = UnityEngine.Networking.UnityWebRequest.EscapeURL("Hello,\n\nI need help with Input Icons UI Toolkit setup:\n\n").Replace("+", "%20");
                Application.OpenURL($"mailto:tobias.froihofer@gmx.at?subject={subject}&body={body}");
            }

            EditorGUILayout.EndVertical();
        }

        // Helper methods
        private void DrawStatusCheck(string label, bool status)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(status ? "✓" : "✗", GUILayout.Width(20));
            EditorGUILayout.LabelField(label, status ? textStyle : textStyleYellow);
            EditorGUILayout.EndHorizontal();
        }

        private bool HasSpriteAssets()
        {
            if (InputIconsManagerSO.Instance == null) return false;

            string spritePath = InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH;
            return !string.IsNullOrEmpty(spritePath) && Directory.Exists(spritePath) &&
                   Directory.GetFiles(spritePath, "*.asset").Length > 0;
        }

        private bool HasTextSpriteAssets()
        {
            return manager != null && (
                manager.keyboardTextAsset != null ||
                manager.ps3TextAsset != null ||
                manager.ps4TextAsset != null ||
                manager.ps5TextAsset != null ||
                manager.nintendoProTextAsset != null ||
                manager.xBoxTextAsset != null
            );
        }

        private InputIconsUITKManagerSO CreateUITKManager()
        {
            // Create manager in Resources folder
            string resourcesPath = "Assets/Resources/InputIcons";
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }

            InputIconsUITKManagerSO newManager = ScriptableObject.CreateInstance<InputIconsUITKManagerSO>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{resourcesPath}/InputIconsUITKManager.asset");
            AssetDatabase.CreateAsset(newManager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(newManager);
            return newManager;
        }

        // Existing helper methods (preserved from original)
        private void SetupPanelTextAndTextStyleSheet()
        {
            Debug.Log("Configuring UI Toolkit assets...");

            if (manager.uitkThemeStyleSheet != null && manager.uitkPanelSettings != null)
                manager.uitkPanelSettings.themeStyleSheet = manager.uitkThemeStyleSheet;

            if (manager.uitkTextSettings != null && manager.uitkPanelSettings != null)
                manager.uitkPanelSettings.textSettings = (PanelTextSettings)manager.uitkTextSettings;

            if (manager.uitkTextStyleSheet != null && manager.uitkTextSettings != null)
                manager.uitkTextSettings.defaultStyleSheet = manager.uitkTextStyleSheet;

            if (manager.uitkTextSettings != null)
            {
                manager.uitkTextSettings.styleSheetsResourcePath = GetStyleSheetPath(manager);
                manager.uitkTextSettings.defaultSpriteAssetPath = manager.defaultSpriteAssetPath;
            }

            EditorUtility.SetDirty(manager.uitkPanelSettings);
            EditorUtility.SetDirty(manager.uitkTextSettings);
            EditorUtility.SetDirty(manager.uitkTextStyleSheet);

            Debug.Log("UI Toolkit assets configured successfully.");
        }

        private string GetStyleSheetPath(InputIconsUITKManagerSO manager)
        {
            string fullPath = AssetDatabase.GetAssetPath(manager.uitkTextStyleSheet);
            string fileName = manager.uitkTextStyleSheet.name + ".asset";

            // Remove the Assets/ prefix if it exists
            if (fullPath.StartsWith("Assets/"))
            {
                fullPath = fullPath.Substring("Assets/".Length);
            }

            // Get the path without the filename
            string pathWithoutFile = fullPath.Substring(0, fullPath.Length - fileName.Length);

            // Check if the path contains Resources/
            int resourcesIndex = pathWithoutFile.LastIndexOf("Resources/");
            if (resourcesIndex != -1)
            {
                // If in Resources folder, only keep the path after Resources/
                pathWithoutFile = pathWithoutFile.Substring(resourcesIndex + "Resources/".Length);
            }

            return pathWithoutFile;
        }

        private void TryToAutomaticallyAssignSpriteAssets()
        {
            if (InputIconsManagerSO.Instance == null)
                return;

            List<InputIconSetBasicSO> iconSOs = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();

            Debug.Log("Searching for UI Toolkit sprite assets in " + InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH);
            List<SpriteAsset> spriteAssets = new List<SpriteAsset>();
            string[] filePaths = Directory.GetFiles(InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH);

            foreach (string filePath in filePaths)
            {
                if (Path.GetExtension(filePath) == ".asset")
                {
                    SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath<SpriteAsset>(filePath);
                    if (spriteAsset != null)
                    {
                        spriteAssets.Add(spriteAsset);
                    }
                }
            }

            manager.keyboardTextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.keyboardIconSet, spriteAssets);
            manager.nintendoProTextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.switchIconSet, spriteAssets);
            manager.ps3TextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.ps3IconSet, spriteAssets);
            manager.ps4TextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.ps4IconSet, spriteAssets);
            manager.ps5TextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.ps5IconSet, spriteAssets);
            manager.xBoxTextAsset = TryToAssignTextAsset(InputIconSetConfiguratorSO.Instance.xBoxIconSet, spriteAssets);
        }

        private SpriteAsset TryToAssignTextAsset(InputIconSetBasicSO iconSet, List<SpriteAsset> spriteAssets)
        {
            if (iconSet == null) return null;

            for (int i = 0; i < spriteAssets.Count; i++)
            {
                if (spriteAssets[i].name.Contains(iconSet.iconSetName))
                {
                    return spriteAssets[i];
                }
            }
            return null;
        }

        private void MoveAndRenameSpriteAssets()
        {
            string destinationPath = AssetDatabase.GetAssetPath(manager);
            destinationPath = destinationPath.Replace("InputIcons/" + manager.name + ".asset", "");
            destinationPath += "/" + manager.uitkTextSettings.defaultSpriteAssetPath;

            if (!Directory.Exists(destinationPath))
            {
                Debug.Log("Creating sprite asset folder...");
                Directory.CreateDirectory(destinationPath);
                AssetDatabase.Refresh();
            }

            MoveAsset(manager.keyboardTextAsset, destinationPath);
            MoveAsset(manager.nintendoProTextAsset, destinationPath);
            MoveAsset(manager.ps3TextAsset, destinationPath);
            MoveAsset(manager.ps4TextAsset, destinationPath);
            MoveAsset(manager.ps5TextAsset, destinationPath);
            MoveAsset(manager.xBoxTextAsset, destinationPath);
            AssetDatabase.Refresh();

            if (InputIconSetConfiguratorSO.Instance != null)
            {
                RenameAsset(manager.keyboardTextAsset, InputIconSetConfiguratorSO.Instance.keyboardIconSet?.iconSetName);
                RenameAsset(manager.nintendoProTextAsset, InputIconSetConfiguratorSO.Instance.switchIconSet?.iconSetName);
                RenameAsset(manager.ps3TextAsset, InputIconSetConfiguratorSO.Instance.ps3IconSet?.iconSetName);
                RenameAsset(manager.ps4TextAsset, InputIconSetConfiguratorSO.Instance.ps4IconSet?.iconSetName);
                RenameAsset(manager.ps5TextAsset, InputIconSetConfiguratorSO.Instance.ps5IconSet?.iconSetName);
                RenameAsset(manager.xBoxTextAsset, InputIconSetConfiguratorSO.Instance.xBoxIconSet?.iconSetName);
            }

            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(manager.keyboardTextAsset);
            Debug.Log("Text sprite assets organized successfully.");
        }

        private void MoveAsset(UnityEngine.Object obj, string destination)
        {
            if (obj == null) return;

            destination += obj.name + ".asset";
            string assetPath = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.MoveAsset(assetPath, destination);
        }

        private void RenameAsset(UnityEngine.Object obj, string newName)
        {
            if (obj == null || string.IsNullOrEmpty(newName)) return;

            string assetPath = AssetDatabase.GetAssetPath(obj);
            string assetFolderPath = System.IO.Path.GetDirectoryName(assetPath);
            string[] assetPathsInFolder = AssetDatabase.FindAssets("", new[] { assetFolderPath });

            // Check if another asset with the same name already exists
            foreach (string path in assetPathsInFolder)
            {
                string existingAssetPath = AssetDatabase.GUIDToAssetPath(path);
                string existingAssetName = System.IO.Path.GetFileNameWithoutExtension(existingAssetPath);

                // Skip the current asset being renamed
                if (existingAssetPath == assetPath)
                    continue;

                // Check if an asset with the same name already exists
                if (existingAssetName == newName || existingAssetName == "_old_" + newName)
                {
                    // Append "_old" to the existing asset's name
                    string oldAssetPath = System.IO.Path.Combine(assetFolderPath, "_old_" + existingAssetName + GetRandomNumber());
                    AssetDatabase.RenameAsset(existingAssetPath, System.IO.Path.GetFileName(oldAssetPath));
                }
            }

            AssetDatabase.RenameAsset(assetPath, newName);
        }

        private int GetRandomNumber()
        {
            System.Random random = new System.Random();
            return random.Next(1000, 9999);
        }

        private void CorrectPositionOfGlyphs()
        {
            CorrectPositionOfGlyphInTextSpriteAsset(manager.keyboardTextAsset);
            CorrectPositionOfGlyphInTextSpriteAsset(manager.nintendoProTextAsset);
            CorrectPositionOfGlyphInTextSpriteAsset(manager.ps3TextAsset);
            CorrectPositionOfGlyphInTextSpriteAsset(manager.ps4TextAsset);
            CorrectPositionOfGlyphInTextSpriteAsset(manager.ps5TextAsset);
            CorrectPositionOfGlyphInTextSpriteAsset(manager.xBoxTextAsset);
            Debug.Log("Glyph positions optimized successfully.");
        }

        private void CorrectPositionOfGlyphInTextSpriteAsset(TextAsset textAsset)
        {
            if (textAsset == null)
                return;

            SpriteAsset spriteAsset = textAsset as SpriteAsset;
            if (spriteAsset?.spriteGlyphTable == null)
                return;

            foreach (SpriteGlyph glyph in spriteAsset.spriteGlyphTable)
            {
                float h = glyph.metrics.height;
                GlyphMetrics metrics = glyph.metrics;
                metrics.horizontalBearingX = 0;
                metrics.horizontalBearingY = h / 4 * 3;
                glyph.metrics = metrics;
            }

            EditorUtility.SetDirty(textAsset);
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness), GUILayout.ExpandHeight(true));
            r.width = thickness;
            r.x += padding / 2;
            r.y -= 2;
            r.height += 3;
            EditorGUI.DrawRect(r, color);
        }
    }
}