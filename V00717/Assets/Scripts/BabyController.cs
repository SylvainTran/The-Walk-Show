using System.IO;
using System.Threading.Tasks;
using UnityEngine;

// This class contains the baby model and controls its attributes
public class BabyController : MonoBehaviour
{
    // The Baby Model component
    private BabyModel babyModel;
    public BabyModel BabyModel { get { return babyModel; } set { babyModel = value; } }

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
    public static event AdultHeightChangeAction _OnAdultHeightChanged; // listened to by LabelController.cs

    // Notify view of skin color changed
    public delegate void SkinColorChanged();
    public static event SkinColorChanged _OnSkinColorChanged; // listened to by View.cs

    // Notify view of head mesh changed
    public delegate void HeadMeshChanged();
    public static event HeadMeshChanged _OnHeadMeshChanged; // listened to by View.cs

    // Notify view of torso mesh changed
    public delegate void TorsoMeshChanged();
    public static event TorsoMeshChanged _OnTorsoMeshChanged; // listened to by View.cs

    private void OnEnable()
    {
        PageController._OnSaveToFile += Save;
    }

    // Cache the BabyModel component
    public void Awake()
    {
        babyModel = GetComponent<BabyModel>();
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
        Debug.Log($"Baby's adult height was changed to: {adultHeight}");
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

    // Save to JSON (async) -> TODO refactor out
    public async Task Save()
    {
        string json = JsonUtility.ToJson(babyModel);
        //BabyModel.UniqueColonistPersonnelID

        using (StreamWriter outputFile = new StreamWriter("colonists.txt", true))
        {
            await outputFile.WriteAsync(json);
            Debug.Log("New colonist data saved successfully.");
        }
    }
}
