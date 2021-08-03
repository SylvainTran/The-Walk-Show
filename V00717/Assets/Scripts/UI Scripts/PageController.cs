using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public abstract class PageController : MonoBehaviour
{
    // Array of pages
    [SerializeField] protected Canvas[] pages;
    // The UI assets
    public UIAssets UIAssets;
    // The previous canvas
    protected Canvas previousCanvas = null;
    // The active canvas
    protected Canvas activeCanvas = null;
    // The group of buttons
    public Transform NavigationButtonCanvas;
    // The lonely confirm page button
    public Button confirmPageButton;
    // The finalize button which saves the edits and exits the menu
    public Button finalizeButton;

    // Change the page by using the page index (usually passed through inspector or explicitly elsewhere)
    public virtual void ChangePage(int pageIndex)
    {
        // Cache previously active canvas to disable it later
        if (activeCanvas != null)
        {
            previousCanvas = activeCanvas;
        }
        // O(1)
        activeCanvas = pages[pageIndex];
        // Disable cached previous canvas
        if (previousCanvas)
        {
            previousCanvas.enabled = false;
        }
        // Enable new active canvas
        if (activeCanvas)
        {
            activeCanvas.enabled = true;
        }
        // Disable nav buttons canvas temporarily
        if (NavigationButtonCanvas.GetComponent<Canvas>().enabled)
        {
            ToggleActive(NavigationButtonCanvas, false);
        }
        // Re-enable the confirm button
        if (!confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(true);
        }
        // Disable the finalize button
        if (finalizeButton.gameObject.activeInHierarchy)
        {
            finalizeButton.gameObject.SetActive(false);
        }
    }

    // Toggle canvas
    public void ToggleActive(Transform canvas, bool active)
    {
        canvas.GetComponent<Canvas>().enabled = active;
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
            ToggleActive(NavigationButtonCanvas, true);
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

    // Sets active menu canvas
    public abstract void SetActiveMenuCanvas();
}
