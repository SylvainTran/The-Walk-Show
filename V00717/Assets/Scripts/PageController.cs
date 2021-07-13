using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
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
    }
}
