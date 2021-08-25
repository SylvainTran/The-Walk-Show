using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public Button loadGameButton;

    private void Start()
    {
        if (SaveSystem.SaveFileExists("PlayerStatistics.json") || SaveSystem.SaveFileExists("colonists.json") || SaveSystem.SaveFileExists("deadColonists.json"))
        {
            loadGameButton.GetComponent<Button>().interactable = true;
        } else
        {
            loadGameButton.GetComponent<Button>().interactable = false;
        }
    }
    public void NewGame()
    {
        // Todo create new save slot
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(3);
    }

    public void PatronsGallery()
    {

    }
}
