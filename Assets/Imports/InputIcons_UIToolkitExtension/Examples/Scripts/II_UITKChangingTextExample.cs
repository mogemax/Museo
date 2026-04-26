using UnityEngine;
using UnityEngine.UIElements;

namespace InputIcons
{
    public class II_UITKChangingTextExample : MonoBehaviour
    {

        float timer = 0.5f;

        private UIDocument uiDoc;

        private Label textToChange;

        public string labelID = "ii_textChanging";

        private int displayNumber = 0;

        public string textOne = "Changing text: <color=yellow> <style=\"Platformer Controls/Move\"> to move</color> <style=\"Platformer Controls/Move/Down> is down Press <style=\"Platformer Controls/Jump> to jump";
        public string textTwo = "Changing text: Use <style=\"Platformer Controls/Jump> to jump";

        // Start is called before the first frame update
        void Start()
        {

        }


        void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0.5f;
                ChangeText();
            }
        }

        private void ChangeText()
        {
            //necessary to get/refresh component here as it might have changed
            uiDoc = GetComponent<UIDocument>();
            textToChange = uiDoc.rootVisualElement.Q<Label>(labelID);

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
