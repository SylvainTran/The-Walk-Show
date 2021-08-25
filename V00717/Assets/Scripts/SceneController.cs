using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    int currentScene;
    int max;

    public void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        max = SceneManager.sceneCountInBuildSettings;
    }

    public void NewGame()
    {
        LoadNextScene(currentScene + 1);
    }

    /// <summary>
    /// The start of game is at buildIndex 3
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene(3);
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
        SceneManager.LoadScene(buildIndex);
    }
    /// <summary>
    /// Default method that checks for 
    /// the count of scenes.
    /// </summary>
    public void LoadNextScene()
    {
        if(currentScene < max)
        {
            LoadNextScene(currentScene + 1);
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
