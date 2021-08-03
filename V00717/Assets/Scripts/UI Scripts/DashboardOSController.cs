using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Ports
using static Enums;
using System;
using UnityEngine.EventSystems;

public class DashboardOSController : PageController
{
    protected int activePageIndex;
    // Dashboard OS canvas
    public Canvas dashboardOS;
    // Dashboard nav
    public Canvas dashboardNav;
    // The login page
    public Canvas loginPage;

    // Vertical group for live colonists
    public VerticalLayoutGroup aliveColonistsVerticalGroupLayout;
    // Vertical group for dead colonists
    public VerticalLayoutGroup deadColonistsVerticalGroupLayout;
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


    // Obituary overlay to fade in/out when clicking on dead colonist icon
    public Canvas obituaryOverlayCanvas;

    public delegate void RequestColonistData(DataRequests requestPort);
    public static event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ReadSaveList;
        BabyModel._OnGameClockEventProcessed += UpdateEventLog;
        GameClockEvent._OnColonistIsDead += OnColonistDied;
        BattleEvent._OnBattleEnded += UpdateEventLog;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
        BabyController._OnRequestColonistDataResponse -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ReadSaveList;
        BabyModel._OnGameClockEventProcessed -= UpdateEventLog;
        GameClockEvent._OnColonistIsDead -= OnColonistDied;
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
        aliveColonistsVerticalGroupLayout.transform.hasChanged = false;
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
        _OnRequestColonistData(DataRequests.DEAD_COLONISTS);
    }

    // Reply from server (baby controller) - alive or dead are treated as colonists
    public void OnServerReply(List<BabyModel> colonists, DataRequests request)
    {
        if (colonists == null || colonists.Count == 0 || colonists.Capacity == 0)
        {
            Debug.Log("No alive/dead colonists to load in the med bay.");
            return;
        }
        
        // Clear icons; TODO only if the children of the vertical g. layout has changed since
        ClearColonistIcons(request);
        CreateColonistIcons(colonists, request);
    }

    // On colonist dead, need to put X medical bay and also exclude that colonist from next save instance
    public void OnColonistDied(GameClockEvent e, ICombatant c)
    {
        UpdateEventLog(e);
        ClearColonistIcons(c);        
    }

    public void ClearColonistIcons(DataRequests request)
    {
        VerticalLayoutGroup layout = request == (int)DataRequests.LIVE_COLONISTS? aliveColonistsVerticalGroupLayout
            : deadColonistsVerticalGroupLayout;
        int len = layout.transform.childCount;

        for (int i = 0; i < len; i++)
        {
            Destroy(layout.transform.GetChild(i).gameObject);
        }
    }

    // Overload for a specific combatant
    public void ClearColonistIcons(ICombatant c)
    {
        int len = aliveColonistsVerticalGroupLayout.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform child = aliveColonistsVerticalGroupLayout.transform.GetChild(i);
            if (child.gameObject.name.Trim().ToLower().Contains(c.Name().Trim().ToLower()))
            {
                Destroy(child.gameObject);
            }
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

    public void CreateColonistIcons(List<BabyModel> colonists, DataRequests request)
    {
        VerticalLayoutGroup layout = request == (int)DataRequests.LIVE_COLONISTS ? aliveColonistsVerticalGroupLayout
    : deadColonistsVerticalGroupLayout;

        // Replace content
        foreach (BabyModel b in colonists)
        {
            if (b != null)
            {
                // Icons with click events
                Image colonistIcon = Instantiate(UIAssets.colonistIcon.GetComponent<Image>());
                layout.childControlWidth = true;
                colonistIcon.transform.localScale = colonistIconSize;
                colonistIcon.rectTransform.SetParent(layout.transform);
                colonistIcon.gameObject.name = b.Name();
                // Add the handle mouse event trigger
                EventTrigger evt = colonistIcon.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { HandleIconMouseClick(b.UniqueColonistPersonnelID_); }); // The UUID is retrieved from the scriptable object "GameCharacterDatabase"
                evt.triggers.Add(entry);

                // TODO adjust rectTransform size - some text gets wrapped due to local scale changing to its new parent vertical layout

                // Names
                TMP_Text colonistName = Instantiate(UIAssets.colonistName.GetComponent<TextMeshProUGUI>());
                colonistName.SetText(b.Name());
                colonistName.gameObject.name = b.Name();
                colonistName.rectTransform.SetParent(layout.transform);
                colonistName.fontSize = 42.0f;
            }
        }
        layout.transform.hasChanged = true;
    }

    // The permanent assets database
    public GameCharacterDatabase gameCharacterDatabase;

    // Colonist icons (dead/alive) click handler
    public void HandleIconMouseClick(int UUID)
    {
        // Look up the UUID in the gameCharacterDatabase
        BabyModel target = gameCharacterDatabase.colonistRegistry.Find(x => x.GetHashCode() == UUID);
        if(target == null)
        {
            Debug.Log($"Target UUID {UUID} not found. Check UUID again.");
            return;
        }
        Debug.Log($"Target icon clicked: {target}");
        Debug.Log($"UUID: {target.GetHashCode()}.");
        Debug.Log($"Name: {target.Name()}.");
    }

}
