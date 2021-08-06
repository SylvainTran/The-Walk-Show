using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// This class contains the baby model and controls its attributes
// TODO refactor monobehaviour specific routines into a seperate class, in order to add a normal constructor
public class BabyController : MonoBehaviour, ISaveableComponent
{
    // The Baby Model component (cached through inspector)
    [SerializeField] private BabyModel babyModel;
    public BabyModel BabyModel { get { return babyModel; } set { babyModel = value; } }

    // The colonists 
    public List<BabyModel> colonists;
    public List<BabyModel> deadColonists = null;

    // The permanent assets database
    public GameCharacterDatabase gameCharacterDatabase;

    // The max n of colonists (temporary n)
    [NonSerialized] public static int MAX_COLONISTS = 4;

    // The UI tooltip event
    public delegate void ToolTipAction(string text);
    public static event ToolTipAction _OnToolTipAction; // listened to by View.cs

    // The UI tooltip exit event
    public delegate void ToolTipActionExit();
    public static event ToolTipActionExit _OnToolTipExitAction;

    // Delegate for changing sex
    public delegate void SexChangeAction(string sex);
    public static event SexChangeAction _OnSexChanged; // listened to by SoundController.cs

    // Delegate for changing adult height
    public delegate void AdultHeightChangeAction(float value);
    public static event AdultHeightChangeAction _OnAdultHeightChanged; // listened to by View.cs

    // Notify view of skin color changed
    public delegate void SkinColorChanged();
    public static event SkinColorChanged _OnSkinColorChanged; // listened to by View.cs

    // Notify view of head or torso mesh changed
    public delegate void MeshChanged(int type);
    public static event MeshChanged _OnMeshChanged; // listened to by View.cs

    // Save to file event
    public delegate void SaveAction(string key, List<BabyModel> c, BabyModel b, string path);
    public static event SaveAction _OnSaveAction;

    // SERVER REQUESTS
    public delegate void RequestColonistDataResponse(List<BabyModel> colonists, Enums.DataRequests request);
    public static event RequestColonistDataResponse _OnRequestColonistDataResponse;

    // Update character registry
    private void OnApplicationQuit()
    {
        Debug.Log("Saving game registry before exiting...");
        foreach (BabyModel dc in deadColonists)
        {
            BabyModel target = gameCharacterDatabase.colonistRegistry.Find(x => x.UniqueColonistPersonnelID_ == dc.UniqueColonistPersonnelID_);
            target.eventMarkersMap = dc.eventMarkersMap;
            target.SetLastEvent(dc.LastEvent);
        }
        _OnSaveAction("colonists", colonists, babyModel, "colonists.json");
        _OnSaveAction("colonists", deadColonists, babyModel, "deadColonists.json");
        _OnSaveAction("colonists", gameCharacterDatabase.colonistRegistry, babyModel, "colonistRegistry.json");
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    // Attach method functions
    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += MallocNewCharacter;
        DashboardOSController._OnRequestColonistData += OnServerReply;
        SaveSystem._SuccessfulSaveAction += ResetCharacterCache;
        GameClockEvent._OnColonistIsDead += OnColonistDied;
    }

