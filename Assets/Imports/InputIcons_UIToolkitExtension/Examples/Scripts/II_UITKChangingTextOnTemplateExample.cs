using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace InputIcons
{
    public class II_UITKChangingTextOnTemplateExample : MonoBehaviour
    {

        VisualElement root;
        VisualTreeAsset templateAsset;

        VisualElement displayContainer;
        bool templateShown = false;

        public string assetName = "II_ExampleTemplate";

        private UIDocument uiDoc;
        private Label textToChange;

        public string labelID = "ii_textChanging";

        float timer = 0.5f;

        private int displayNumber = 0;

        public string textOne = "Changing text: <color=yellow> <style=\"Platformer Controls/Move\"> to move</color> <style=\"Platformer Controls/Move/Down> is down Press <style=\"Platformer Controls/Jump> to jump";
        public string textTwo = "Changing text: Use <style=\"Platformer Controls/Jump> to jump";

        private async void Awake()
        {
            templateAsset = await GetTemplateAsset();
            displayContainer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("rootObject");
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (templateAsset == null)
                return;

            if (!templateShown)
            {
                ShowTemplate();
                templateShown = true;
            }

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0.5f;
                ChangeText();
            }
        }

        async Task<VisualTreeAsset> GetTemplateAsset()
        {
            VisualTreeAsset template = Resources.Load<VisualTreeAsset>(assetName);
            while (template == null)
                await Task.Yield();

            return template;
        }

        void ShowTemplate()
        {
            root = templateAsset.CloneTree().Q<VisualElement>("myRoot");
            displayContainer.Add(root);
        }

        private void ChangeText()
        {
            if (root == null) return;

            //necessary to get/refresh component here as it might have changed
            //uiDoc = GetComponent<UIDocument>();
            textToChange = root.Q<Label>(labelID);

            if (displayNumber == 0)
            {
                displayNumber = 1;

                textToChange.text = textOne;
            }
            else if (displayNumber == 1)
            {
                displayNumber = 0;

                textToChange.text = textTwo;
            }

        }
    }
}
