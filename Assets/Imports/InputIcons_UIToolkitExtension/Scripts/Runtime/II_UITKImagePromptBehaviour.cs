using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static InputIcons.II_SpritePrompt;

namespace InputIcons
{
    public class II_UITKImagePromptBehaviour : MonoBehaviour
    {
        private UIDocument rootDocument => GetComponent<UIDocument>();
        public List<UITKImagePromptData> spritePromptDatas = new List<UITKImagePromptData>();

        private void OnEnable()
        {
            InitializeUIDocumentData();
            UpdateDisplayedImages();
            InputIconsManagerSO.onBindingsChanged += UpdateDisplayedImages;
            InputIconsManagerSO.onControlsChanged += UpdateDisplayedImages;
        }

        private void OnDisable()
        {
            InputIconsManagerSO.onBindingsChanged -= UpdateDisplayedImages;
            InputIconsManagerSO.onControlsChanged -= UpdateDisplayedImages;
        }

        private void InitializeUIDocumentData()
        {
            for (int i = 0; i < spritePromptDatas.Count; i++)
            {
                if (rootDocument == null || rootDocument.rootVisualElement == null) continue;

                // If no root element specified, search from document root
                if (string.IsNullOrEmpty(spritePromptDatas[i].rootElementID))
                {
                    spritePromptDatas[i].image = rootDocument.rootVisualElement.Q(spritePromptDatas[i].imageID);
                }
                // If root element specified, first find that element, then search for imageID within it
                else
                {
                    VisualElement rootElement = rootDocument.rootVisualElement.Q(spritePromptDatas[i].rootElementID);
                    if (rootElement != null)
                    {
                        spritePromptDatas[i].image = rootElement.Q(spritePromptDatas[i].imageID);
                    }
                }
            }
        }

        public void UpdateDisplayedImages()
        {
            foreach (UITKImagePromptData s in spritePromptDatas)
            {
                s.UpdateDisplayedSprite();
            }
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        private void UpdateDisplayedImages(InputDevice inputDevice)
        {
            UpdateDisplayedImages();
        }

       

#if UNITY_EDITOR
        public void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;
            InitializeUIDocumentData();
            UpdateDisplayedImages();
        }
#endif

        [System.Serializable]
        public class UITKImagePromptData : II_PromptData
        {
            
            public VisualElement image;
            public string imageID;
            public string rootElementID;

            public UITKImagePromptData(): base()
            {

            }

            public UITKImagePromptData(UITKImagePromptData data) : base(data)
            {
                if (data == null)
                    return;

                image = data.image;
                imageID = data.imageID;
                rootElementID = data.rootElementID;
            }

            public void UpdateDisplayedSprite()
            {
                if (actionReference == null)
                {
                    if (image != null)
                        image.style.backgroundImage = null;
                    return;
                }


                if (image != null)
                {
                    image.style.backgroundImage = new StyleBackground(GetKeySprite());
                }
                 
            }

            public static List<UITKImagePromptData> CloneList(List<UITKImagePromptData> originalList)
            {
                if (originalList == null)
                    return null;

                List<UITKImagePromptData> clonedList = new List<UITKImagePromptData>();

                foreach (var item in originalList)
                {
                    UITKImagePromptData clonedItem = new UITKImagePromptData(item);
                    clonedList.Add(clonedItem);
                }

                return clonedList;
            }

        }
    }
}