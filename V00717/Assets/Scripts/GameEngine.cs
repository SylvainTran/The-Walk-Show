using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class State
{
    public GameEngine gameEngine;
    public State(GameEngine g)
    {
        gameEngine = g;
        gameEngine.StringToLog = $"Entering {this.GetType()}.";
        gameEngine.Notify();
    }
    ~State()
    {
        gameEngine.StringToLog = $"Leaving state {this.GetType()}."; 
        gameEngine.Notify();
    }
    public virtual void NewGame()
    {
        Debug.LogWarning("Invalid state: Cannot 'New Game' from this state.");
    }
    public virtual void LoadGame()
    {
        Debug.LogWarning("Invalid state: Cannot 'Load Game' from this state.");
    }
    public virtual bool CheckShipState()
    {
        Debug.LogWarning("Invalid state: Cannot 'CheckShipState' from this state.");
        return false;
    }
    public virtual void CheckCrewState()
    {
        Debug.LogWarning("Invalid state: Cannot 'CheckCrewState' from this state.");
    }
    public virtual void CheckCommunityState()
    {
        Debug.LogWarning("Invalid state: Cannot 'CheckCommunityState' from this state.");
    }
    public virtual void CheckStoryState()
    {
        Debug.LogWarning("Invalid state: Cannot 'CheckStoryState' from this state.");
    }
    public virtual void RemoteViewControl()
    {

    }

    public virtual void Scheduling()
    {

    }

    public virtual void Community()
    {

    }

    public virtual void Lounge()
    {

    }
    public void QuitGame() {
        gameEngine.SceneController.QuitGame();
    }
}
public class MainMenuState : State
{
    public MainMenuState(GameEngine g) : base(g)
    {

    }
    public override void NewGame()
    {
        gameEngine.SceneController.NewGame();
        gameEngine.TransitionToState(new StartGameState(gameEngine));
        gameEngine.TransitionToState(new GameLoadedState(gameEngine));
        gameEngine.TransitionToState(new NewDayState(gameEngine));
        gameEngine.TransitionToState(new StreamerRoomState(gameEngine));
    }
    public override void LoadGame()
    {
        gameEngine.SceneController.LoadGame();
    }
}
public class LoadingGameState : State
{
    public LoadingGameState(GameEngine g) : base(g)
    {

    }
}
public class LoadingStoryState : State
{
    public LoadingStoryState(GameEngine g) : base(g)
    {

    }
}
public class LoadingCharactersState : State
{
    public LoadingCharactersState(GameEngine g) : base(g)
    {

    }
}
public class StartGameState : State
{
    public StartGameState(GameEngine g) : base(g)
    {
        // Add first main cast(s) in the story
        gameEngine.Casting.entities.Add(gameEngine.AlfredoPeristasisGameObject);
        gameEngine.Casting.entities.Add(gameEngine.YuriArtyomGameObject);
    }
}
public class GameLoadedState : State
{
    public GameLoadedState(GameEngine g) : base(g)
    {

    }
}
public class NewDayState : State
{
    private bool continueGame = false;
    public NewDayState(GameEngine g) : base(g)
    {
        ValidateStates();
    }
    public void ValidateStates()
    {
        continueGame = CheckShipState();
        if(continueGame)
        {
            CheckCrewState();
            CheckCommunityState();
            CheckStoryState();
        }
    }
    public override bool CheckShipState()
    {
        Debug.Log("Checked ship state!");
        return true;
    }
    public override void CheckCrewState()
    {
        Debug.Log("Checked crew state!");
    }
    public override void CheckCommunityState()
    {
        Debug.Log("Checked community state!");
    }
    public override void CheckStoryState()
    {
        Debug.Log("Checked story state!");
    }
}
public class EndDayState : State
{
    public EndDayState(GameEngine g) : base(g)
    {

    }
}
public class WinState : State
{
    public WinState(GameEngine g) : base(g)
    {

    }
}
public class GameOverState : State
{
    public GameOverState(GameEngine g) : base(g)
    {

    }
}
public class StreamerRoomState : State
{
    public StreamerRoomState(GameEngine g) : base(g)
    {
    }

    public override void RemoteViewControl()
    {
        Debug.Log("[GameEngine] RemoteViewControl enabled.");
        foreach (GameObject castingShiftPrefabInGame in gameEngine.castingShiftPrefabsInGame)
        {
            Debug.Log($"Active cast today? : {castingShiftPrefabInGame.GetComponent<CastingShift>().GetActiveIsOn()}");
            Debug.Log($"Cast in room : {castingShiftPrefabInGame.GetComponent<CastingShift>().GetRoleDropDownValue()}");
            Debug.Log($"Begin time : {castingShiftPrefabInGame.GetComponent<CastingShift>().GetBeginTime()}");
            Debug.Log($"End time : {castingShiftPrefabInGame.GetComponent<CastingShift>().GetEndTime()}");
        }
    }

