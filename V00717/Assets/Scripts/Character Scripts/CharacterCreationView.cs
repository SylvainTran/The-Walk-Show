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

    // The following are cached from inspector
    // Tool tip text to update
    public TMP_Text tooltipGO;
    public Image ecnb = null; 
    public TMP_Text text = null;
    // The price tip
    public TMP_Text priceTip;

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
        // TODO UUID new way to get it from files?
        uniqueColonistPersonnelID_CC.SetText($"Unique Colonist Personnel ID:");
    }

    // Floating descending text animation
    public void TriggerTextAnimation(float value)
    {
        // Re-enable and lerp the image component's alpha values from 0-1 (in), then 1-0 (out)
        ecnb.enabled = true;
        text.enabled = true;
        ecnb.canvasRenderer.SetAlpha(0f);
        text.canvasRenderer.SetAlpha(0f);
        ecnb.CrossFadeAlpha(1f, 3f, false);
        text.CrossFadeAlpha(1f, 3f, false);
        StartCoroutine(FadeOutImage(4.5f, 0f, 3f, 3f, ecnb, text));
    }

    // Fade out the image and its child text components after a certain delay
    private IEnumerator FadeOutImage(float delay, float target, float fadeIn, float fadeOut, Image image, TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        image.CrossFadeAlpha(target, fadeIn, false);
        text.CrossFadeAlpha(target, fadeOut, false);
        // Disable the image and text (separate behaviour from canvas renderer)
        StartCoroutine(DisableButtonImage(fadeOut, ecnb, text));
     }

    // Disable the image and its child text components
    private IEnumerator DisableButtonImage(float delay, Image image, TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        image.enabled = false;
        text.enabled = false;
    }

    // Update tool tip box content on pointer enter some UI elements
    public void UpdateToolTip(string text)
    {
        tooltipGO.SetText(text);
    }

    // Clear the tool tip canvas's TMP_Text component
    public void ClearToolTip()
    {
        tooltipGO.SetText("");
    }

    // Update tool tip box content on pointer enter some UI elements
    public void UpdatePriceTip(string text)
    {
        priceTip.SetText(text);
    }

    // Clear the tool tip canvas's TMP_Text component
    public void ClearPriceTip()
    {
        priceTip.SetText("");
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
