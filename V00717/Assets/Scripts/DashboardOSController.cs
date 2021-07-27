using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ports
using static Enums;

public class DashboardOSController : PageController
{
    protected int activePageIndex;
    // Dashboard OS canvas
    public Canvas dashboardOS;
    // Dashboard nav
    public Canvas dashboardNav;
    // The login page
    public Canvas loginPage;

    // Vertical group
    public VerticalLayoutGroup verticalGroupLayout;
    // Size parameters (TODO put in a config file or PlayerPrefs)
    public Vector3 colonistIconSize;
    
    // Whether the player has logged in yet (just a fake)
    // private bool isLoggedIn = false;

    public delegate void RequestColonistData(DataRequests requestPort);
    public static event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ReadSaveList;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ReadSaveList;
    }

    // Sets the current active menu canvas to the dashboard OS which deals with inputs by state
    public override void SetActiveMenuCanvas()
    {
        StarterAssetsInputs.SetActiveMenuCanvas(dashboardOS);
    }

    public void Init()
    {
        verticalGroupLayout.transform.hasChanged = false;
    }

    // This fake login just prevents the player from seeing the login page again
    public void Login()
    {
        //isLoggedIn = true;
        ChangePage((int)DashboardPageIndexes.DESKTOP);
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
        if(pageIndex == (int)DashboardPageIndexes.DATABASE)
        {
            ReadSaveList();
        }
    }

    // Request from the colonist server the active list of colonists alive - Note: this method is also a subscriber of the SuccessfulSaveAction event publisher.
    public void ReadSaveList()
    {
        _OnRequestColonistData(DataRequests.LIVE_COLONISTS);
    }

    // Reply from server (baby controller)
    public void OnServerReply(BabyModel[] colonists)
    {
        // Clear icons; TODO only if the children of the vertical g. layout has changed since
        ClearColonistIcons();
        CreateColonistIcons(colonists);
    }

    public void ClearColonistIcons()
    {
        int len = verticalGroupLayout.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            Destroy(verticalGroupLayout.transform.GetChild(i).gameObject);
        }
    }

    public void CreateColonistIcons(BabyModel[] colonists)
    {
        // Replace content
        foreach (BabyModel b in colonists)
        {
            if (b != null)
            {
                // Icons with click events
                Image colonistIcon = Instantiate(UIAssets.colonistIcon.GetComponent<Image>());
                verticalGroupLayout.childControlWidth = true;
                colonistIcon.transform.localScale = colonistIconSize;
                colonistIcon.rectTransform.SetParent(verticalGroupLayout.transform);
                // TODO adjust rectTransform size - some text gets wrapped due to local scale changing to its new parent vertical layout

                // Names
                TMP_Text colonistName = Instantiate(UIAssets.colonistName.GetComponent<TextMeshProUGUI>());
                colonistName.SetText(b.Name);
                colonistName.rectTransform.SetParent(verticalGroupLayout.transform);
                colonistName.fontSize = 42.0f;
            }
        }
        verticalGroupLayout.transform.hasChanged = true;
    }
}
