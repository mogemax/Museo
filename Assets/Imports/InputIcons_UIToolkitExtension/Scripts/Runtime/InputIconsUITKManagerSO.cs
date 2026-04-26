using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static InputIcons.InputIconsManagerSO;
using static InputIcons.InputIconsUtility;
using TextAsset = UnityEngine.TextCore.Text.TextAsset;

namespace InputIcons
{

    [CreateAssetMenu(fileName = "InputIconsUITKManager", menuName = "Input Icon Set/Input Icons UI Toolkit Manager", order = 505)]
    public class InputIconsUITKManagerSO : ScriptableObject
    {
        private static readonly List<II_UIDocument> uiDocuments = new List<II_UIDocument>();

        public readonly string defaultSpriteAssetPath = "InputIcons/Sprite Assets/";

        public ThemeStyleSheet uitkThemeStyleSheet;
        public PanelSettings uitkPanelSettings;
        public TextSettings uitkTextSettings;
        public TextStyleSheet uitkTextStyleSheet;

        public TextAsset keyboardTextAsset;
        public TextAsset nintendoProTextAsset;
        public TextAsset ps3TextAsset;
        public TextAsset ps4TextAsset;
        public TextAsset ps5TextAsset;
        public TextAsset xBoxTextAsset;


        public TextUpdateOptions uiDocumentUpdateOptions = TextUpdateOptions.SearchAndUpdate;


        private static InputIconsUITKManagerSO instance;
        public static InputIconsUITKManagerSO Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else
                {
                    InputIconsUITKManagerSO iconManagerUITK = Resources.Load("InputIcons/InputIconsUITKManager") as InputIconsUITKManagerSO;
                    if (iconManagerUITK)
                    {
                        instance = iconManagerUITK;
                    }


                    return instance;
                }
            }
            set => instance = value;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RunOnStart()
        {
            if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled)
            {
                Instance.Initialize();
            }
        }
