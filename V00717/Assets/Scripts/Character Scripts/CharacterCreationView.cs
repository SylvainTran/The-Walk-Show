using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

// Deals with view render changes
public class CharacterCreationView : MonoBehaviour
{   
    public CreationController CreationController;
    public GameObject characterModelPrefab;
    public GameObject newCharacterModelInstance;

    // The Baby Model GOs to render/change mesh, and the scriptable object to pull data from
    private Renderer BabyModelHeadRenderer;
    private Renderer BabyModelTorsoRenderer;
    public MeshFilter BabyModelHeadMeshFilter;
    public MeshFilter BabyModelTorsoMeshFilter;    
    public GameController GameController;
    // Scriptable object with assets
    public ModelAssets ModelAssets;

    // Unique colonist personnel ID in colonist creation screen
    public TMP_Text uniqueColonistPersonnelID_CC;

    // Delegate for changing sex
    public delegate void SexChangeAction(string sex);
    public static event SexChangeAction _OnSexChanged; // listened to by SoundController.cs

    public static Vector3 characterModelPrefabCoords = Vector3.zero;
    public static Vector3 characterModelPrefabInstanceCoords = Vector3.zero;

    // Attach the event listeners
    public void OnEnable()
    {
        SaveSystem._SuccessfulSaveAction += UpdateColonistUUIDText;
    }

    // Detach the event listeners
    public void OnDisable()
    {
        SaveSystem._SuccessfulSaveAction -= UpdateColonistUUIDText;
    }

    // Updates the colonist uuid text on start
    private void Start()
    {
        // Create a new character template mesh
        GameController = GameObject.FindObjectOfType<GameController>();
        CreationController = GameController.CreationController;
        characterModelPrefabCoords = new Vector3(-2.14f, 0.75f, 2.9f);
        characterModelPrefabInstanceCoords = new Vector3(2.14f, 0.75f, characterModelPrefabCoords.z += 50.0f);

        BabyModelHeadRenderer = newCharacterModelInstance.GetComponent<Renderer>();
        BabyModelTorsoRenderer = newCharacterModelInstance.GetComponent<Renderer>();

        BabyModelHeadMeshFilter = newCharacterModelInstance.GetComponent<MeshFilter>();
        BabyModelTorsoMeshFilter = newCharacterModelInstance.GetComponent<MeshFilter>();
        // Setup its camera and render texture
        UpdateColonistUUIDText();
    }

    // Setter for new colonist nickname
    public void OnNickNameChanged(string nickName)
    {
        newCharacterModelInstance.GetComponent<CharacterModel>().NickName = nickName;
        GetComponent<AuditionEditor>().SetNameChoice(nickName);
        Debug.Log($"And so {nickName} was given his nickname.");
    }

    //Setter for baby's sex via Unity's built-in event system.
    public void OnSexChanged(string sex)
    {
        if(newCharacterModelInstance == null)
        {
            return;            
        }

        // Call listeners - Sound
        _OnSexChanged(sex);
        newCharacterModelInstance.GetComponent<CharacterModel>().Sex = sex;
        Debug.Log($"Character's sex was changed to: {sex}");
    }

    public void OnSkinColorChangedR(float value)
    {
        if(newCharacterModelInstance == null)
        {
            return;            
        }

        newCharacterModelInstance.GetComponent<CharacterModel>().SkinColorR = value;
        UpdateSkinColor();
    }

    public void OnSkinColorChangedG(float value)
    {
        if(newCharacterModelInstance == null)
        {
            return;            
        }

        newCharacterModelInstance.GetComponent<CharacterModel>().SkinColorG = value;
        UpdateSkinColor();
    }

    public void OnSkinColorChangedB(float value)
    {
        if(newCharacterModelInstance == null)
        {
            return;            
        }

        newCharacterModelInstance.GetComponent<CharacterModel>().SkinColorB = value;
        UpdateSkinColor();
    }

    // Updates the colonist uuid text in identification tab
    public void UpdateColonistUUIDText()
    {
        if(uniqueColonistPersonnelID_CC)
        {
            uniqueColonistPersonnelID_CC.SetText($"Unique Actor Union ID: {CharacterModelObject.uniqueColonistPersonnelID}");
        }
    }

