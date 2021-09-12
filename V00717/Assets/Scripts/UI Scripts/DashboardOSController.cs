using System;
using UnityEngine.EventSystems;
using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// Ports
using static Enums;

public class DashboardOSController : PageController
{
    // For characters
    public GameController GameController;
    protected int activePageIndex;
    // Dashboard OS canvas
    public Canvas dashboardOS;
    // Bridge Page
    public GameObject creatorBridgePage;
    // On Air Page
    public GameObject onAirCanvas;
    // Livestream chat
    public GameObject livestreamChat;
    // Jukebox page
    public GameObject jukeboxPage;
    // EVA-NEWS page
    public GameObject evaNewsPage;

    // Dashboard nav
    public Canvas dashboardNav;
    //// The login page
    //public Canvas loginPage;

    // Vertical group for live colonists
    public RectTransform aliveColonistsVerticalGroupLayout;
    // Vertical group for dead colonists
    public RectTransform deadColonistsVerticalGroupLayout;

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
    private int MAX_UIACTION_EVENT = CreationController.MAX_COLONISTS * 2;

    /// <summary>
    /// The event log go to append the texts
    /// </summary>
    public TMP_Text eventLogText = null;
    public TMP_Text livestreamChatText = null;

    public float eventLogYOffset = 0.0f; // Starts at 0 (anchored at top of parent container), then decreases by the decrement to go down
    public float eventLogYOffsetDecrement = 5.0f; // Default value
    public float eventLogTextTopMargin = 10.0f; // In case we need more space

    // Queue of generated event logs (we dequeue first ones when max reached)
    private Queue<string> eventLogQueue;
    private Queue<string> viewerLogQueue;

    // Obituary overlay to fade in/out when clicking on dead colonist icon
    public Canvas obituaryOverlayCanvas;
    public TMP_Text obituaryDescription; // Where we feed the generated obituary/poem
    public Image obituaryIcon;

    // Chat log overlay to fade in/out
    public Canvas chatCallOverlayCanvas;
    public TMP_Text chatDialogue; // The dialogues are generated variants from the four/five main characters
    public Image chatterIcon;

    /// <summary>
    /// The icons of the quadrants (NE, NW, SW, SE).
    /// Cached to destroy them when the selection stage is cleared.
    /// </summary>
    public List<GameObject> addedQuadrantIconsList;
    public List<GameObject> addedQuadrantActionUIEventList;

    public Dictionary<string, int> donatorListAndAmount;
    public TMP_Text donationTotalText;
    public TMP_Text donatorRankingList;
    // To erase first ten or display
    Dictionary<string, int> firstTenDonators;

