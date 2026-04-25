using UnityEngine;

public class Healt : MonoBehaviour
{
    public int life = 100;
    public Vector3 labelOffset = new Vector3(0f, 1.5f, 0f);

    private void OnGUI()
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + labelOffset);
        if (screenPosition.z <= 0f)
        {
            return;
        }

        float x = screenPosition.x - 35f;
        float y = Screen.height - screenPosition.y;
        GUI.Label(new Rect(x, y, 90f, 20f), "Life: " + life);
    }
}
