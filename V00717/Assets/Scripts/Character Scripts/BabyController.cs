using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// This class contains the baby model and controls its attributes
// TODO refactor monobehaviour specific routines into a seperate class, in order to add a normal constructor
public class BabyController
{
    // The Baby Model component
    private BabyModel babyModel = null;
    public BabyModel BabyModel { get { return babyModel; } set { babyModel = value; } }

    // The colonists 
    public List<BabyModel> colonists = null;
    public List<BabyModel> deadColonists = null;

    // The permanent assets database
    public GameCharacterDatabase gameCharacterDatabase = null;

    // The max n of colonists (temporary n)
    public static int MAX_COLONISTS = 4;

    // SERVER REQUESTS
    public delegate void RequestColonistDataResponse(List<BabyModel> colonists, Enums.DataRequests request);
    public static event RequestColonistDataResponse _OnRequestColonistDataResponse;

    public BabyController(ref List<BabyModel> colonists, ref List<BabyModel> deadColonists, GameCharacterDatabase gameCharacterDatabase, ref BabyModel babyModel)
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += MallocNewCharacter;
        DashboardOSController._OnRequestColonistData += OnServerReply;
        CharacterCreationView._OnSexChanged += OnSexChanged;
        SaveSystem._SuccessfulSaveAction += ResetCharacterCache;
        this.babyModel = babyModel;
        this.babyModel = new BabyModel();
        this.colonists = colonists;
        this.deadColonists = deadColonists;
        this.gameCharacterDatabase = gameCharacterDatabase;
        LoadGameCharacters();
    }

    ~BabyController()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= MallocNewCharacter;
        DashboardOSController._OnRequestColonistData -= OnServerReply;
        CharacterCreationView._OnSexChanged -= OnSexChanged;
        SaveSystem._SuccessfulSaveAction -= ResetCharacterCache;
    }

    public void LoadGameCharacters()
    {
        // First load game if needed (TODO validate contents too, can have bad format and exist)
        if (SaveSystem.SaveFileExists("colonists.json"))
        {
            LoadCharactersFromJSONFile(ref colonists, "colonists.json", true);
        }
        if (SaveSystem.SaveFileExists("deadColonists.json"))
        {
            LoadCharactersFromJSONFile(ref deadColonists, "deadColonists.json", false);
        }
        if (SaveSystem.SaveFileExists("colonistRegistry.json"))
        {
            LoadCharactersFromJSONFile(ref gameCharacterDatabase.colonistRegistry, "colonistRegistry.json", false);
            // Update the UUID count by using the length of the registry list
            gameCharacterDatabase.colonistUUIDCount = gameCharacterDatabase.colonistRegistry.Count;
        }
    }

    public void ResetCharacterCache()
    {
        MallocNewCharacter();
    }

    // Creates an array of baby models from the json text read and deserialized from path
    public void LoadCharactersFromJSONFile(ref List<BabyModel> colonists, string path, bool deleteIfEmpty)
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
            return;
        }
        for(int i = 0; i < deserializedObject.colonists.Length; i++)
        {
            colonists.Add(deserializedObject.colonists[i]);
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

    public void OnSexChanged(string sex)
    {
        babyModel.Sex = sex;
    }

    //Setter for skin color changed (red slider)
    public void OnSkinColorChanged_R(float r)
    {
        babyModel.SkinColorR = r;
    }

    //Setter for skin color changed (green slider)
    public void OnSkinColorChanged_G(float g)
    {
        babyModel.SkinColorG = g;
    }

    //Setter for skin color changed (blue slider)
    public void OnSkinColorChanged_B(float b)
    {
        babyModel.SkinColorB = b;
    }

    // Called on finalize creation menu
    public void CreateNewColonist()
    {
        if (!CreationMenuController.validEntry || colonists.Count > MAX_COLONISTS)
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
