using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Ports
using static Enums;
using System;

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

    // The vertical layout for event logs
    public RectTransform eventLogVerticalLayout;
    // Number of event logs instantiated yet
    public int eventCount = default;
    // The max amount of event logs instantiable
    private const int MAX_EVENT_COUNT = 100;

    public TMP_Text eventLogText = null;

    public float eventLogYOffset = 0.0f; // Starts at 0 (anchored at top of parent container), then decreases by the decrement to go down
    public float eventLogYOffsetDecrement = 5.0f; // Default value
    public float eventLogTextTopMargin = 10.0f; // In case we need more space

    // Queue of generated event logs (we dequeue first ones when max reached)
    private Queue<string> eventLogQueue;

    public delegate void RequestColonistData(DataRequests requestPort);
    public static event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ReadSaveList;
        BabyModel._OnGameClockEventProcessed += UpdateEventLog;
        //GameClockEvent._OnColonistIsDead += UpdateEventLog;
        BattleEvent._OnBattleEnded += UpdateEventLog;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ReadSaveList;
        BabyModel._OnGameClockEventProcessed -= UpdateEventLog;
        //GameClockEvent._OnColonistIsDead -= UpdateEventLog;
        BattleEvent._OnBattleEnded -= UpdateEventLog;
    }

    private void Start()
    {
        eventLogQueue = new Queue<string>(MAX_EVENT_COUNT);
    }

    private void UpdateEventLog(GameClockEvent e)
    {
        if(eventCount < MAX_EVENT_COUNT)
        {
            AppendEventLogTMPProText(e);
            ++eventCount;
        } else
        {
            // Clear the first log TODO after archiving it elsewhere
            Debug.Log("Clearing old log");
            int eraseCount = 10;
            for(int i = 0; i < eraseCount; i++)
            {
                eventLogQueue.Dequeue();
                --eventCount;
            }
            eventLogText.SetText("");
            string rebuiltLog = "";
            foreach(string message in eventLogQueue)
            {
                rebuiltLog += message;
            }
            eventLogText.SetText(rebuiltLog);
        }
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

    // TODO refactor out a writer component to re-use elsewhere
    public void AppendEventLogTMPProText(GameClockEvent e)
    {
        if(eventLogText == null)
        {
            CreateEventLogTMProText(e);
        } else
        {
            TextMeshProUGUI existingText = eventLogText.GetComponent<TextMeshProUGUI>();
            string appendText = existingText.text;
            appendText += "\n\n" + e.Message;
            existingText.SetText(appendText);
        }
        eventLogQueue.Enqueue(e.Message);
    }

    public void CreateEventLogTMProText(GameClockEvent e)
    {
        eventLogText = Instantiate(UIAssets.eventLogText.GetComponent<TextMeshProUGUI>());
        eventLogText.SetText(e.Message);
        eventLogText.rectTransform.SetParent(eventLogVerticalLayout.transform, false); // False lets us use the prefab's coordinates
        eventLogText.rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
        eventLogText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        eventLogText.rectTransform.pivot = new Vector2(0.5f, 1.0f);
        eventLogText.fontSize = 12.0f;
        //eventLogText.rectTransform.GetComponent<TextMeshProUGUI>().margin = new Vector4(0.0f, eventLogTextTopMargin);

        // Change the line spacing depending on the height of the generated text, which can be variable
        // The proportion of the spacing scales with the height of the generated text
        // Adjust where the next text appears using the text's size (height gotten from bounds)
        //eventLogText.ForceMeshUpdate();
        //float textHeight = Math.Abs(eventLogText.textBounds.size.y);
        //float ratio = textHeight;// / 5.0f;
        //eventLogYOffsetDecrement = textHeight;// * (ratio);
        //Debug.Log($"Offset: {eventLogYOffset}, Decr: {eventLogYOffsetDecrement}");
        //eventLogYOffset += eventLogYOffsetDecrement;
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
