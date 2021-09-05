using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuController : SceneController
{
    public Button loadGameButton;

    // Start is called before the first frame update
    new void Start()
    {
        if (SaveSystem.SaveFileExists("PlayerStatistics.json") || SaveSystem.SaveFileExists("colonists.json") || SaveSystem.SaveFileExists("deadColonists.json"))
        {
            loadGameButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            loadGameButton.GetComponent<Button>().interactable = false;
        }
    }
}
