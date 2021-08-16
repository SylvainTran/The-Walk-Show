using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void NewGame()
    {
        // Todo create new save slot
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PatronsGallery()
    {
        SceneManager.LoadScene(2);
    }
}
