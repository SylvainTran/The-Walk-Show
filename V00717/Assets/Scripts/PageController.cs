using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    // The colonist identification page
    public Transform identification;
    // The biological data page
    public Transform biological;
    // The psychological profile data page
    public Transform psychological;
    // The socioeconomic data page
    public Transform socioeconomic;

    // The previous canvas
    private Canvas previousCanvas = null;
    // The active canvas
    private Canvas activeCanvas = null;
    // The group of buttons
    public Transform NavigationButtonCanvas;
    // The lonely confirm page button
    public Button confirmPageButton;
    // The finalize button which saves the edits and exits the menu
    public Button finalizeButton;

    // Save to file event
    public delegate Task SaveToFile();
    public static event SaveToFile _OnSaveToFile; // Listened to by BabyController.cs

    // Changes the active page
    public void ChangePage(string pageName)
    {
        // Cache previously active canvas to disable it later
        if(activeCanvas != null)
        {
            previousCanvas = activeCanvas;
        }
        // Match the page and set it active
        switch(pageName)
        {
            case "identification":
                activeCanvas = identification.gameObject.GetComponent<Canvas>();
                break;
            case "biological":
                activeCanvas = biological.gameObject.GetComponent<Canvas>();
                break;
            case "psychological":
                activeCanvas = psychological.gameObject.GetComponent<Canvas>();
                break;
            case "socioeconomic":
                activeCanvas = socioeconomic.gameObject.GetComponent <Canvas>();
                break;
            default:
                break;
        }
        // Disable cached previous canvas
        if(previousCanvas)
        {
            previousCanvas.enabled = false;
        }
        // Enable new active canvas
        if(activeCanvas)
            activeCanvas.enabled = true;
        // Disable nav buttons canvas temporarily
        if(NavigationButtonCanvas.GetComponent<Canvas>().enabled)
        {
            NavigationButtonCanvas.GetComponent<Canvas>().enabled = false;
        }
        // Re-enable the confirm button
        if(!confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(true);
        }
        // Disable the finalize button
        if (finalizeButton.gameObject.activeInHierarchy)
        {
            finalizeButton.gameObject.SetActive(false);
        }
    }
    // Confirm button event
    public void ConfirmPage()
    {
        // Disable currently active canvas
        if(activeCanvas && activeCanvas.enabled)
        {
            activeCanvas.enabled = false;
        }
        // Re-enable nav canvas
        if (!NavigationButtonCanvas.GetComponent<Canvas>().enabled)
        {
            NavigationButtonCanvas.GetComponent<Canvas>().enabled = true;
        }
        // Hide confirm page button
        if(confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(false);
        }
        // Show finalize page button
        if (!finalizeButton.gameObject.activeInHierarchy)
        {
            finalizeButton.gameObject.SetActive(true);
        }
    }
    // Save changes to file (/colonists.txt) and exit colonist creation menu
    public void FinalizeCreation()
    {
        // Event to save the current baby template to a file
        _OnSaveToFile();
        // Increment the static unique colonist personnel ID
        BabyModel.UniqueColonistPersonnelID++;
        // TODO exit menu
    }
}
