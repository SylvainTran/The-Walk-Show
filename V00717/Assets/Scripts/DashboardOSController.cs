using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DashboardOSController : PageController
{
    protected int activePageIndex;
    // Dashboard OS canvas
    public Canvas dashboardOS;
    // Dashboard nav
    public Canvas dashboardNav;
    // The login page
    public Canvas loginPage;    

    // Whether the player has logged in yet (just a fake)
    // private bool isLoggedIn = false;

    public delegate void RequestColonistData();
    public event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        //BabyController._OnRequestColonistDataReply +=  
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
    }

    // Sets the current active menu canvas to the dashboard OS which deals with inputs by state
    public override void SetActiveMenuCanvas()
    {
        StarterAssetsInputs.SetActiveMenuCanvas(dashboardOS);
    }

    // This fake login just prevents the player from seeing the login page again
    public void Login()
    {
        //isLoggedIn = true;
        ChangePage((int)Enums.DashboardPageIndexes.DESKTOP);
        // Hide login page forever (this session)
        loginPage.enabled = false;
        dashboardNav.enabled = true;
    }

    // TODO fix active page index changed in other menus too
    public override void ChangePage(int pageIndex)
    {
        // Cache the active page index
        activePageIndex = pageIndex;
        base.ChangePage(pageIndex);
        // Deal with special pages
        if(pageIndex == (int)Enums.DashboardPageIndexes.DATABASE)
        {
            ReadSaveList();
        }
    }

    // Read from the list of saved colonists
    public void ReadSaveList()
    {
        _OnRequestColonistData();
    }

    // Reply from server (baby controller)
    public void OnServerReply()
    {
        
    }
}