    public override void Scheduling()
    {
        Debug.Log("[GameEngine] Scheduling window enabled.");
        // MAIN PAGE
        // Get the main casting currently active (depending on the story, and live/dead)
        //Entities<GameObject> casting = gameEngine.Casting; ;
        //foreach(GameObject go in casting.entities) {
        //    Debug.Log($"Active Cast: {go.gameObject.GetComponent<CharacterModel>().name}");
        //}
        string[] castingDummyData = { "Alan Pent", "Alfredo Peristasis", "Derrick Groot", "Erina Cliffhills", "Myla Keys", "Sabrina Brielstursson", "Takatsu Honda", "Yuri Artyom", "Zedina Werty"};
        string[] activeCasting = { "Alfredo Peristasis", "Zedina Werty" };
        foreach (string go in activeCasting)
        {
            Debug.Log($"Active Cast: {go}");
        }
        // Display as a list, Press [1-4] keys to change corresponding cast's schedule
        foreach (GameObject cast in gameEngine.Casting.entities)
        {
            GameObject newShift = GameObject.Instantiate(gameEngine.CastingShiftPrefab);
            newShift.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            newShift.transform.SetParent(GameObject.FindObjectOfType<SchedulingWindowContent>().transform);
            newShift.SetActive(true);
            newShift.GetComponent<CastingShift>().BindCharacter(cast);
            gameEngine.castingShiftPrefabsInGame.Add(newShift);
            Debug.Log($"Instantiated a new casting shift for cast: {cast.GetComponent<CharacterModel>().name}");
        }

        // var boxTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("UXML/shift.uxml");
        // SUB PAGE
        // Display time schedule for the selected cast, for current date/day in-game
        // One time schedule shift = minimum 3 hours, at least one shift a day, and max 1h breaks between shifts and max 1h30 break total
        // Display shift hours that can be modified : enter start, end time from dropdown list
        // Display categorical job options from dropdown list (will increase character's skills) 
        // CREATE an Order/Shift Object (self-contained, can be deferred until main game loop executes it) from this data
        // Store it in the game engine list of shifts for the next in-game day
        // Validate at each step and confirm, then back to MAIN PAGE
    }

    public override void Community()
    {

    }

    public override void Lounge()
    {

    }
}
public class CheckShipState : State
{
    public CheckShipState(GameEngine g) : base(g)
    {

    }
}
public class CheckCrewState : State
{
    public CheckCrewState(GameEngine g) : base(g)
    {

    }
}
public class CheckCommunityState : State
{
    public CheckCommunityState(GameEngine g) : base(g)
    {

    }
}
public class CheckStoryState : State
{
    public CheckStoryState(GameEngine g) : base(g)
    {

    }
}
public class Entities<T>
{
    public List<T> entities;

    public Entities() {
        entities = new List<T>();
    }
}
public class GameEngine : MonoBehaviour, ISubject
{
    private static GameEngine Instance;
    private static State currentState;
    private List<Observer> observers;
    private static List<LogObserver> logObservers;
    private string stringToLog;
    private SceneController sceneController;

    //  UI Prefabs
    public static GameObject SchedulingWindow;
    public GameObject SchedulingWindowScrollview;
    public GameObject CastingShiftPrefab;
    //  Instantiated casting shift prefabs in the scheduling window
    public List<GameObject> castingShiftPrefabsInGame;

    /// <summary>
    /// The active casting (the active story game character(s)).
    /// </summary>
    private Entities<GameObject> casting;
    [SerializeField] public GameObject AlfredoPeristasisGameObject;
    [SerializeField] public GameObject YuriArtyomGameObject;
    [SerializeField] public GameObject ZedinaWertyGameObject;
    [SerializeField] public GameObject ErinaCliffHillsGameObject;
    [SerializeField] public GameObject DerrickGrootGameObject;
    [SerializeField] public GameObject MylaKeysGameObject;
    [SerializeField] public GameObject SabrinaBrielsturssonGameObject;
    [SerializeField] public GameObject TakashiHondaGameObject;
    [SerializeField] public GameObject AlanPentGameObject;

    public List<LogObserver> LogObserver { get => logObservers; set => logObservers = value; }
    public List<Observer> Observers { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string StringToLog { get => stringToLog; set => stringToLog = value; }
    public static State CurrentState { get => currentState; set => currentState = value; }
    public SceneController SceneController { get => sceneController; set => sceneController = value; }
    public Entities<GameObject> Casting { get => casting;   set => casting = value; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            Instance = this;
            observers = new List<Observer>();
            logObservers = new List<LogObserver>();
            sceneController = new SceneController();
            casting = new Entities<GameObject>();
            castingShiftPrefabsInGame = new List<GameObject>();
            DontDestroyOnLoad(this.gameObject);
            TransitionToState(new MainMenuState(this));
        }
    }
    public void TransitionToState(State newState)
    {
        currentState = null;
        currentState = newState;
        Debug.Log($"Current state: {currentState}");
    }
    public static GameEngine GetGameEngine()
    {
        return Instance;
    }
    public void AddLogObserver(LogObserver o)
    {
        logObservers.Add(o);
    }
    public void Attach(Observer o)
    {
        observers.Add(o);
    }
    public void Detach(Observer o)
    {
        observers.Remove(o);
    }
    public virtual void Notify()
    {
        foreach (Observer o in observers)
        {
            o.UpdateState();
        }
    }
    public void NewGame()
    {
        currentState.NewGame();
    }
    public void LoadGame()
    {
        currentState.LoadGame();
    }
    public void RemoteViewControl()
    {
        currentState.RemoteViewControl();
    }

    public void Scheduling()
    {
        currentState.Scheduling();
    }

    public void Community()
    {
        currentState.Community();
    }

    public void Lounge()
    {
        currentState.Lounge();
    }
}
