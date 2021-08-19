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
    public ChatDatabase chatDatabaseSO;
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

    // Transforms for parenting pending calls
    public Transform cameraLane1TargetCallTransform;
    public Transform cameraLane2TargetCallTransform;
    public Transform cameraLane3TargetCallTransform;
    public Transform cameraLane4TargetCallTransform;

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

    public TMP_Text eventLogText = null;

    public float eventLogYOffset = 0.0f; // Starts at 0 (anchored at top of parent container), then decreases by the decrement to go down
    public float eventLogYOffsetDecrement = 5.0f; // Default value
    public float eventLogTextTopMargin = 10.0f; // In case we need more space

    // Queue of generated event logs (we dequeue first ones when max reached)
    private Queue<string> eventLogQueue;
    private Queue<WaypointEvent> eventQueue;

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
    public List<Image> addedQuadrantIconsList;
    public List<Image> addedQuadrantActionUIEventList;

    public delegate void RequestColonistData(DataRequests requestPort);
    public static event RequestColonistData _OnRequestColonistData;

    private void OnEnable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS += SetActiveMenuCanvas;
        CreationController._OnRequestColonistDataResponse += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ReadSaveList;
        CharacterModel._OnGameClockEventProcessed += UpdateEventLog;
        PendingCallEvent._OnPendingCallEvent += UpdatePendingCallsLog;
        GameClockEvent._OnColonistIsDead += OnColonistDied;
        BattleEvent._OnBattleEnded += UpdateEventLog;
        SeasonController._OnQuadrantSelectionAction += UpdateQuadrantSelectionUI;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= SetActiveMenuCanvas;
        CreationController._OnRequestColonistDataResponse -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ReadSaveList;
        CharacterModel._OnGameClockEventProcessed -= UpdateEventLog;
        PendingCallEvent._OnPendingCallEvent -= UpdatePendingCallsLog;
        GameClockEvent._OnColonistIsDead -= OnColonistDied;
        BattleEvent._OnBattleEnded -= UpdateEventLog;
        SeasonController._OnQuadrantSelectionAction -= UpdateQuadrantSelectionUI;
    }

    private void Start()
    {
        eventLogQueue = new Queue<string>(MAX_EVENT_COUNT);
        addedQuadrantIconsList = new List<Image>();
        eventQueue = new Queue<WaypointEvent>(MAX_UIACTION_EVENT);
        addedQuadrantActionUIEventList = new List<Image>();

        if (GameController == null)
        {
            GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
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
    /// <summary>
    /// Updates the quadrants with selection UI.
    /// </summary>
    public void UpdateQuadrantSelectionUI()
    {
        Transform[] cameraLanes = { cameraLane1TargetCallTransform, cameraLane2TargetCallTransform, cameraLane3TargetCallTransform, cameraLane4TargetCallTransform };
        GameObject[] quadrantIcons = new GameObject[] { UIAssets.UIQuadrantIcon, UIAssets.UIQuadrantIcon, UIAssets.UIQuadrantIcon, UIAssets.UIQuadrantIcon };
        for(int i = 0; i < cameraLanes.Length; i++)
        {
            SetQuadrantUIOnClick(GameController.Colonists[i].GetComponent<CharacterModel>(), quadrantIcons, cameraLanes[i]);
        }
    }

    /// <summary>
    /// Give an event to one of the characters
    /// </summary>
    public void AssignNewUIActionEvent()
    {
        if (addedQuadrantActionUIEventList.Count < MAX_UIACTION_EVENT)
        {
            List<GameObject> characters = GameController.Colonists;

            CharacterModel randCharacter = characters[UnityEngine.Random.Range(0, characters.Count)].GetComponent<CharacterModel>();
            WaypointEvent wayPointEvent = GameController.gameClockEventController.GenerateRandomWaypointEvent(randCharacter);

            // The randSubQuadrant should be in the same quadrant than the randCharacter
            // We can use the outgoing edges of the graph for this - it will only use the edges of the current waypoint, like a path
            GameWaypoint currentWaypoint = GameController.quadrantMapper.gameWayPoints[randCharacter.InQuadrant];
            EdgeObject[] waypoints = currentWaypoint.edges;
            GameWaypoint newWaypoint = null;

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

            SpawnEventAtWaypoint(newWaypoint, wayPointEvent);
            SpawnQuadrantUIActionEvent(newWaypoint, wayPointEvent, randCharacter);
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
        Dictionary<int, GameObject> gameWaypointToCameraUIMap = GameController.quadrantMapper.gameWaypointToCameraUIMap;
        if(gameWaypointToCameraUIMap.Count == 0 || waypointEvent.actionMethodPointers == null)
        {
            return;
        }
        Transform parentTransform = gameWaypointToCameraUIMap[newWaypoint.intKey].transform;
        //Image icon = e.GetEventIcon();

        // Check what to spawn depending on the waypoint event (e.g., cue to go, cue to avoid)
        Image quadrantIcon;

        foreach(Action<CharacterModel, GameWaypoint> action in waypointEvent.actionMethodPointers)
        {
            // Create a new quadrantIcon with this action as callback for all of the actions in the waypoint event
            // TODO put fancy icons in the events - need the same with quadrants selection
            quadrantIcon = Instantiate(UIAssets.UIQuadrantIcon.GetComponent<Image>(), parentTransform, true);
            // Start timeout to destroy the event
            StartCoroutine(DestroyQuadrantUIEvent(newWaypoint, quadrantIcon, 5.0f));
            // TODO show timer

            // TODO refactor into this method (work with method group?):
            //AddEventListener(quadrantIcon, target.GetComponent<CharacterModel>(), GameController.quadrantMapper.gameWayPoints[randSubQuadrant], ,AssignGameWaypointData);
            // Add the handle mouse event trigger
            EventTrigger evt = quadrantIcon.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            evt.triggers.Add(entry);
            entry.callback.AddListener((eventData) => {
                action(randCharacter, newWaypoint);
                // Event log
                randCharacter.OnGameClockEventGenerated(waypointEvent);
                // Remove event
                newWaypoint.waypointEvent = null;
                // may have been deleted from auto timer already
                if(quadrantIcon)
                {
                    Destroy(quadrantIcon.gameObject);
                }
                addedQuadrantActionUIEventList.Remove(quadrantIcon);
            });

            addedQuadrantActionUIEventList.Add(quadrantIcon);
        }
    }

    public IEnumerator DestroyQuadrantUIEvent(GameWaypoint newWaypoint, Image quadrantIcon, float delay)
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
        if(creatorBridgePage.activeInHierarchy)
        {
            previouslyActiveCanvas = creatorBridgePage;
        } else if (onAirCanvas.activeInHierarchy)
        {
            previouslyActiveCanvas = onAirCanvas;
        } else if (livestreamChat.activeInHierarchy)
        {
            previouslyActiveCanvas = livestreamChat;
        } else if (jukeboxPage.activeInHierarchy)
        {
            previouslyActiveCanvas = jukeboxPage;
        } else if (evaNewsPage.activeInHierarchy)
        {
            previouslyActiveCanvas = evaNewsPage;
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
    public void OnColonistDied(GameClockEvent e, GameObject c)
    {
        UpdateEventLog(e);
        ClearColonistIcons(c);        
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
            // Todo fix no name error
            Transform child = aliveColonistsVerticalGroupLayout.transform.GetChild(i);
            if (child.gameObject.name == null || c.GetComponent<CharacterModel>().Name() == null)
            {
                return;
            }
            if (child.gameObject.name.Trim().ToLower().Contains(c.GetComponent<CharacterModel>().Name().Trim().ToLower()))
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
    public void AddCameraFeedActionUI(CharacterModel c, Transform parentTransform, bool takeParentPosition, Action<CharacterModel, Image, TMP_Text, int> chatCallback = null, Action<CharacterModel, Image[], int> quadrantSelectionCallback = null)
    {
        if (c != null)
        {
            // Icons with click events
            Image colonistIcon = Instantiate(UIAssets.colonistIcon.GetComponent<Image>());
            colonistIcon.rectTransform.SetParent(parentTransform);
            if(takeParentPosition)
            {
                colonistIcon.rectTransform.position = parentTransform.position;
            }
            colonistIcon.gameObject.name = c.Name();
            TMP_Text colonistName = Instantiate(UIAssets.colonistName.GetComponent<TextMeshProUGUI>());

            if (takeParentPosition)
            {
                colonistName.rectTransform.position = parentTransform.position;
            }
            if(chatCallback != null)
            {
                // Add the handle mouse event trigger
                EventTrigger evt = colonistIcon.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                evt.triggers.Add(entry);
                // Names
                colonistName.SetText(c.Name());
                colonistName.gameObject.name = c.Name();
                colonistName.rectTransform.SetParent(parentTransform);
                entry.callback.AddListener((eventData) => { chatCallback(c, colonistIcon, colonistName, c.UniqueColonistPersonnelID_); });
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
        Debug.Log($"UUID: {target.GetHashCode()}.");
        Debug.Log($"Name: {targetComponent.Name()}.");

        // Toggle on the obituary overlay
        // Cache it in the inputs system
        StarterAssetsInputs.activeOverlayScreen = obituaryOverlayCanvas;
        StarterAssetsInputs.activeOverlayScreen.enabled = true;

        // Generate an obituary
        ObituaryGenerator og = new ObituaryGenerator(targetComponent);
        String eventFrequencyTest = og.GenerateEventFrequencyText();
        string majorEventText = og.GenerateMajorEventText();

        obituaryDescription.SetText("Media Object: " + targetComponent.Name() + "\n" + "Live Status: " + targetComponent.LastEvent + "\n" + "\n" + majorEventText + "\n" + eventFrequencyTest);
    }

    // Colonist icons (dead/alive) click handler
    public void AddChatCallOnClick(CharacterModel caller, Image callerIcon, TMP_Text callerName, int UUID)
    {
        if (GameController == null || GameController.Colonists == null)
        {
            return;
        }
        // Look up the UUID in the gameCharacterDatabase
        GameObject target =GameController.Colonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == UUID);
        if (target == null)
        {
            Debug.Log($"Target UUID {UUID} not found. Check UUID again.");
            return;
        }
        StarterAssetsInputs.activeOverlayScreen = chatCallOverlayCanvas;
        StarterAssetsInputs.activeOverlayScreen.enabled = true;

        CharacterModel targetComponent = target.GetComponent<CharacterModel>();

        // Generate a dialogue thread
        ChatGenerator cg = new ChatGenerator(targetComponent, chatDatabaseSO);
        chatDialogue.SetText("Caller: " + targetComponent.Name() + "\n" + "Call Log:\n" + cg.GetDialogueTextByTheme());
        StartCoroutine(ClearChat(caller, callerIcon, callerName, 5.0f));
    }

    public void AddEventListener(Image image, CharacterModel characterModel, GameWaypoint waypoint, Action<CharacterModel, GameWaypoint> callbackA = null, Action<CharacterModel, Image, WaypointEvent> callbackB = null)
    {
        // Add the handle mouse event trigger
        EventTrigger evt = image.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        evt.triggers.Add(entry);
        if(callbackA != null)
        {
            entry.callback.AddListener((eventData) => { callbackA(characterModel, waypoint); });
        }
        if(callbackB != null)
        {
            entry.callback.AddListener((eventData) => { callbackB(characterModel, image, waypoint.waypointEvent); });
        }
    }

    public void AssignGameWaypointData(CharacterModel c, Image image, WaypointEvent e)
    {

    }

    /// <summary>
    /// Assigns a quadrant to its character.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="quadrantWaypoint"></param>
    /// <returns></returns>
    public void AssignQuadrantData(CharacterModel c, GameWaypoint quadrantWaypoint)
    {
        if(c.InQuadrant > -1 || SeasonController.currentGameState != SeasonController.GAME_STATE.QUADRANT_SELECTION)
        {
            return;
        }
        // Make the character go to quadrantWaypoint
        // Assign new owner for that waypoint in SeasonController using the intKey property
        switch (quadrantWaypoint.intKey)
        {
            case 0: case 1: case 2:
            case 3:
                {
                    if (SeasonController.quadrantNEOwner == null)
                    {
                        SeasonController.quadrantNEOwner = c;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
            case 4: case 5: case 6: case 7:
                {
                    if (SeasonController.quadrantNWOwner == null)
                    {
                        SeasonController.quadrantNWOwner = c;
                    } else
                    {
                        return;
                    }
                    break;
                }
            case 8: case 9: case 10:
            case 11:
                {
                    if (SeasonController.quadrantSWOwner == null)
                    {
                        SeasonController.quadrantSWOwner = c;
                    } else
                    {
                        return;
                    }
                    break;
                }
            case 12: case 13: case 14:
            case 15:
                {
                    if (SeasonController.quadrantSEOwner == null)
                    {
                        SeasonController.quadrantSEOwner = c;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
            default:
                break;
        }
        c.InQuadrant = quadrantWaypoint.intKey;

        c.GetComponent<Bot>().MoveToQuadrant(GameController.quadrantMapper.gameWayPoints[quadrantWaypoint.intKey]);
        SeasonController.ScavengingPhaseFlag(addedQuadrantIconsList);
    }

    /// <summary>
    /// Deletes a list of image actions.
    /// </summary>
    /// <param name="images">The list of images to delete.</param>
    public static void ClearQuadrantUIActions(List<Image> images)
    {
        foreach (Image image in images)
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
    /// Remap the values of quadrants 1-4 to the range of waypoints in all quadrants
    /// which is 16, or 0-15 in zero based-indexing.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public int remap(int value, int from1, int to1, int from2, int to2)
    {
        // Map waypoints: 0 -> 0-3, 1 -> 4-7, 2 -> 8-11, 3 -> 12-15
        int randOffset = UnityEngine.Random.Range(0, 3);
        // Transform the range and clamp to be sure
        int origin = value - from1;
        int bias = origin / to1;
        int scale = bias * to2;
        int offset = scale + from2;
        int result = offset += randOffset;

        result = Mathf.Clamp(result, 0, 15);
        return result;
    }

    // We're getting indexes from 0-3 and we want them remapped from 0-3,4-7,8-11,12,15
    // The awkward index i is used to adjust the new from2 parameter (for translating the desired base or origin translation).
    public void SetQuadrantUIOnClick(CharacterModel character, GameObject[] quadrantIcons, Transform parent)
    {
        if (GameController == null || GameController.Colonists == null)
        {
            return;
        }
        GameWaypoint[] waypoints = GameController.quadrantMapper.gameWayPoints;
        int mappedWaypointIndex = 0;

        for (int i = 0; i < quadrantIcons.Length; i++)
        {
            Image quadrantIcon = Instantiate(quadrantIcons[i].GetComponent<Image>(), parent, true);
            addedQuadrantIconsList.Add(quadrantIcon);
            mappedWaypointIndex = remap(i, 0, quadrantIcons.Length, i + (3 * i) + 1, i * 4 + 4);
            AddEventListener(quadrantIcon, character, waypoints[mappedWaypointIndex], AssignQuadrantData);
        }
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

    private void UpdatePendingCallsLog(GameClockEvent e, CharacterModel c)
    {
        // Check which camera lane transform to parent the new icon
        Transform targetParent = null;

        if(c.TrackLanePosition == 0)
        {
            targetParent = cameraLane1TargetCallTransform;
        } else if(c.TrackLanePosition == 1)
        {
            targetParent = cameraLane2TargetCallTransform;
        } else if(c.TrackLanePosition == 2)
        {
            targetParent = cameraLane3TargetCallTransform;
        } else if(c.TrackLanePosition == 3)
        {
            targetParent = cameraLane4TargetCallTransform;
        }
        AddCameraFeedActionUI(c, targetParent, true);
    }
}
