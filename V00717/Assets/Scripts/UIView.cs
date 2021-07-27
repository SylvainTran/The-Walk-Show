using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class UIView : MonoBehaviour
{
    // The dashboard OS
    public Canvas dashboardOS;

    // Attach the event listeners
    public void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += ShowDashboardOS;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += CloseActiveMenu;
    }

    // Detach the event listeners
    public void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= ShowDashboardOS;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= CloseActiveMenu;
    }

    // Close active menu
    private void CloseActiveMenu()
    {
        if (StarterAssetsInputs.activeMenuCanvas.isActiveAndEnabled)
        {
            Debug.Log("Closing activeMenuCanvas");
            StarterAssetsInputs.activeMenuCanvas.enabled = false;
            Debug.Log(StarterAssetsInputs.activeMenuCanvas.enabled);
        }
    }

    // Show dashboard OS on input trigger
    private void ShowDashboardOS()
    {
        dashboardOS.enabled = true;
        Debug.Log("Showing dashboard OS canvas");
    }

}
