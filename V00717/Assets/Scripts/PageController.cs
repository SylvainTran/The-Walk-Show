using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    // The biological data page
    public Transform biologicalData;
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
            case "BiologicalData":
                activeCanvas = biologicalData.gameObject.GetComponent<Canvas>();
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
        if(activeCanvas.enabled)
        {
            activeCanvas.enabled = false;
        }
        // Re-enable nav canvas
        if (!NavigationButtonCanvas.GetComponent<Canvas>().enabled)
        {
            NavigationButtonCanvas.GetComponent<Canvas>().enabled = true;
        }
        // Re-enable confirm page button
        if(confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(false);
        }
    }
}