    // Dettach method functions
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= MallocNewCharacter;
        DashboardOSController._OnRequestColonistData -= OnServerReply;
        SaveSystem._SuccessfulSaveAction -= ResetCharacterCache;
        GameClockEvent._OnColonistIsDead -= OnColonistDied;
    }

    public void ResetCharacterCache()
    {
        MallocNewCharacter();
    }

    // Creates an array of baby models from the json text read and deserialized from path
    public List<BabyModel> LoadCharactersFromJSONFile(string path, bool deleteIfEmpty)
    {
        // Generate new characters based on JSON file
        string text = System.IO.File.ReadAllText(path);
        SaveSystem.SavedArrayObject deserializedObject = JsonUtility.FromJson<SaveSystem.SavedArrayObject>(text);
        //SaveSystem.SavedArrayObject deserializedObject = JsonConvert.DeserializeObject<SaveSystem.SavedArrayObject>(text);
        if(deserializedObject.colonists == null || deserializedObject.colonists.Length == 0)
        {
            Debug.Log("No alive/dead colonists to load.");
            // Delete file if specified
            if(deleteIfEmpty)
            {
                File.Delete(path);
            }
            return null;
        }
        List<BabyModel> _colonists = new List<BabyModel>();
        for(int i = 0; i < deserializedObject.colonists.Length; i++)
        {
            _colonists.Add(deserializedObject.colonists[i]);
        }
        return _colonists;
    }

    // Malloc baby model and the colonists array
    private void Awake()
    {
        ResetCharacterCache();
    }

    private void Start()
    {
        // First load game if needed (TODO validate contents too, can have bad format and exist)
        if(SaveSystem.SaveFileExists("colonists.json"))
        {
            colonists = LoadCharactersFromJSONFile("colonists.json", true);
        }
        if (SaveSystem.SaveFileExists("deadColonists.json"))
        {
            deadColonists = LoadCharactersFromJSONFile("deadColonists.json", false);
        }
        if (SaveSystem.SaveFileExists("colonistRegistry.json"))
        {
            gameCharacterDatabase.colonistRegistry = LoadCharactersFromJSONFile("colonistRegistry.json", false);
            // Update the UUID count by using the length of the registry list
            gameCharacterDatabase.colonistUUIDCount = gameCharacterDatabase.colonistRegistry.Count;
        }
    }

    // Re-allocate memory for a new character
    public void MallocNewCharacter()
    {
        babyModel = new BabyModel();
    }

    // Setter for new colonist name
    public void OnNameChanged(string name)
    {
        babyModel.CharacterName = name;
        Debug.Log($"And so {babyModel.Name()} was given his name.");
    }

    // Setter for new colonist nickname
    public void OnNickNameChanged(string nickName)
    {
        babyModel.NickName = nickName;
        Debug.Log($"And so {babyModel.NickName} was given his nickname.");
    }

    //Setter for baby's sex via Unity's built-in event system.
    public void OnSexChanged(string sex)
    {
        babyModel.Sex = sex;
        Debug.Log($"Baby's sex was changed to: {sex}");
        // Call listeners - Sound, Meta-Narrator, etc.
        _OnSexChanged(sex);
    }

    //Setter for baby's adult height via Unity's built-in event system.
    public void OnAdultHeightChanged(float adultHeight)
    {
        babyModel.AdultHeight = adultHeight;
        _OnAdultHeightChanged(adultHeight); // Call view to update height marker
        //Debug.Log($"Baby's adult height was changed to: {adultHeight}");
    }

    //Setter for skin color changed (red slider)
    public void OnSkinColorChanged_R(float r)
    {
        babyModel.SkinColorR = r;
        _OnSkinColorChanged();
    }

    //Setter for skin color changed (green slider)
    public void OnSkinColorChanged_G(float g)
    {
        babyModel.SkinColorG = g;
        _OnSkinColorChanged();
    }

    //Setter for skin color changed (blue slider)
    public void OnSkinColorChanged_B(float b)
    {
        babyModel.SkinColorB = b;
        _OnSkinColorChanged();
    }

    // Setter for head mesh (the image contains the meshName reference)
    public void OnMeshChanged(int meshIndex)
    {
        _OnMeshChanged(meshIndex);
    }

    // UI Tooltip box - text is passed from the inspector
    public void OnUIElementPointerEnter(string text)
    {
        _OnToolTipAction(text);
    }

    // Clear the UI toolkit
    public void OnUIElementPointerExit()
    {
        _OnToolTipExitAction();
    }

    public void deleteSaveFile()
    {
        if (colonists.Count == 0)
        {
            File.Delete("colonists.json");
        }
    }

    public void OnColonistDied(GameClockEvent e, ICombatant c)
    {
        // Remove the dead before saving again
        colonists.Remove(c as BabyModel);
        // If colonists file is empty, destroy it?
        deleteSaveFile();
        deadColonists.Add(c as BabyModel);
        _OnSaveAction("colonists", deadColonists, babyModel, "deadColonists.json");
    }

    // Called on finalize creation menu
    public void AddNewColonist()
    {
        if(!CreationMenuController.validEntry || colonists.Count > MAX_COLONISTS)
        {
            return;
        }
        // Add event marker and to active colonists
        string bornEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 0);
        babyModel.eventMarkersMap.EventMarkersFeed.Add(bornEventAchievement, 1); // The base achievement is to be born

        colonists.Add(babyModel);
        // Also add to colonist registry permanent asset for UUIDs
        gameCharacterDatabase.colonistUUIDCount++;
        babyModel.UniqueColonistPersonnelID_ = gameCharacterDatabase.colonistUUIDCount;
        gameCharacterDatabase.colonistRegistry.Add(babyModel);
        _OnSaveAction("colonists", gameCharacterDatabase.colonistRegistry, babyModel, "colonistRegistry.json");
    }

    // The save method service for the client - FIXME this is for through the creation menu
    public void Save(bool checkMaxElements)
    {
        // Event to save the current baby template to a file
        if(!CreationMenuController.validEntry)
        {
            return;
        }
        if (colonists == null)
        {
            colonists = new List<BabyModel>();
        }
        // We don't check for max elements if saving dead colonists (for now)
        if (!checkMaxElements || colonists.Count <= MAX_COLONISTS) // The last one being added makes it equal to MAX_COLONISTS
        {
            // TODO add dead colonists unique ID too?
            //SaveToJSONFile(key, nbElements, savedObject, path, "Save successful");
            if (colonists.Count > 0)
            {
                _OnSaveAction("colonists", colonists, babyModel, "colonists.json");
            }
            if (deadColonists.Count > 0)
            {
                // Needs to load up the previous dead colonists first before rewriting
                _OnSaveAction("colonists", deadColonists, babyModel, "deadColonists.json");
            }
        }
        else
        {
            Debug.Log("Save game impossible :-(. Full capacity reached.");
        }
    }

    // Handle client requests
    public void OnServerReply(Enums.DataRequests requestPort)
    {
        switch(requestPort)
        {
            case Enums.DataRequests.LIVE_COLONISTS:
                _OnRequestColonistDataResponse(colonists, Enums.DataRequests.LIVE_COLONISTS);
                break;
            case Enums.DataRequests.DEAD_COLONISTS:
                _OnRequestColonistDataResponse(deadColonists, Enums.DataRequests.DEAD_COLONISTS);
                break;
            default:
                break;
        }
    }
}
