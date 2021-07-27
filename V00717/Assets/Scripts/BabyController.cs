using UnityEngine;
using System;
using Newtonsoft.Json;

// This class contains the baby model and controls its attributes
public class BabyController : MonoBehaviour, ISaveableComponent
{
    // The Baby Model component (cached through inspector)
    [SerializeField] private BabyModel babyModel;
    public BabyModel BabyModel { get { return babyModel; } set { babyModel = value; } }

    // Inner node class for JSON serialization (contains array of colonists)
    public Colonists _colonists = null;

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

    // Notify view of head mesh changed
    public delegate void HeadMeshChanged();
    public static event HeadMeshChanged _OnHeadMeshChanged; // listened to by View.cs

    // Notify view of torso mesh changed
    public delegate void TorsoMeshChanged();
    public static event TorsoMeshChanged _OnTorsoMeshChanged; // listened to by View.cs

    // Save to file event
    public delegate void SaveAction(string key, Colonists c, BabyModel b, string path);
    public static event SaveAction _OnSaveAction;

    // Attach method functions
    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += MallocNewCharacter;
    }

    // Dettach method functions
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= MallocNewCharacter;
    }

    // Creates an array of baby models from the json text read and deserialized from path
    public BabyModel[] LoadCharactersFromJSONFile(string path)
    {
        // Generate new characters based on JSON file
        string text = System.IO.File.ReadAllText(path);
        SaveSystem.SavedArrayObject stuff = JsonConvert.DeserializeObject<SaveSystem.SavedArrayObject>(text);
        BabyModel[] _colonists = new BabyModel[4];
        for(int i = 0; i < stuff.colonists.Length; i++)
        {
            _colonists[i] = stuff.colonists[i];
        }
        return _colonists;
    }

    // Loads game data from JSON file
    private void Preload()
    {
        // TODO deal with edge case where the file exists but its contents are invalid
        _colonists.colonists = LoadCharactersFromJSONFile("colonists.json");
    }

    // Malloc baby model and the colonists array
    private void Awake()
    {
        MallocNewCharacter();
        _colonists = new Colonists();
    }

    private void Start()
    {
        // First load game if needed (TODO validate contents too, can have bad format and exist)
        if (SaveSystem.SaveFileExists("colonists.json"))
        {
            Preload();
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
        babyModel.Name = name;
        Debug.Log($"And so {babyModel.Name} was given his name.");
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
    public void OnHeadMeshChanged(string meshName)
    {
        babyModel.ActiveHeadName = meshName;
        _OnHeadMeshChanged();
    }

    // Setter for torso mesh (the image contains the meshName reference)
    public void OnTorsoMeshChanged(string meshName)
    {
        babyModel.ActiveTorsoName = meshName;
        _OnTorsoMeshChanged();
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

    // This inner class exists so that we can serialize the object
    // with its contained array of colonists to JSON (we can't do it directly).
    [Serializable]
    public class Colonists
    {
        // The array to serialize
        [SerializeField] public BabyModel[] colonists;
        // The max n of colonists (temporary n)
        [NonSerialized] public int MAX_COLONISTS = 4;

        // Empty constructor, inits colonists array
        public Colonists()
        {
            this.colonists = new BabyModel[MAX_COLONISTS];
        }

        // Getter for the N max colonists
        public int GetMaxColonists()
        {
            return MAX_COLONISTS;
        }
    }

    public void Save()
    {
        // Event to save the current baby template to a file
        BabyModel.UniqueColonistPersonnelID++;
        _OnSaveAction("colonists", _colonists, babyModel, "colonists.json");
    }
}
