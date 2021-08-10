using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class GameController : MonoBehaviour
{
    BabyModel babyModel = null;
    BabyController babyController = null;
    GameClockEventController gameClockEventController = null;

    [Header("The trigger chance for any time event to occur.")]
    [Tooltip("Higher values mean less chance of a triggered event happening.")]
    public float triggerChance = 5.0f;

    [Header("The interval rate in (s) at which events will tick for any character.")]
    [Tooltip("Faster rates will accelerate the game characters' deaths greatly.")]
    public float eventIntervalRate = 5.0f;

    // Save to file event
    public delegate void SaveAction(string key, List<BabyModel> c, BabyModel b, string path);
    public static event SaveAction _OnSaveAction;

    public GameCharacterDatabase gameCharacterDatabase;
    public ChatDatabase chatDatabaseSO;
    public List<BabyModel> colonists = null;
    public List<BabyModel> deadColonists = null;

    private void OnEnable()
    {
        GameClockEvent._OnColonistIsDead += OnColonistDied;
        TimeController._OnUpdateEventClock += OnEventClockUpdate;
    }

    private void OnDisable()
    {
        GameClockEvent._OnColonistIsDead -= OnColonistDied;
        TimeController._OnUpdateEventClock -= OnEventClockUpdate;
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Create a game character database if it doesn't exist (failsafe)
        // ScriptableObject gcd = (ScriptableObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/GameCharacterDatabase.asset", typeof(ScriptableObject));
        gameCharacterDatabase = (GameCharacterDatabase)AssetDatabase.LoadAssetAtPath("Assets/MyResources/GameCharacterDatabase.asset", typeof(GameCharacterDatabase));
        if (gameCharacterDatabase == null)
        {
            gameCharacterDatabase = ScriptableObject.CreateInstance<GameCharacterDatabase>();
            AssetDatabase.CreateAsset(gameCharacterDatabase, $"Assets/MyResources/{gameCharacterDatabase.name}.asset");
        }
        // Load chat database SO and initialize main controllers
        chatDatabaseSO = (ChatDatabase)AssetDatabase.LoadAssetAtPath("Assets/MyResources/ChatDatabase.asset", typeof(ChatDatabase));
        if (chatDatabaseSO == null)
        {
            chatDatabaseSO = ScriptableObject.CreateInstance<ChatDatabase>();
            AssetDatabase.CreateAsset(chatDatabaseSO, $"Assets/MyResources/{chatDatabaseSO.name}.asset");
        }
        // Load from saved file if needed
        if (chatDatabaseSO.REGRET_THEME.Length == 0)
        {
            LoadChatDatabaseJSON($"Assets/chatDatabase.json");
        }
        // Objects are passed to controllers by reference to keep the same object updated here at the same time
        babyController = new BabyController(ref colonists, ref deadColonists, gameCharacterDatabase, ref babyModel);
        gameClockEventController = new GameClockEventController(ref colonists, triggerChance);
        CharacterCreationView.SetCharacterViewModel(ref babyController);
    }

    public void OnEventClockUpdate()
    {
        gameClockEventController.OnEventClockUpdate();
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
        deleteSaveFile();
        deadColonists.Add(c as BabyModel);
        //UpdateGameCharacterRegistry();
        _OnSaveAction("colonists", deadColonists, babyController.BabyModel, "deadColonists.json");
    }

    // Called on finalize creation menu
    public void AddNewColonistToRegistry()
    {
        babyController.CreateNewColonist();
        _OnSaveAction("colonists", gameCharacterDatabase.colonistRegistry, babyController.BabyModel, "colonistRegistry.json");
    }

    // The save method service for the client - FIXME this is for through the creation menu
    public void Save(bool checkMaxElements)
    {
        // Event to save the current baby template to a file
        if (!CreationMenuController.validEntry || babyController.colonists.Count > BabyController.MAX_COLONISTS)
        {
            return;
        }
        int count = babyController.colonists.Count;
        // We don't check for max elements if saving dead colonists (for now)
        if (!checkMaxElements || count <= BabyController.MAX_COLONISTS) // The last one being added makes it equal to MAX_COLONISTS
        {
            // TODO add dead colonists unique ID too?
            //SaveToJSONFile(key, nbElements, savedObject, path, "Save successful");
            if (colonists.Count > 0)
            {
                _OnSaveAction("colonists", colonists, babyController.BabyModel, "colonists.json");
            }
            if (deadColonists.Count > 0)
            {
                // Needs to load up the previous dead colonists first before rewriting
                _OnSaveAction("colonists", deadColonists, babyController.BabyModel, "deadColonists.json");
            }
        }
        else
        {
            Debug.Log("Save game impossible :-(. Full capacity reached.");
        }
    }

    public void UpdateGameCharacterRegistry()
    {
        foreach (BabyModel dc in deadColonists)
        {
            BabyModel target = gameCharacterDatabase.colonistRegistry.Find(x => x.UniqueColonistPersonnelID_ == dc.UniqueColonistPersonnelID_);
            target.eventMarkersMap = dc.eventMarkersMap;
            target.SetLastEvent(dc.LastEvent);
        }
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
    public void LoadChatDatabaseJSON(string path)
    {
        string text = System.IO.File.ReadAllText(path);
        JSONDatabase deserializedObject = JsonUtility.FromJson<JSONDatabase>(text);
        chatDatabaseSO.STRESS_THEME = deserializedObject.STRESS_THEME;
        chatDatabaseSO.REGRET_THEME = deserializedObject.REGRET_THEME;
        chatDatabaseSO.REALIZATION_THEME = deserializedObject.REALIZATION_THEME;
        chatDatabaseSO.DENIAL_THEME = deserializedObject.DENIAL_THEME;
        chatDatabaseSO.GUILT_THEME = deserializedObject.GUILT_THEME;
        chatDatabaseSO.FEAR_THEME = deserializedObject.FEAR_THEME;
    }

    /// <summary>
    /// The camera lanes to watch the characters
    /// </summary>
    public void UpdateCameraLanes()
    {

    }

    // DEBUG MODE: Nothing here
    // Update character registry
    //private void OnApplicationQuit()
    //{
    //    Debug.Log("Saving game registry before exiting...");
    //    foreach (BabyModel dc in deadColonists)
    //    {
    //        BabyModel target = gameCharacterDatabase.colonistRegistry.Find(x => x.UniqueColonistPersonnelID_ == dc.UniqueColonistPersonnelID_);
    //        target.eventMarkersMap = dc.eventMarkersMap;
    //        target.SetLastEvent(dc.LastEvent);
    //    }
    //    if(colonists != null && colonists.Count > 0)
    //    {
    //        _OnSaveAction("colonists", colonists, babyController.BabyModel, "colonists.json");
    //    }
    //    if(deadColonists != null && deadColonists.Count > 0)
    //    {
    //        _OnSaveAction("colonists", deadColonists, babyController.BabyModel, "deadColonists.json");
    //    }
    //    if(gameCharacterDatabase != null)
    //    {
    //        _OnSaveAction("colonists", gameCharacterDatabase.colonistRegistry, babyController.BabyModel, "colonistRegistry.json");
    //    }
    //    // Save any created assets to disk - Necessary?
    //    AssetDatabase.SaveAssets();
    //    Debug.Log("Application ending after " + Time.time + " seconds");
    //}
}