    // Procedure to update the skin color of BabyModelGO's game object's children
    // Gets the renderer of the go's children and updates its materials with the new rgb values
    public void UpdateSkinColor()
    {
        CharacterModel character = newCharacterModelInstance.GetComponent<CharacterModel>();
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.SetColor("_Color", new Color(character.SkinColorR, character.SkinColorG, character.SkinColorB));
        BabyModelHeadRenderer.material = newMat;
        BabyModelTorsoRenderer.material = newMat;
    }

    // Procedure to update the skin color of BabyModelGO's game object's children
    // Gets the renderer of the go's children and updates its materials with the new rgb values
    public void UpdateSkinColor(GameObject go)
    {
        Renderer[] sharedMeshList = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer m in sharedMeshList)
        {
            m.material.SetColor("_Color", Color.gray);
        }
    }

    // Views update itself concerning head/torso changes in UI (type == 0-4 : head mesh, type 4-8 : torso mesh)
    // Changes the mesh of the baby model by accessing its mesh filter setter and mesh property
    public void UpdateMesh(int meshIndex)
    {
        int capacity = ModelAssets.heads.Capacity;
        // Capacity (4 for now) is the last head mesh index, so every index larger to that is a torso
        List<MeshFilter> meshList = meshIndex < capacity ? ModelAssets.heads : ModelAssets.torsos;
        MeshFilter? meshFilter = meshList[meshIndex % capacity];
        Mesh newMesh = null;
        
        if(meshFilter != null)
        {
            newMesh = meshFilter.sharedMesh;
        }
        else
        {
            return;
        }
        // Transform Child 1 are torsos, child 0 are heads
        if (meshIndex < capacity)
        {
            BabyModelHeadMeshFilter.mesh = newMesh;
        }
        else
        {
            BabyModelTorsoMeshFilter.mesh = newMesh;
        }
    }

    // Called on finalize creation menu
    public void AddNewColonistToRegistry()
    {
        if (GameController.Colonists.Count >= CreationController.MAX_COLONISTS)
        {
            Debug.Log("max characters already");
            GameController.seasonController.EndAuditions();
            GameController.auditionStatus.enabled = true;
            return;
        }
        if(CreateNewColonist())
        {
            GameController.Save();
            GetComponent<CreationMenuController>().DestroyEditor();

            if(GameController.Colonists.Count < CreationController.MAX_COLONISTS)
            {
                GameController.StartAuditionsAfterDelay(1);
            }
        }
    }

    // Called on finalize creation menu
    public bool CreateNewColonist()
    {
        //if (!GetComponent<CreationMenuController>().validEntry)
        //{
        //    Debug.Log("Invalid entry");
        //    return false;
        //}
        // Create a characterModel component to attach to its mesh game object
        // TODO update UUID in a more reliable new way
        CharacterModelObject.uniqueColonistPersonnelID++;
        CreateNewCharacterMesh();
        GameController.ValidateCharactersState();
        return true;
    }

    
    public void CreateNewCharacterMesh()
    {
        // if(!GetComponent<CreationMenuController>().validEntry)
        // {
        //     return;
        // }
        CreationController creationController = GameController.CreationController;

        // Set the new Material runner games character to the last track position (set from live game character count)
        int trackLanePosition = creationController.FindAvailableCameraLane();

        GameObject newCharacterMesh = newCharacterModelInstance;
        try
        {
            CharacterModel newCharacterModel = newCharacterMesh.GetComponent<CharacterModel>();
            newCharacterMesh.transform.position = creationController.TrackLanePositions[trackLanePosition];
            newCharacterMesh.gameObject.name = newCharacterModel.NickName;
            newCharacterModel.InitEventsMarkersFeed(); // Inits events feed and last event but they're null at this stage
            newCharacterModel.UniqueColonistPersonnelID_ = CharacterModelObject.uniqueColonistPersonnelID; // Sets the uuid field, not the static one as it wont be serialized
            // Update UUID for application length - then needs to be saved to file
            GameController.Colonists.Add(newCharacterMesh);

            // TODO Set its mesh to the players' choices using the character model component        
            creationController.SetTrackLanePosition(trackLanePosition, newCharacterMesh.transform);
        }
        catch (ArgumentNullException ane)
        {
            Debug.Log(ane.Message);
            Debug.LogError("Error: No prefab model for characters loaded.");
        }
        catch (ArgumentException ae)
        {
            Debug.LogError(ae.Message);
        }
    }
}
