using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int mainMenuSceneIndex = 0;

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Scene index invalido: {sceneIndex}. Revisa Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadCurrentScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No hay una siguiente escena en Build Settings.");
            return;
        }

        SceneManager.LoadScene(nextIndex);
    }

    public void LoadPreviousScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int previousIndex = currentIndex - 1;

        if (previousIndex < 0)
        {
            Debug.LogWarning("No hay una escena anterior.");
            return;
        }

        SceneManager.LoadScene(previousIndex);
    }

    public void LoadMainMenu()
    {
        LoadSceneByIndex(mainMenuSceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