#endif

        private void Awake()
        {
            Instance = this;
            Initialize();
        }

        private void OnEnable()
        {
            Instance = this;
            Initialize();
        }

        //Use Initialize to start listening for update events on the InputIconsManager to update UI Toolkit texts when necessary
        public void Initialize()
        {
            InputIconsManagerSO.onStyleSheetsUpdated -= HandleStyleSheetUpdated; //unsubscribe first since this method could be called several times
            InputIconsManagerSO.onStyleSheetsUpdated += HandleStyleSheetUpdated;

            InputIconsManagerSO.onStylesPrepared -= HandleStylesPrepared;
            InputIconsManagerSO.onStylesPrepared += HandleStylesPrepared;
            InputIconsManagerSO.onStylesAddedToStyleSheet -= HandleStylesAdded;
            InputIconsManagerSO.onStylesAddedToStyleSheet += HandleStylesAdded;
            //InputIconsLogger.Log("UI Toolkit Manager listening for style changes");
        }

        public static void HandleStylesPrepared()
        {
            UITK_InputStyleHack.PrepareCreateStyles(GetStyleUpdatesDeviceSpecific(false));
            UITK_InputStyleHack.PrepareCreateStyles(GetStyleUpdatesDeviceSpecific(true));
        }

        public static void HandleStylesAdded()
        {
            UITK_InputStyleHack.CreateStyles(GetStyleUpdatesDeviceSpecific(false));
            UITK_InputStyleHack.CreateStyles(GetStyleUpdatesDeviceSpecific(true));
            UITK_InputStyleHack.RemoveEmptyEntriesInStyleSheet();
        }

        private void OnDestroy()
        {
            InputIconsManagerSO.onStyleSheetsUpdated -= HandleStyleSheetUpdated;
            InputIconsManagerSO.onStylesPrepared -= HandleStylesPrepared;
            InputIconsManagerSO.onStylesAddedToStyleSheet -= HandleStylesAdded;
        }

        public static void RegisterUIDocument(II_UIDocument uiDocument)
        {
            uiDocuments.Add(uiDocument);
        }

        public static void UnregisterUIDocument(II_UIDocument uiDocument)
        {
            uiDocuments.Remove(uiDocument);
        }


        private void HandleStyleSheetUpdated()
        {
            UpdateTextStyleSheet();
        }

        public void UpdateTextStyleSheet()
        {
            if (InputIconSetConfiguratorSO.GetCurrentIconSet() is InputIconSetKeyboardSO)
                OverrideStylesInStyleSheetDeviceSpecific(false);
            else
                OverrideStylesInStyleSheetDeviceSpecific(true);

            RefreshUIElements();
            RefreshUITKImagePrompts();
        }

        public void RefreshUIElements()
        {
            if (uiDocumentUpdateOptions == TextUpdateOptions.SearchAndUpdate)
            {
                //go through all Text objects in the scene and set them dirty
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (!scene.isLoaded)
                    return;

                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    UIDocument[] tmpObjects = obj.GetComponentsInChildren<UIDocument>();
                    foreach (UIDocument tObj in tmpObjects)
                    {
                        RefreshUIDocument(tObj);
                    }
                }
            }
            else if (uiDocumentUpdateOptions == TextUpdateOptions.ViaInputIconsTextComponents)
            {
                RefreshInputIconsUIDocuments();
            }
        }

        public static void RefreshInputIconsUIDocuments()
        {
            foreach (II_UIDocument doc in uiDocuments)
            {
                doc.RefreshTexts();
            }
        }

        //This is used to refresh the labels in UI Documents to display the new sprites
        //It might be an ugly hack, if you know a better way to update the labels, please let me know
        public static void RefreshUIDocument(UIDocument uiDoc)
        {

            if (uiDoc == null)
                return;
            
           
            PanelSettings settings = uiDoc.panelSettings;
            uiDoc.panelSettings = null;
            uiDoc.panelSettings = settings;
            

            /*
            if (uiDoc.rootVisualElement != null)
            {
                if (uiDoc.rootVisualElement.style.display != DisplayStyle.None)
                {
                    uiDoc.rootVisualElement.style.display = DisplayStyle.None;
                    uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
                }
            }
            */
        }

        public static void RefreshUITKImagePrompts()
        {
            II_UITKImagePromptBehaviour[] imagePrompts = FindObjectsOfType<II_UITKImagePromptBehaviour>();
            foreach (var imagePrompt in imagePrompts)
            {
                if (imagePrompt == null) continue;

                imagePrompt.UpdateDisplayedImages();
            }
        }


        //Overrides the values in the default style sheet with the ones currently in the style lists of the manager
        public static void OverrideStylesInStyleSheetDeviceSpecific(bool gamepadStyles)
        {
            List<UITK_InputStyleHack.StyleStruct> styleUpdates = GetStyleUpdatesDeviceSpecific(gamepadStyles);

            UITK_InputStyleHack.UpdateStyles(styleUpdates);
        }

        public static List<UITK_InputStyleHack.StyleStruct> GetStyleUpdatesDeviceSpecific(bool gamepadStyles)
        {

            List<UITK_InputStyleHack.StyleStruct> styleUpdates = new List<UITK_InputStyleHack.StyleStruct>();

            List<InputStyleData> inputStyles;

            string inputIconsOpeningTag = InputIconsManagerSO.Instance.openingTag;
            string inputIconsClosingTag = InputIconsManagerSO.Instance.closingTag;

            if (!gamepadStyles)
                inputStyles = InputIconsManagerSO.Instance.inputStyleKeyboardDataList;
            else
                inputStyles = InputIconsManagerSO.Instance.inputStyleGamepadDataList;

            for (int i = 0; i < inputStyles.Count; i++)
            {
                if (inputStyles[i] == null)
                    continue;

                string style = InputIconsManagerSO.Instance.GetCustomStyleTag(inputStyles[i]);
                style = inputIconsOpeningTag + style + inputIconsClosingTag;
                styleUpdates.Add(new UITK_InputStyleHack.StyleStruct(inputStyles[i].bindingName, style, ""));


                //font custom glyphs not yet supported in UI Toolkit
                /*
                if (!InputIconsManagerSO.Instance.isUsingFonts) //if not using fonts, can skip this
                    continue;

                //handle font codes as well
                InputIconSetBasicSO iconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                if (gamepadStyles)
                    iconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();

                if (iconSet.fontAsset != null)
                    style = "<font=\"" + iconSet.fontAsset.name + "\"";
                else
                {
                    style = "<font=\"InputIcons_Keyboard_Font SDF\"";
                    if (gamepadStyles)
                        style = "<font=\"InputIcons_Gamepad_Font SDF\"";
                }

                style += ">";
                if (InputIconsManagerSO.Instance.showAllInputOptionsInStyles)
                    style += inputStyles[i].fontCode;
                else
                    style += inputStyles[i].fontCode_singleInput;

                string closingTag = "</font>";
                styleUpdates.Add(new UITK_InputStyleHack.StyleStruct("Font/" + inputStyles[i].bindingName, style, closingTag));
                */
            }



            return styleUpdates;
        }

        //Removes all InputIcons related entries from the default style sheet. Can only remove entries which are still present
        //in the list of Input Action Assets assigned to the manager
        public static void RemoveAllStyleSheetEntries()
        {
            UITK_InputStyleHack.RemoveAllEntries();
        }
    }
}
