using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

// Deals with view render changes
public class CharacterCreationView : UIView
{
    // The Baby Model GOs to render/change mesh, and the scriptable object to pull data from
    public Renderer BabyModelHeadRenderer;
    public Renderer BabyModelTorsoRenderer;
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

    // Attach the event listeners
    public new void OnEnable()
    {
        SaveSystem._SuccessfulSaveAction += UpdateColonistUUIDText;
    }

    // Detach the event listeners
    public new void OnDisable()
    {
        SaveSystem._SuccessfulSaveAction -= UpdateColonistUUIDText;
    }

    // Updates the colonist uuid text on start
    private void Start()
    {
        UpdateColonistUUIDText();
    }

    // Setter for new colonist nickname
    public void OnNickNameChanged(string nickName)
    {
        GameController.CreationController.OnNickNameChanged(nickName);
        Debug.Log($"And so {nickName} was given his nickname.");
    }

    //Setter for baby's sex via Unity's built-in event system.
    public void OnSexChanged(string sex)
    {
        // Call listeners - Sound
        _OnSexChanged(sex);
        Debug.Log($"Baby's sex was changed to: {sex}");
    }

    public void OnSkinColorChangedR(float value)
    {
        GameController.CreationController.OnSkinColorChanged_R(value);
        UpdateSkinColor();
    }

    public void OnSkinColorChangedG(float value)
    {
        GameController.CreationController.OnSkinColorChanged_G(value);
        UpdateSkinColor();
    }

    public void OnSkinColorChangedB(float value)
    {
        GameController.CreationController.OnSkinColorChanged_B(value);
        UpdateSkinColor();
    }

    // Updates the colonist uuid text in identification tab
    public void UpdateColonistUUIDText()
    {
        uniqueColonistPersonnelID_CC.SetText($"Unique Colonist Personnel ID: {CharacterModelObject.uniqueColonistPersonnelID}");
    }

    // Procedure to update the skin color of BabyModelGO's game object's children
    // Gets the renderer of the go's children and updates its materials with the new rgb values
    public void UpdateSkinColor()
    {
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.SetColor("_Color", new Color(GameController.CharacterModel.SkinColorR, GameController.CharacterModel.SkinColorG, GameController.CharacterModel.SkinColorB));
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
}
