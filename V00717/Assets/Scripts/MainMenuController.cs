using UnityEngine;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    public Button loadGameButton;

    // Start is called before the first frame update
    public void Start()
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
