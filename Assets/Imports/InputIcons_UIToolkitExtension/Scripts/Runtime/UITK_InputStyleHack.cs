using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.TextCore.Text;
using UnityEngine;


namespace InputIcons
{


    public class UITK_InputStyleHack
    {
        // and the method info for the method which must be called to reinitialise the style sheet after it's updated
        static MethodInfo m_minfoTMPStyleSheet_LoadStyleDictionaryInternal = typeof(TextStyleSheet).GetMethod("LoadStyleDictionaryInternal", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

        // grab the field info for TMP_StyleSheet's internal style list
        static FieldInfo m_finfoTextStyleSheet_m_StyleList = typeof(TextStyleSheet).GetField("m_StyleList", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

        // grab the internal strings for initialising a TMP_Style at runtime
        static FieldInfo m_finfoTextStyle_m_OpeningDefinition = typeof(TextStyle).GetField("m_OpeningDefinition", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
        static FieldInfo m_finfoTextStyle_m_ClosingDefinition = typeof(TextStyle).GetField("m_ClosingDefinition", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));


        public struct StyleStruct
        {
            public string name;
            public string openingTag;
            public string closingTag;

            public StyleStruct(string name, string openingTag, string closingTag)
            {
                this.name = name;
                this.openingTag = openingTag;
                this.closingTag = closingTag;
            }
        }

        public static void AddOrReplaceStyle(string styleName, string openingTag, string closingTag)
        {
            List<StyleStruct> styles = new List<StyleStruct>();
            styles.Add(new StyleStruct(styleName, openingTag, closingTag));

            CreateStyles(styles);
        }

        /// <summary>
        /// Creates empty entries in the default style sheet to be later filled with actual values.
        /// IMPORTANT: before filling entries with actual values, the style sheet needs to update. Do this by
        /// changing a field in the sprite sheet slightly.
        /// </summary>
        /// <param name="entries"></param>
        public static void PrepareCreateStyles(List<StyleStruct> entries)
        {
            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
            List<TextStyle> stylesList = (List<TextStyle>) m_finfoTextStyleSheet_m_StyleList.GetValue(styleSheet);

            for (int i = 0; i < entries.Count*2; i++)
            {
                stylesList.Add(null);
            }

            SelectDefaultStyleSheetInHierarchy();
        }

        public static void UpdateStyles(List<StyleStruct> entries)
        {
            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
            if (styleSheet == null)
                return;

            List<TextStyle> stylesList = (List<TextStyle>) m_finfoTextStyleSheet_m_StyleList.GetValue(styleSheet);

            for (int j = 0; j < entries.Count; j++)
            {
                int foundInstances = 0;
                for (int i = 0; i < stylesList.Count; ++i)
                {

                    if (stylesList[i].name == entries[j].name)
                    {
                        stylesList[i].name = entries[j].name;
                        m_finfoTextStyle_m_OpeningDefinition.SetValue(stylesList[i], entries[j].openingTag);
                        m_finfoTextStyle_m_ClosingDefinition.SetValue(stylesList[i], entries[j].closingTag);

                        stylesList[i].RefreshStyle();
                        foundInstances++;
                    }
                }
            }

            styleSheet.RefreshStyles();
        }

        public static int CreateStyles(List<StyleStruct> entries)
        {
            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
            List<TextStyle> stylesList = (List<TextStyle>) m_finfoTextStyleSheet_m_StyleList.GetValue(styleSheet);
            int c = 0;

            for (int j = 0; j < entries.Count; j++)
            {

                for (int i = 0; i < stylesList.Count; ++i)
                {

                    if (stylesList[i].name == "" || stylesList[i].name == entries[j].name)
                    {
                        stylesList[i].name = entries[j].name;
                        m_finfoTextStyle_m_OpeningDefinition.SetValue(stylesList[i], entries[j].openingTag);
                        m_finfoTextStyle_m_ClosingDefinition.SetValue(stylesList[i], entries[j].closingTag);

                        stylesList[i].RefreshStyle();
                        c++;
                        break;

                    }
                }
            }

            try
            {
                styleSheet?.RefreshStyles();
            }
            catch (Exception)
            {
                Debug.LogWarning("WARNING: Something went wrong. Styles not added to style sheet ...");
            }

           
            //SelectDefaultStyleSheetInHierarchy();
#if UNITY_EDITOR
            EditorUtility.SetDirty(styleSheet);
#endif
            return c;
        }

        public static void RemoveEmptyEntriesInStyleSheet()
        {
            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
            List<TextStyle> stylesList = (List<TextStyle>) m_finfoTextStyleSheet_m_StyleList.GetValue(styleSheet);
            for (int i = 0; i < stylesList.Count; ++i)
            {

                if (stylesList[i].name == "")
                {
                    stylesList.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void RemoveAllEntries()
        {
            // grab the field info for TMP_StyleSheet's internal style list
            FieldInfo m_finfoTMPStyleSheet_m_StyleList = typeof(TextStyleSheet).GetField("m_StyleList", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

            List<string> styleData = InputIcons.InputIconsManagerSO.Instance.GetAllBindingNames();

            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
            List<TextStyle> lstStyleList = (List<TextStyle>) m_finfoTMPStyleSheet_m_StyleList.GetValue(styleSheet);

            if (lstStyleList == null)
                return;

            // check for the style already in the stylesheet & remove it if it is
            for (int i = lstStyleList.Count - 1; i >= 0; i--)
            {
                if (styleData.Contains(lstStyleList[i].name))
                    lstStyleList.RemoveAt(i);

            }

            styleSheet.RefreshStyles();
#if UNITY_EDITOR
            EditorGUIUtility.PingObject(styleSheet);
            //SelectDefaultStyleSheetInHierarchy();
#endif
        }

        private static void SelectDefaultStyleSheetInHierarchy()
        {
            TextStyleSheet styleSheet = InputIconsUITKManagerSO.Instance.uitkTextStyleSheet;
#if UNITY_EDITOR
            Selection.activeObject = styleSheet;
            EditorGUIUtility.PingObject(Selection.activeObject);
#endif
        }
    }
}
