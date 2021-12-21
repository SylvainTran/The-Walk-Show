using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    public int currentScene;
    public int max;

    public SceneController()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        max = SceneManager.sceneCountInBuildSettings;
        Debug.LogWarning($"MAX n scenes: {max}");

    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
        currentScene = 1;
    }

    public void UpdateCurrentScene(int value)
    {
        currentScene = value;
    }

    /// <summary>
    /// The start of game is at buildIndex 2
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene(2);
        currentScene = 2;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void PatronsGallery()
    {

    }
    /// <summary>
    /// Overload with buildIndex
    /// </summary>
    /// <param name="buildIndex"></param>
    public void LoadNextScene(int buildIndex)
    {
        Debug.LogWarning($"Load scene at: Build Index {buildIndex}");
        SceneManager.LoadScene(buildIndex);
    }
    /// <summary>
    /// Default method that checks for 
    /// the count of scenes.
    /// </summary>
    public void LoadNextScene()
    {
        Debug.LogWarning($"Loading next scene! Current Scene Index: {currentScene} Max: {max}");
        if(currentScene < max)
        {
            Debug.LogWarning($"Loading scene at index {currentScene + 1}");
            SceneManager.LoadScene(++currentScene);
        }
    }

    public void Update()
    {
        if(currentScene > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            LoadNextScene();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
