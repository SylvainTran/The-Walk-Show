using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using static SeasonController;
using UnityEngine.AI;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{
    private CharacterModelObject characterModel = null;
    public CharacterModelObject CharacterModel { get { return characterModel; } set { characterModel = value; } }
    private CreationController creationController = null;
    public CreationController CreationController { get { return creationController; } set { creationController = value; } }
    private CharacterCreationView characterCreationView = null;
    public CharacterCreationView CharacterCreationView { get { return characterCreationView; } set { characterCreationView = value; }}
    public GameClockEventController gameClockEventController = null;

    public CameraController CameraController;

    private float donationMoney = 0.0f;
    public float DonationMoney { get { return donationMoney; } set { donationMoney = value; } }

    private PlayerStatistics playerStatistics;
    public RandomizedAudition randomizedAuditionDatabase;
    public DashboardOSController dashboardOSController;
    /// <summary>
    /// The prefab of the audition menu. Comes with AuditionEditor component that randomizes its fields at start.
    /// </summary>
    public GameObject auditionEditorPrefab;
    /// <summary>
    /// The GO with Vertical Layout to parent the audition editor prefabs in.
    /// </summary>
    public Transform auditionEditorContainer;

    public List<GameObject> auditionEditorsInGame;
    public GameObject[] landingPositions;

    public PlayerStatistics GetPlayerStatistics()
    {
        return playerStatistics;
    }

    [Header("The trigger chance for any time event to occur.")]
    [Tooltip("Higher values mean less chance of a triggered event happening.")]
    public float triggerChance = 5.0f;

    [Header("The interval rate in (s) at which events will tick for any character.")]
    [Tooltip("Faster rates will accelerate the game characters' deaths greatly.")]
    public float eventIntervalRate = 5.0f;

    // Save to file event
    public delegate void SaveAction(string key, List<GameObject> c, string path);
    public static event SaveAction _OnSaveAction;

    public delegate void PlayerStatisticsAction(PlayerStatistics playerStatistics, string path, string successMessage);
    public static event PlayerStatisticsAction _OnSavePlayerStatisticsAction;

    public ChatDatabase chatDatabaseSO;
    private List<GameObject> colonists = null;
    public List<GameObject> Colonists { get { return colonists; } set { colonists = value; }}
    private List<GameObject> deadColonists = null;
    public List<GameObject> DeadColonists { get { return deadColonists; } set { deadColonists = value; } }

    /// <summary>
    /// These cameras follow/track a character in its lane (by index, going up to 3)
    /// </summary>
    public Camera[] laneFeedCams;

    /// <summary>
    /// The model to instanciate and re-look after creating a character
    /// </summary>
    public GameObject characterModelPrefab;

    /// <summary>
    /// Wrapper for the adjacency graph and methods on it
    /// that'll be used in game.
    ///
    /// Set through the inspector for now.
    /// </summary>
    public QuadrantMapper quadrantMapper;
    /// <summary>
    /// Controls the seasons (game scene order)
    /// system.
    /// </summary>
    public SeasonController seasonController;

    /// <summary>
    /// The channel controller for viewer reactions.
    /// </summary>
    public ChannelController channelController;

    public GameObject EventHighlightCameraRawImage;

    public TMP_Text auditionStatus;

    public GameObject panopticonLift;

    public CuteKaomojiDatabase cuteKaomojiDatabase;
    public InterjectionDatabase interjectionDatabase;
    public ExpletivesDatabase expletivesDatabase;
    public AdverbsDatabase adverbsDatabase;
    public EncouragementDatabase encouragementDatabase;

    /// <summary>
    /// Actors or Items that can spawn at waypoints and in-between
    /// </summary>
    public GameObject coinPrefab;
    public GameObject snakePrefab;

    // Auditioning role text
    public TMP_Text auditionRole;
    private void OnEnable()
    {
        GameClockEvent._OnColonistIsDead += OnColonistDied;
        TimeController._OnUpdateEventClock += OnEventClockUpdate;
        Viewer._OnNewDonationAction += SetDonationMoney;
        SeasonController._OnSeasonIntroAction += SetupIntroPhase;
    }

    private void OnDisable()
    {
        GameClockEvent._OnColonistIsDead -= OnColonistDied;
        TimeController._OnUpdateEventClock -= OnEventClockUpdate;
        Viewer._OnNewDonationAction -= SetDonationMoney;
        SeasonController._OnSeasonIntroAction -= SetupIntroPhase;        
    }

    public void SetDonationMoney(string donatorName, int donationAmount)
    {
        donationMoney += donationAmount;
    }

    private IEnumerator Start()
    {
#if UNITY_EDITOR        
        //chatDatabaseSO = (ChatDatabase)AssetDatabase.LoadAssetAtPath(Path.Combine(Application.dataPath + "/MyResources/ChatDatabase"), typeof(ChatDatabase));
        if (chatDatabaseSO == null)
        {
            chatDatabaseSO = ScriptableObject.CreateInstance<ChatDatabase>();
            //AssetDatabase.CreateAsset(chatDatabaseSO, Path.Combine(Application.dataPath + "/MyResources/ChatDatabase"));
        }
#endif
#if UNITY_STANDALONE || UNITY_WEBGL
        if (chatDatabaseSO == null)
        {
           chatDatabaseSO = ScriptableObject.CreateInstance<ChatDatabase>();
        }
#endif
        // Load from saved file if needed
        if (chatDatabaseSO.REGRET_THEME.Length == 0)
        {
            yield return StartCoroutine(LoadChatDatabaseJSON("chatDatabase.json"));
        }

        colonists = new List<GameObject>();
        deadColonists = new List<GameObject>();
        characterModel = new CharacterModelObject();

        creationController = new CreationController(characterModelPrefab, laneFeedCams);
        gameClockEventController = new GameClockEventController(this, triggerChance);
        // Load JSON corpus databases - TODO put in a function
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("randomizedAudition.json"));
        randomizedAuditionDatabase = JsonUtility.FromJson<RandomizedAudition>(currentFileTextRead);
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("cute_kaomoji.json"));
        cuteKaomojiDatabase = JsonUtility.FromJson<CuteKaomojiDatabase>(currentFileTextRead);
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("expletives.json"));
        expletivesDatabase = JsonUtility.FromJson<ExpletivesDatabase>(currentFileTextRead);
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("encouragements.json"));
        encouragementDatabase = JsonUtility.FromJson<EncouragementDatabase>(currentFileTextRead);
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("adverbs.json"));
        adverbsDatabase = JsonUtility.FromJson<AdverbsDatabase>(currentFileTextRead);
        yield return StartCoroutine(ReadAssetFromPlatformDependentPath("interjections.json"));
        interjectionDatabase = JsonUtility.FromJson<InterjectionDatabase>(currentFileTextRead);

        LoadGameCharacters();

        // Set game state to the intro
        seasonController = new SeasonController(this);
        // Validate the stage we're in
        if(currentGameState == GAME_STATE.SEASON_INTRO)
        {
            StartAuditions(CreationController.MAX_COLONISTS);
            SetupIntroPhase();
        }
        else
        {
            seasonController.EndAuditions();
            StartCoroutine(CloseAfterDelay(CloseSpecialEventsWindow, 5.0f));
            auditionStatus.enabled = true;
        }

        // Start specific coroutines
        channelController.GameController = this;
        channelController.GenerateRandomViewersCoroutine = StartCoroutine(channelController.GenerateRandomViewers(UnityEngine.Random.Range(0, 5)));        
    }
    public string currentFileTextRead;    
    public IEnumerator ReadAssetFromPlatformDependentPath(string relativePath)
    {
        yield return null;
        // Reset handler
        currentFileTextRead = null;
        //#if UNITY_EDITOR 
        //        resultPath = ReadFromApplicationPath(relativePath);
        //#endif
#if UNITY_EDITOR || UNITY_STANDALONE
        currentFileTextRead = ReadFromStreamingAssetsPath(relativePath);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
        // Need Unity Web Request
        yield return StartCoroutine(GetRequest(Path.Combine(Application.streamingAssetsPath, relativePath)));
        currentFileTextRead = jsonTextFromWebRequestDownloadHandler;
#endif
    }

    string jsonTextFromWebRequestDownloadHandler = null;
    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    break;
                case UnityWebRequest.Result.Success:
                    string sliced = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data, 3, webRequest.downloadHandler.data.Length - 3);
                    jsonTextFromWebRequestDownloadHandler = sliced;
                    break;
            }
        }
    }

    public string ReadFromApplicationPath(string relativePath)
    {
        return System.IO.File.ReadAllText(Path.Combine(Application.dataPath, relativePath));
    }

    public string ReadFromPersistentPath(string relativePath)
    {
        return System.IO.File.ReadAllText(Path.Combine(Application.persistentDataPath, relativePath));
    }

    public string ReadFromStreamingAssetsPath(string relativePath)
    {
        return System.IO.File.ReadAllText(Path.Combine(Application.streamingAssetsPath, relativePath));
    }

    [Serializable]
    public struct RandomizedAudition
    {
        public AuditionActor[] actors;
    }
    [Serializable]
    public struct AuditionActor
    {
        [SerializeField]
        public int age;
        [SerializeField]
        public string name;
        [SerializeField]
        public string gender;
    }
    [Serializable]
    public struct InterjectionDatabase
    {
        [SerializeField]
        public string[] interjections;
    }
    [Serializable]
    public struct ExpletivesDatabase
    {
        [SerializeField]
        public string[] expletives;
    }
    [Serializable]
    public struct EncouragementDatabase
    {
        [SerializeField]
        public string[] encouragements;
    }
    [Serializable]
    public struct AdverbsDatabase
    {
        [SerializeField]
        public string[] adverbs;
    }
    [Serializable]
    public struct CuteKaomojiDatabase
    {
        [SerializeField]
        public string[] cuteKaomoji;
    }

    public void SetupIntroPhase()
    {
        EventHighlightCameraRawImage.gameObject.SetActive(true);
        ToggleRawImageComponents(EventHighlightCameraRawImage.GetComponentInChildren<RawImage>(), EventHighlightCameraRawImage.GetComponentInChildren<TMP_Text>(), true);
    }

    public IEnumerator CloseAfterDelay(Action callbackA, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (callbackA != null)
        {
            callbackA();
        }
    }

    public void CloseSpecialEventsWindow()
    {
        ToggleRawImageComponents(EventHighlightCameraRawImage.GetComponentInChildren<RawImage>(), EventHighlightCameraRawImage.GetComponentInChildren<TMP_Text>(), false);
        EventHighlightCameraRawImage.gameObject.SetActive(false);
    }

    public void ToggleRawImageComponents(RawImage image, TMP_Text text, bool enabled)
    {
        image.enabled = enabled;
        text.enabled = enabled;
    }

    public void SetupQuadrantSelectionPhase()
    {
        for(int i = 0; i < colonists.Count; i++)
        {
            if(!colonists[i].GetComponent<NavMeshAgent>().isOnNavMesh)
            {
                colonists[i].GetComponent<NavMeshAgent>().Warp(landingPositions[i].transform.position);
            }
        }
    }

    /// <summary>
    /// Assigns a quadrant to its character.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="quadrantWaypoint"></param>
    /// <returns></returns>
    public void AssignQuadrantData(CharacterModel c, GameWaypoint quadrantWaypoint)
    {
        // Assign new owner for that waypoint in SeasonController using the intKey property
        switch (quadrantWaypoint.intKey)
        {
            case 0:
            case 1:
            case 2:
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
            case 4:
            case 5:
            case 6:
            case 7:
                {
                    if (SeasonController.quadrantNWOwner == null)
                    {
                        SeasonController.quadrantNWOwner = c;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
            case 8:
            case 9:
            case 10:
            case 11:
                {
                    if (SeasonController.quadrantSWOwner == null)
                    {
                        SeasonController.quadrantSWOwner = c;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
            case 12:
            case 13:
            case 14:
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
        ScavengingPhaseFlag();
    }

    public void ScavengingPhaseFlag()
    {
        QUADRANTS_ASSIGNED++;
        if (QUADRANTS_ASSIGNED == CreationController.MAX_COLONISTS)
        {
            foreach (GameObject g in colonists)
            {
                quadrantMapper.GoToQuadrant(g.GetComponent<CharacterModel>(), quadrantMapper.gameWayPoints[g.GetComponent<CharacterModel>().InQuadrant]);
            }
        }
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

    public void SetupStartingQuadrant(GameObject newCharacterMesh)
    {
        Transform[] cameraLanes = { CameraController.cameraLane1TargetCallTransform, CameraController.cameraLane2TargetCallTransform, CameraController.cameraLane3TargetCallTransform, CameraController.cameraLane4TargetCallTransform };
        // AUTO ASSIGNATION: cameras 1-4
        GameWaypoint[] waypoints = quadrantMapper.gameWayPoints;
        int mappedWaypointIndex = remap(SeasonController.QUADRANTS_ASSIGNED, 0, cameraLanes.Length, SeasonController.QUADRANTS_ASSIGNED + (3 * SeasonController.QUADRANTS_ASSIGNED) + 1, SeasonController.QUADRANTS_ASSIGNED * 4 + 4);
        newCharacterMesh.GetComponent<CharacterModel>().InQuadrant = mappedWaypointIndex;
        AssignQuadrantData(newCharacterMesh.GetComponent<CharacterModel>(), waypoints[mappedWaypointIndex]);
    }

    public DraggedActionHandler[] draggedActionHandlers;

    public void LoadGameCharacters()
    {
        // First load game if needed (TODO validate contents too, can have bad format and exist)
        if (SaveSystem.SaveFileExists("PlayerStatistics.json"))
        {
            playerStatistics = LoadPlayerStatistics("PlayerStatistics.json");
            //  Update UUIDs
            CharacterModelObject.uniqueColonistPersonnelID = playerStatistics.characterUUIDCount;
            // Load current game state
            currentGameState = (GAME_STATE)playerStatistics.currentGameState;
        }
        else // New game
        {
            playerStatistics = new PlayerStatistics();
            playerStatistics.characterUUIDCount = 0;
            playerStatistics.currentGameState = 0;
            currentGameState = 0;
        }
        if (SaveSystem.SaveFileExists("colonists.json"))
        {
            LoadCharactersFromJSONFile(colonists, "colonists.json", true, true);
            // Update camera lanes and draggedActionHandlers (Toolbelt)
            // Setup dragged action handler with this actor's UUID
            for(int i = 0; i < colonists.Count; i++)
            {
                creationController.SetTrackLanePosition(creationController.FindAvailableCameraLane(), colonists[i].transform);
                SetupStartingQuadrant(colonists[i]);

                if(draggedActionHandlers[i].ActionActorTargetUUID != -1)
                {
                    continue;
                }
                draggedActionHandlers[i].ActionActorTargetUUID = colonists[i].GetComponent<CharacterModel>().UniqueColonistPersonnelID_;
            }
        }
        if (SaveSystem.SaveFileExists("deadColonists.json"))
        {
            LoadCharactersFromJSONFile(deadColonists, "deadColonists.json", false, true);
        }
    }

    public PlayerStatistics LoadPlayerStatistics(string path)
    {
        string text = ReadFromPersistentPath(path);
        PlayerStatistics deserializedObject = JsonUtility.FromJson<PlayerStatistics>(text);

        return deserializedObject;
    }

    // Creates an array of baby models from the json text read and deserialized from path
    public bool LoadCharactersFromJSONFile(List<GameObject> characters, string path, bool deleteIfEmpty, bool instantiateGO)
    {
        if(path == null || !SaveSystem.SaveFileExists(path))
        {
            return false;
        }
        // Generate new characters based on JSON file
        string text = null;
        SaveSystem.SavedArrayObject deserializedObject;
        List<CharacterModelObject> deserializedObjectList = null;
        try
        {
            text = ReadFromPersistentPath(path); // PERSISTENT PATH
            // TODO exception special case for empty file / corrupt
            if(text.Length == 0 || text == null)
            {
                return false; 
            }
            deserializedObject = JsonUtility.FromJson<SaveSystem.SavedArrayObject>(text);
            deserializedObjectList = new List<CharacterModelObject>(deserializedObject.colonists);
        } catch(Exception e)
        {
            Debug.LogError(e.Message);
        }

        if (deserializedObjectList == null || deserializedObjectList.Count == 0 || deserializedObjectList[0] == null)
        {
            Debug.Log("No characters to load.");
            // Delete file if specified
            if (deleteIfEmpty)
            {
                File.Delete(Application.dataPath + "/" + path);
            }
            return false;
        }
        for (int i = 0; i < deserializedObjectList.Count; i++)
        {
            if (instantiateGO)
            {
                GameObject newCharacter = GameObject.Instantiate(characterModelPrefab, landingPositions[i].transform.position, Quaternion.identity);
                newCharacter.transform.SetParent(landingPositions[i].transform);
                newCharacter.GetComponent<CharacterModel>().InitEventsMarkersFeed();
                newCharacter.GetComponent<CharacterModel>().InitCharacterModel(deserializedObjectList[i]); // Should get its uuid from the field, which got its value from previous static uuid, which was updated - we should expect anyways
                newCharacter.GetComponent<CharacterModel>().InitEventsMarkersFeed(deserializedObjectList[i]);
                newCharacter.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ = deserializedObjectList[i].UniqueColonistPersonnelID_; // Sets the uuid field, not the static one as it wont be serialized
                characters.Add(newCharacter);
            }            
        }
        return true;
    }

    public void OnEventClockUpdate()
    {
        if(gameClockEventController == null)
        {
            return;
        }
        gameClockEventController.OnEventClockUpdate();
    }

    public void deleteSaveFile()
    {
        if (colonists.Count == 0)
        {
            File.Delete(Application.dataPath + "/" + "colonists.json");
        }
    }

    public void OnColonistDied(GameClockEvent e, GameObject c)
    {
        // Remove the dead before saving again
        colonists.Remove(c);
        deleteSaveFile();
        deadColonists.Add(c);
        _OnSaveAction("colonists", deadColonists, "deadColonists.json");
    }

    public void InitAuditions()
    {
        StartAuditions(CreationController.MAX_COLONISTS - colonists.Count);
    }

    /// <summary>
    /// Used to validate character status on loading a new game or in the game
    /// </summary>
    public bool NumberOfCharactersBelowMax()
    {
        return colonists.Count < CreationController.MAX_COLONISTS;
    }

    /// <summary>
    /// This is the character casting (creation) step.
    /// </summary>
    public void StartAuditions(int n)
    {
        auditionEditorsInGame = new List<GameObject>();
        // Take the prefab and randomize starting fields exposed in the AuditorEditor's fields including mesh choices
        for (int i = 0; i < n; i++)
        {
            CreateNewAuditionEditor();
        }
    }

    /// <summary>
    /// This is the character casting (creation) step.
    /// </summary>
    public void StartAuditionsAfterDelay(int n)
    {
        // Take the prefab and randomize starting fields exposed in the AuditorEditor's fields including mesh choices
        for (int i = 0; i < n; i++)
        {
            StartCoroutine(CreateNewEditor(i * 10 + 5));
        }
    }

    // Instantiates a character as well as an audition editor
    public void CreateNewAuditionEditor()
    {
        // Sometimes this can get called (using IEnumerator coroutine) after we already changed state, so don't do aynthing if not in intro state
        if (currentGameState != GAME_STATE.SEASON_INTRO)
        {
            return;
        }

        GameObject ae = Instantiate(auditionEditorPrefab, auditionEditorContainer.transform, true);
        auditionEditorsInGame.Add(ae);
        ae.gameObject.GetComponent<CharacterCreationView>().newCharacterModelInstance = Instantiate(characterModelPrefab, CharacterCreationView.characterModelPrefabInstanceCoords + new Vector3(0.0f, 25.0f, 0.0f), Quaternion.identity);
        ae.gameObject.GetComponent<CharacterCreationView>().newCharacterModelInstance.GetComponent<CharacterModel>().InitEventsMarkersFeed(); // Inits events feed and last event but they're null at this stage
        ae.gameObject.GetComponent<AuditionEditor>().RandomizeFields();
    }

    public IEnumerator CreateNewEditor(float delay)
    {
        if(NumberOfCharactersBelowMax())
        {
            yield return new WaitForSeconds(delay);
            CreateNewAuditionEditor();
        }
        else
        {
            auditionStatus.enabled = true;
        }
    }

    // The save method service for the client - FIXME this is for through the creation menu
    public void Save()
    {
        // Event to save the current baby template to a file
        if (colonists.Count > CreationController.MAX_COLONISTS)
        {
            return;
        }
        int count = colonists.Count;
        // We don't check for max elements if saving dead colonists (for now)
        if (count <= CreationController.MAX_COLONISTS) // The last one being added makes it equal to MAX_COLONISTS
        {
            // TODO add dead colonists unique ID too?
            //SaveToJSONFile(key, nbElements, savedObject, path, "Save successful");
            if (colonists.Count > 0)
            {
                _OnSaveAction("colonists", colonists, "colonists.json");
            }
            if (deadColonists.Count > 0)
            {
                // Needs to load up the previous dead colonists first before rewriting
                _OnSaveAction("colonists", deadColonists, "deadColonists.json");
            }
        }
        else
        {
            Debug.Log("Save game impossible :-(. Full capacity reached.");
        }
    }

    private IEnumerator ResetButton(Button button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    }

    // TODO CSV Version for ease of editing?
    public Dictionary<string, string> LoadChatDatabase(string dbName)
    {
        string pathToDb = $"{Application.dataPath}/{dbName}";
        string line = null;
        string[] quotedLine = null;
        string[] quotedText = { "\"" };
        Dictionary<string, string> chatDatabase = new Dictionary<string, string>();
        try
        {
            // Open the text file using a stream reader and the ",,," csv delimiter
            using (var sr = new StreamReader($"{pathToDb}"))
            {
                line = sr.ReadToEnd();
                quotedLine = line.Split(quotedText, System.StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < quotedLine.Length; i++)
                {
                    if(i > 2 && i % 2 != 0)
                    {
                        chatDatabase.Add(quotedLine[i - 1].Substring(0, quotedLine[i].IndexOf(',')), quotedLine[i]);
                    }
                }
            }
            return chatDatabase;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }
    /// <summary>
    /// Wrapper class to serialize the database that contains arrays.
    /// </summary>
    [Serializable]
    internal class JSONDatabase
    {
        public string[] STRESS_THEME = null;
        public string[] REGRET_THEME = null;
        public string[] REALIZATION_THEME = null;
        public string[] DENIAL_THEME = null;
        public string[] GUILT_THEME = null;
        public string[] FEAR_THEME = null;
    }

    // JSON VERSION
    private IEnumerator LoadChatDatabaseJSON(string path)
    {
        yield return ReadAssetFromPlatformDependentPath(path);
        JSONDatabase deserializedObject = JsonUtility.FromJson<JSONDatabase>(currentFileTextRead);
        chatDatabaseSO.STRESS_THEME = deserializedObject.STRESS_THEME;
        chatDatabaseSO.REGRET_THEME = deserializedObject.REGRET_THEME;
        chatDatabaseSO.REALIZATION_THEME = deserializedObject.REALIZATION_THEME;
        chatDatabaseSO.DENIAL_THEME = deserializedObject.DENIAL_THEME;
        chatDatabaseSO.GUILT_THEME = deserializedObject.GUILT_THEME;
        chatDatabaseSO.FEAR_THEME = deserializedObject.FEAR_THEME;
    }

    // DEBUG MODE: Nothing here
    // Update character registry
    private void OnApplicationQuit()
    {
        if (colonists != null && colonists.Count > 0)
        {
            _OnSaveAction("colonists", colonists, "colonists.json");
        }
        if (deadColonists != null && deadColonists.Count > 0)
        {
            _OnSaveAction("colonists", deadColonists, "deadColonists.json");
        }
        // Update the UUID count and save it
        playerStatistics.characterUUIDCount = CharacterModelObject.uniqueColonistPersonnelID;
        playerStatistics.currentGameState = (int)SeasonController.currentGameState;
        _OnSavePlayerStatisticsAction(playerStatistics, "PlayerStatistics.json", "successfully saved player stats.");
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
