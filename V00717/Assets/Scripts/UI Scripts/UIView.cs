using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class UIView : MonoBehaviour
{
    // The dashboard OS
    public Canvas dashboardOS;
    public Canvas bridgeCanvas;
    public GameObject creationBridge;

    // Attach the event listeners
    public void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += ShowDashboardOS;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += CloseActiveMenu;
        StarterAssetsInputs._OnTriggerCloseOverlay += CloseActiveOverlay;
    }

    // Detach the event listeners
    public void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= ShowDashboardOS;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= CloseActiveMenu;
        StarterAssetsInputs._OnTriggerCloseOverlay -= CloseActiveOverlay;
    }

    // Close active menu
    private void CloseActiveMenu()
    {
        if (dashboardOS.enabled)
        {
            dashboardOS.enabled = false;
        }

        if (StarterAssetsInputs.activeMenuCanvas && StarterAssetsInputs.activeMenuCanvas.activeInHierarchy)
        {
            StarterAssetsInputs.activeMenuCanvas.SetActive(false);
            StarterAssetsInputs.activeMenuCanvas = null;
        }
    }

    private void CloseActiveOverlay()
    {
        if (StarterAssetsInputs.activeOverlayScreen.isActiveAndEnabled)
        {
            Debug.Log("Closing activeOverlayScreen");
            StarterAssetsInputs.activeOverlayScreen.enabled = false;
            StarterAssetsInputs.activeOverlayScreen = null;
        }
    }

    // Show dashboard OS on input trigger
    private void ShowDashboardOS()
    {
        dashboardOS.gameObject.SetActive(true);
        dashboardOS.enabled = true;
        bridgeCanvas.enabled = true;
        creationBridge.SetActive(true);
        Debug.Log("Showing dashboard OS canvas");
    }

}
