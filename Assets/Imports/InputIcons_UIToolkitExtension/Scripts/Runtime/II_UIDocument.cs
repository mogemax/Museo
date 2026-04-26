using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace InputIcons
{
    /// <summary>
    /// Attach to a UI Document to update the document when changes to the bindings happen
    /// or when the player switches devices
    /// </summary>
    public class II_UIDocument : MonoBehaviour
    {
        private UIDocument myUIDocument => GetComponent<UIDocument>();
        public List<string> textIDsToUpdate = new List<string>();

        public UIDocument GetDocument()
        {
            return myUIDocument;
        }

        private void OnEnable()
        {
            //Register with the manager to update this text object when the user changes devices
            InputIconsUITKManagerSO.RegisterUIDocument(this);
        }

        private void OnDisable()
        {
            //Unregister when not needed
            InputIconsUITKManagerSO.UnregisterUIDocument(this);
        }

        public void RefreshTexts()
        {
            //a little ugly hack to refresh the labels by adding a space to the end of the text and removing it shortly after
            if (myUIDocument.rootVisualElement != null)
            {
                for (int i = 0; i < textIDsToUpdate.Count; i++)
                {
                    Label el = myUIDocument.rootVisualElement.Q<Label>(textIDsToUpdate[i]);

                    string temp = el.text;
                    el.text = temp+" ";
                    myUIDocument.rootVisualElement.schedule.Execute(_ => { el.text.Trim(); }).ExecuteLater(10);
                }
            }
        }

    
    }
}

