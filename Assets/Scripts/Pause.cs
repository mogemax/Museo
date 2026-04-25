using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            Time.timeScale = isPaused ? 0f : 1f;
            AudioListener.pause = isPaused;
        }
    }

}