    public delegate void RequestColonistData(DataRequests requestPort);
    public static event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        CreationController._OnRequestColonistDataResponse += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ReadSaveList;
        CharacterModel._OnGameClockEventProcessed += ProcessEventFromCharacter;
        CharacterModel._OnGameClockEventProcessed += ProcessEventFromViewer;
        PendingCallEvent._OnPendingCallEvent += UpdatePendingCallsLog;
        Bot._OnMainActorIsDead += OnMainActorDied;
        BattleEvent._OnBattleEnded += ProcessEventFromCharacter;
        //SeasonController._OnQuadrantSelectionAction += UpdateQuadrantSelectionUI;
        Viewer._OnNewDonationAction += SetDonationMoneyView;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
        CreationController._OnRequestColonistDataResponse -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ReadSaveList;
        CharacterModel._OnGameClockEventProcessed -= ProcessEventFromCharacter;
        CharacterModel._OnGameClockEventProcessed -= ProcessEventFromViewer;
        PendingCallEvent._OnPendingCallEvent -= UpdatePendingCallsLog;
        Bot._OnMainActorIsDead -= OnMainActorDied;
        BattleEvent._OnBattleEnded -= ProcessEventFromCharacter;
        //SeasonController._OnQuadrantSelectionAction -= UpdateQuadrantSelectionUI;
        Viewer._OnNewDonationAction -= SetDonationMoneyView;
    }

    private void Start()
    {
        eventLogQueue = new Queue<string>(MAX_EVENT_COUNT);
        viewerLogQueue = new Queue<string>(MAX_EVENT_COUNT);

        addedQuadrantIconsList = new List<GameObject>();
        addedQuadrantActionUIEventList = new List<GameObject>();

        donatorListAndAmount = new Dictionary<string, int>();
        firstTenDonators = new Dictionary<string, int>();

        if (GameController == null)
        {
            GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
    }

    /// <summary>
    /// The event log handler
    /// </summary>
    /// <param name="e"></param>
    public void ProcessEventFromCharacter(GameClockEvent e)
    {
        UpdateEventLog(e, eventLogText, eventLogQueue);
    }
    /// <summary>
    /// The viewer reaction handler
    /// 
    /// TODO should not kick in until GameController and others are initialized.
    /// </summary>
    /// <param name="e"></param>
    public void ProcessEventFromViewer(GameClockEvent e)
    {
        if(GameController == null || GameController.randomizedAuditionDatabase.actors == null || GameController.randomizedAuditionDatabase.actors.Length == 0)
        {
            return;
        }
        // Generate new viewer reaction text
        GameClockEventReaction reaction = GameController.channelController.GenerateReaction(e);
        UpdateEventLog(reaction, livestreamChatText, viewerLogQueue);
    }

    /// <summary>
    /// Used for both the event log and the viewers.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="textParent"></param>
    /// <param name="logQueue"></param>
    private void UpdateEventLog(GameClockEvent e, TMP_Text textParent, Queue<string> logQueue)
    {
        if(eventCount < MAX_EVENT_COUNT)
        {
            AppendEventLogTMPProText(e, textParent, logQueue);
            ++eventCount;
        } else
        {
            // Clear the first log TODO after archiving it elsewhere
            Debug.Log("Clearing old log");
            int eraseCount = 10;
            for(int i = 0; i < eraseCount; i++)
            {
                logQueue.Dequeue();
                --eventCount;
            }
            textParent.SetText("");
            string rebuiltLog = "";
            foreach(string message in logQueue)
            {
                rebuiltLog += message;
            }
            textParent.SetText(rebuiltLog);
        }
    }

    /// <summary>
    /// Give an event to one of the characters
    /// </summary>
    public void AssignNewUIActionEvent(int repeats)
    {
        if (addedQuadrantActionUIEventList.Count < MAX_UIACTION_EVENT)
        {
            Camera[] trackedCharacters = GameController.CreationController.LaneFeedCams;
            CharacterModel randCharacter = trackedCharacters[repeats].GetComponent<CharacterTracker>().Target.GetComponent<CharacterModel>();

            if(randCharacter == null || randCharacter.InQuadrant == -1)
            {
                Debug.Log("Not ready yet.");
                return;
            }

            GameClockEvent wayPointEvent = GameController.gameClockEventController.GenerateRandomWaypointEvent(randCharacter);
            // The randSubQuadrant should be in the same quadrant than the randCharacter
            // We can use the outgoing edges of the graph for this - it will only use the edges of the current waypoint, like a path
            GameWaypoint currentWaypoint = null;
            EdgeObject[] waypoints = null;
            GameWaypoint newWaypoint = null;
            try
            {
                currentWaypoint = GameController.quadrantMapper.gameWayPoints[randCharacter.InQuadrant];
                waypoints = currentWaypoint.edges;
                newWaypoint = null;
            } catch(IndexOutOfRangeException e)
            {
                Debug.LogError(e.Message);
                return;
            }

            for (int i = 0; i < waypoints.Length; i++)
            {
                if(waypoints[i].endPoint.waypointEvent == null)
                {
                    newWaypoint = waypoints[i].endPoint;
                    break;
                }
            }
            // All taken for now
            if(newWaypoint == null)
            {
                return;
            }

            SpawnEventAtWaypoint(newWaypoint, wayPointEvent as WaypointEvent);
            SpawnQuadrantUIActionEvent(newWaypoint, wayPointEvent as WaypointEvent, randCharacter);
            SpawnItemAtWaypoint();
        }
    }

    public void SpawnEventAtWaypoint(GameWaypoint newWaypoint, WaypointEvent waypointEvent)
    {
        newWaypoint.waypointEvent = waypointEvent;
    }

    public void SpawnItemAtWaypoint()
    {

    }

    public void SpawnQuadrantUIActionEvent(GameWaypoint newWaypoint, WaypointEvent waypointEvent, CharacterModel randCharacter)
    {
        // need the parent transform to put it in
        GameObject[] gameWaypointToCameraUIMap = GameController.quadrantMapper.gameWaypointToCameraUIMap;
        if(gameWaypointToCameraUIMap.Length == 0 || waypointEvent.actionMethodPointers == null)
        {
            return;
        }
        Transform parentTransform = gameWaypointToCameraUIMap[newWaypoint.intKey].transform;
        foreach (System.Delegate del in waypointEvent.actionMethodPointers)
        {
            Image quadrantIcon = Instantiate(UIAssets.UIAlertIcon.GetComponent<Image>(), parentTransform, true);
            StartCoroutine(DestroyQuadrantUIEvent(newWaypoint, quadrantIcon.gameObject, 5.0f));
            EventTrigger evt = quadrantIcon.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            evt.triggers.Add(entry);
            entry.callback.AddListener((eventData) => {
                bool successful = GameController.quadrantMapper.GoToQuadrant(randCharacter, newWaypoint);
                // Navmesh issues
                if(!successful)
                {
                    // Character should try to go to a random quadrant instead ?
                    Debug.Log("Failed to move to quadrant (NAVMESH WTF)");
                    return;
                }
                // Event log
                randCharacter.OnGameClockEventGenerated(waypointEvent);
                newWaypoint.waypointEvent = null;
 
                if(quadrantIcon)
                {
                    Destroy(quadrantIcon.gameObject);
                }
                addedQuadrantActionUIEventList.Remove(quadrantIcon.gameObject);
            });
            addedQuadrantActionUIEventList.Add(quadrantIcon.gameObject);
        }
    }

    public IEnumerator DestroyQuadrantUIEvent(GameWaypoint newWaypoint, GameObject quadrantIcon, float delay)
    {
        yield return new WaitForSeconds(delay);
        // also remove the actual waypoint event
        newWaypoint.waypointEvent = null;

        if(quadrantIcon)
        {
            Destroy(quadrantIcon.gameObject);
        }
        addedQuadrantActionUIEventList.Remove(quadrantIcon);
    }

    // Sets the current active menu canvas to the dashboard OS which deals with inputs by state
    public override void SetActiveMenuCanvas()
    {
        StarterAssetsInputs.SetActiveMenuCanvas(creatorBridgePage);
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
        // loginPage.enabled = false;
        dashboardNav.enabled = true;
    }

    // TODO fix active page index changed in other menus too
    public override void ChangePage(int pageIndex)
    {
        // If the bridge page is open, make it the previous page
        GameObject previouslyActiveCanvas = null;
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].activeInHierarchy)
            {
                previouslyActiveCanvas = pages[i];
                break;
            }
        }
        StarterAssetsInputs.previouslyActiveCanvas = previouslyActiveCanvas;

        // Cache the active page index
        activePageIndex = pageIndex;
        StarterAssetsInputs.activeMenuCanvas = pages[pageIndex];
        // Change page through parent's method
        base.ChangePage(pageIndex);

        // Deal with special pages
        if (GameController.CreationController != null && pageIndex == (int)DashboardPageIndexes.DATABASE)
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
    public void OnServerReply(List<GameObject> colonists, DataRequests request)
    {
        if (colonists == null || colonists.Count == 0 || colonists.Capacity == 0)
        {
            Debug.Log("No alive/dead colonists to load in the med bay.");
            return;
        }

        // Clear icons; TODO only if the children of the vertical g. layout has changed since
        RectTransform layout = request == (int)DataRequests.LIVE_COLONISTS ? aliveColonistsVerticalGroupLayout
    : deadColonistsVerticalGroupLayout;

        ClearColonistIcons(request, layout);
        AddCameraFeedActionUI(colonists, request, layout);
    }

    // On colonist dead, need to put X medical bay and also exclude that colonist from next save instance
    public void OnMainActorDied(GameObject a)
    {
        //UpdateEventLog(e);
        ClearColonistIcons(a);        
    }

    public void ClearColonistIcons(DataRequests request, Transform layout)
    {
        int len = layout.transform.childCount;

        for (int i = 0; i < len; i++)
        {
            Destroy(layout.transform.GetChild(i).gameObject);
        }
    }

    // Overload for a specific combatant
    public void ClearColonistIcons(GameObject c)
    {
        int len = aliveColonistsVerticalGroupLayout.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform child = aliveColonistsVerticalGroupLayout.transform.GetChild(i);
            if (child.gameObject.name == null || c.GetComponentInChildren<CharacterModel>().NickName == null)
            {
                return;
            }
            if (child.gameObject.name.Trim().ToLower().Contains(c.GetComponentInChildren<CharacterModel>().NickName.Trim().ToLower()))
            {
                Destroy(child.gameObject);
            }
        }
    }

    // TODO refactor out a writer component to re-use elsewhere
    public void AppendEventLogTMPProText(GameClockEvent e, TMP_Text parentText, Queue<string> logQueue)
    {
        if(e == null)
        {
            return;
        }
        if(parentText == null)
        {
            CreateEventLogTMProText(e);
        } else
        {
            TextMeshProUGUI existingText = parentText.GetComponent<TextMeshProUGUI>();
            string appendText = existingText.text;
            appendText += "\n\n" + e.Message;
            existingText.SetText(appendText);
        }

        logQueue.Enqueue(e.Message);
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
    }

    public void AddCameraFeedActionUI(List<GameObject> colonists, DataRequests request, Transform parentLayout)
    {
        // Replace content
        foreach (GameObject b in colonists)
        {
            if (b != null)
            {
                // Icons with click events
                Image colonistIcon = Instantiate(UIAssets.colonistIcon.GetComponent<Image>());
                colonistIcon.rectTransform.SetParent(parentLayout.transform);
                colonistIcon.gameObject.name = b.GetComponent<CharacterModel>().Name();
                // Add the handle mouse event trigger
                EventTrigger evt = colonistIcon.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { AddObituaryOnClick(b.GetComponent<CharacterModel>().UniqueColonistPersonnelID_); });
                evt.triggers.Add(entry);

                // Names
                TMP_Text colonistName = Instantiate(UIAssets.colonistName.GetComponent<TextMeshProUGUI>());
                colonistName.SetText(b.GetComponent<CharacterModel>().Name());
                colonistName.gameObject.name = b.GetComponent<CharacterModel>().Name();
                colonistName.rectTransform.SetParent(colonistIcon.transform);  // Childed to icon           
            }
        }
        parentLayout.transform.hasChanged = true;
    }

    /// <summary>
    /// Overload for one character:
    /// Adds chat instances in the mind chat
    /// </summary>
    /// <param name="c"></param>
    /// <param name="parentLayout"></param>
    public void AddCameraFeedActionUI(CharacterModel c, Transform parentTransform, _AddChatCallOnClick chatCallback = null, Action<CharacterModel, Image[], int> quadrantSelectionCallback = null)
    {
        if (c != null)
        {
            // Icons with click events
            Image generatedChatterIcon = Instantiate(UIAssets.colonistIcon, parentTransform, true).GetComponent<Image>();
            generatedChatterIcon.gameObject.name = c.Name();
            TMP_Text generatedChatterName = Instantiate(UIAssets.colonistName.GetComponent<TextMeshProUGUI>(), parentTransform, true);

            if(chatCallback != null)
            {
                // Add the handle mouse event trigger
                EventTrigger evt = generatedChatterIcon.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                evt.triggers.Add(entry);
                // Names
                generatedChatterName.SetText(c.Name());
                generatedChatterName.gameObject.name = c.Name();
                generatedChatterName.rectTransform.SetParent(parentTransform);
                entry.callback.AddListener((eventData) => { chatCallback(c, generatedChatterIcon, generatedChatterName); });
            }
        }
    }

    // Colonist icons (dead/alive) click handler
    public void AddObituaryOnClick(int UUID)
    {
        Debug.Log("Clicked on obituary on click icon but returning");
        if (GameController == null || GameController.DeadColonists == null)
        {
            return;
        }
        Debug.Log("Clicked on obituary on click icon");
        // Look up the UUID in the gameCharacterDatabase
        GameObject target = GameController.DeadColonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == UUID);
        // TODO separate handling living targets
        if (target == null)
        {
            target = GameController.Colonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == UUID);
        }
        if(target == null)
        {
            Debug.Log($"Target UUID {UUID} not found. Check UUID again.");
            return;
        }
        CharacterModel targetComponent = target.GetComponent<CharacterModel>();
        Debug.Log($"Target icon clicked: {target}");
        Debug.Log($"Name: {targetComponent.Name()}.");

        // Toggle on the obituary overlay
        // Cache it in the inputs system
        StarterAssetsInputs.activeOverlayScreen = obituaryOverlayCanvas;
        StarterAssetsInputs.activeOverlayScreen.enabled = true;

        // Generate an obituary
        ObituaryGenerator og = new ObituaryGenerator(targetComponent);
        String eventFrequencyTest = og.GenerateEventFrequencyText();
        string majorEventText = og.GenerateMajorEventText();

        obituaryDescription.SetText("Media Object: " + targetComponent.Name() + "\n" + "Live Status: " + targetComponent.LastEvent + "\n" + "\n" + majorEventText + "\n" + eventFrequencyTest + "\n\nPRESS ENTER TO EXIT");
    }

    // Colonist icons (dead/alive) click handler
    public delegate void _AddChatCallOnClick(CharacterModel characterModel, Image generatedChatterIcon, TMP_Text generatedChatterName);

    public void AddChatCallOnClick(CharacterModel characterModel, Image generatedChatterIcon, TMP_Text generatedChatterName)
    {
        if (GameController == null || GameController.Colonists == null)
        {
            return;
        }
        
        // Look up the UUID in the gameCharacterDatabase
        GameObject target = GameController.Colonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == characterModel.UniqueColonistPersonnelID_);
        if (target == null)
        {
            Debug.Log($"Target UUID not found. Check UUID again.");
            return;
        }
        StarterAssetsInputs.activeOverlayScreen = chatCallOverlayCanvas;
        StarterAssetsInputs.activeOverlayScreen.enabled = true;

        CharacterModel targetComponent = target.GetComponent<CharacterModel>();

        // Generate a dialogue thread
        ChatGenerator cg = new ChatGenerator(targetComponent, GameController.chatDatabaseSO);
        chatDialogue.SetText("Caller: " + targetComponent.Name() + "\n" + "Call Log:\n" + cg.GetDialogueTextByTheme() + "\n\nPRESS ENTER TO EXIT");
        StartCoroutine(ClearChat(characterModel, generatedChatterIcon, generatedChatterName, 5.0f));
    }

    /// <summary>
    /// Can return false if navigation was unsuccessful. In that case, don't destroy the icon.
    ///  
    /// </summary>
    /// <param name="characterModel"></param>
    /// <param name="gameWaypoint"></param>
    /// <returns></returns>
    public delegate void CallBackA(CharacterModel characterModel, GameWaypoint gameWaypoint);

    public void AddEventListener(Image image, CharacterModel characterModel, GameWaypoint waypoint, CallBackA callbackA = null, Action<CharacterModel, Image, WaypointEvent> callbackB = null)
    {
        // Add the handle mouse event trigger
        EventTrigger evt = image.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        evt.triggers.Add(entry);
        if(callbackA != null)
        {
            entry.callback.AddListener((eventData) => { callbackA(characterModel, waypoint); });
            //image.GetComponent<Button>().onClick.AddListener(delegate() { callbackA(characterModel, waypoint); } );
        }
        if(callbackB != null)
        {
            entry.callback.AddListener((eventData) => { callbackB(characterModel, image, waypoint.waypointEvent); });
        }
    }

    /// <summary>
    /// Deletes a list of image actions.
    /// </summary>
    /// <param name="images">The list of images to delete.</param>
    public static void ClearQuadrantUIActions(List<GameObject> images)
    {
        foreach (GameObject image in images)
        {
            Destroy(image.gameObject);
        }
    }

    /// <summary>
    /// Deletes individual actions.
    /// </summary>
    /// <param name="image">The image to delete.</param>
    public static void ClearQuadrantUIAction(Image image)
    {
        Destroy(image.gameObject);
    }

    /// <summary>
    /// TODO Sponsors money
    /// </summary>
    /// <param name="donatorName"></param>
    /// <param name="donationAmount"></param>
    public void SetDonationMoneyView(string donatorName, int donationAmount)
    {
        if(donatorListAndAmount.Count >= 100)
        {
            // Erase first few...
            // Keep the biggest ones first
            Dictionary<string, int> lastFiftyDonators = (from donator in donatorListAndAmount orderby donator.Value ascending select donator)
                                                            .Take(50)
                                                                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach(string donator in lastFiftyDonators.Keys)
            {
                donatorListAndAmount.Remove(donator);
            }
        }
        // Map used for donator ranking
        if(donatorListAndAmount.ContainsKey(donatorName))
        {
            // TODO handle hash key collisions - linear probing/double hashing/etc.
            return;
        }
        donatorListAndAmount.Add(donatorName, donationAmount);
        firstTenDonators = (from donator in donatorListAndAmount orderby donator.Value descending select donator)
                            .Take(10)
                                .ToDictionary(pair => pair.Key, pair => pair.Value);

        string donatorList = null;
        foreach(KeyValuePair<string, int> donator in firstTenDonators)
        {
            donatorList += $"{donator.Key}: ${donator.Value}\n";
        }
        donatorRankingList.text = donatorList;
        donationTotalText.text = $"Total donation money received: ${GameController.DonationMoney}";
    }

    private IEnumerator ClearChat(CharacterModel caller, Image callerIcon, TMP_Text callerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(callerIcon && callerName)
        {
            Destroy(callerIcon.gameObject);
            Destroy(callerName.gameObject);
        }
        // Reset call 
        caller.IsInPendingCall = false;
        StopCoroutine("ClearChat");
    }

    public GameObject pendingCallsParentTransform;
    private void UpdatePendingCallsLog(GameClockEvent e, CharacterModel c)
    {
        AddCameraFeedActionUI(c, pendingCallsParentTransform.transform, AddChatCallOnClick);
    }
}
