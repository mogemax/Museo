#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InputIcons
{
    public static class InputIconsUITKSpritePacker
    {

        private static bool CreateUITKSpriteAsset(Object obj)
        {
            Selection.activeObject = obj;
            
#if UNITY_6000_0_OR_NEWER
            if (!EditorApplication.ExecuteMenuItem("Assets/Create/Text Core/Sprite Asset"))
#endif
                if (!EditorApplication.ExecuteMenuItem("Assets/Create/Text/Sprite Asset"))
                {
                    Debug.LogError("Could not create Sprite Text Asset .. Maybe the execute menu item " +
                        "path got changes. Was previously in Assets/Create/Text/Sprite Asset or in " +
                        "Assets/Create/Text Core/Sprite Asset");

                    return false;
                }

            return true;
        }

        //[MenuItem("Tools/Input Icons Create UITK Sprite Assets", priority = 0)]
        public static void CreateSpriteAssetsOfIconSets()
        {
           
            List<InputIconSetBasicSO> iconSOs = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();

            Object lastEntry = null;
            Debug.Log("Searching Input Icons sprite atlases in the " + InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH + " folder");
            int c = 0;
            List<string> createdAssetPaths = new List<string>();
            for (int i = 0; i < iconSOs.Count; i++)
            {
                InputIconSetBasicSO iconSet = iconSOs[i];
                if (iconSet == null)
                    continue;

                string atlasFileName = InputIconsManagerSO.Instance.TEXTMESHPRO_SPRITEASSET_FOLDERPATH +iconSet.iconSetName+  ".png";
                Object finalTexture = AssetDatabase.LoadAssetAtPath(atlasFileName, typeof(Texture2D));
                if(finalTexture!=null)
                {
                    string sourcePath = AssetDatabase.GetAssetPath(finalTexture);
                    if (createdAssetPaths.Contains(sourcePath))
                        continue;

                    createdAssetPaths.Add(sourcePath);

                    Selection.activeObject = finalTexture;
                    lastEntry = finalTexture;
                    
                    if (CreateUITKSpriteAsset(Selection.activeObject))
                        c++;
                }
            }
            Debug.Log(c + " textures found. "+c+" Text Sprite Assets created.\n" +
                "Now move them into the Sprite Assets folder in: InputIcons_UIToolkitExtension/Resources/InputIcons/Sprite Assets/ and rename them to match the name of the respective Sprite Atlases");

            EditorGUIUtility.PingObject(lastEntry);
        }
    }
}
#endif