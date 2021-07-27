using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// Deals with view render changes
public class CharacterCreationView : MonoBehaviour
{
    // The Baby Model GO to render, and the scriptable object to pull data from
    public GameObject BabyModelGO;
    public BabyController babyController;
    // Scriptable object with assets
    public ModelAssets ModelAssets;

    // Adult height marker
    public GameObject adultHeightMarker;
    // Unique colonist personnel ID in colonist creation screen
    public GameObject uniqueColonistPersonnelID_CC;

    // E-Cart notification button (for fade animation)
    public GameObject ecartNotificationButton;
    // The following are cached from inspector
    // Tool tip text to update
    public TMP_Text tooltipGO;

    public Image ecnb = null; 
    public TMP_Text text = null;
    // The price tip
    public TMP_Text priceTip;

    // Attach the event listeners
    public void OnEnable()
    {
        BabyController._OnAdultHeightChanged += UpdateAdultHeightLabel;
        BabyController._OnAdultHeightChanged += TriggerTextAnimation;
        BabyController._OnSkinColorChanged += UpdateSkinColor;
        BabyController._OnHeadMeshChanged += UpdateHeadMesh;
        BabyController._OnTorsoMeshChanged += UpdateTorsoMesh;
        BabyController._OnToolTipAction += UpdateToolTip;
        BabyController._OnToolTipExitAction += ClearToolTip;
    }

    // Detach the event listeners
    public void OnDisable()
    {
        BabyController._OnAdultHeightChanged -= UpdateAdultHeightLabel;
        BabyController._OnAdultHeightChanged -= TriggerTextAnimation;
        BabyController._OnSkinColorChanged -= UpdateSkinColor;
        BabyController._OnHeadMeshChanged -= UpdateHeadMesh;
        BabyController._OnTorsoMeshChanged -= UpdateTorsoMesh;
        BabyController._OnToolTipAction -= UpdateToolTip;
        BabyController._OnToolTipExitAction -= ClearToolTip;
    }

    // Updates the adult height marker label
    public void UpdateAdultHeightLabel(float value)
    {
        adultHeightMarker.GetComponent<TMP_Text>().SetText($"{Math.Round(value)} cm");
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
        Renderer rendHead = BabyModelGO.transform.GetChild(0).GetComponent<Renderer>(); // Head is the first child (0)
        Renderer rendTorso = BabyModelGO.transform.GetChild(1).GetComponent<Renderer>(); // Torso is the second child (1)
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.SetColor("_Color", new Color(babyController.BabyModel.SkinColorR, babyController.BabyModel.SkinColorG, babyController.BabyModel.SkinColorB));
        rendHead.material = newMat;
        rendTorso.material = newMat;
    }

    // Views update itself concerning head/torso changes in UI
    // Changes the mesh of the baby model by accessing its mesh filter setter and mesh property
    public void UpdateHeadMesh()
    {
        Mesh newHeadMesh = null;
        foreach (GameObject head in ModelAssets.heads)
        {
            Debug.Log($"Head Mesh filter: {head.gameObject.name}");
            if (head.gameObject.name.Equals(babyController.BabyModel.ActiveHeadName)) // search find for the current active head name
            {
                newHeadMesh = head.gameObject.GetComponent<MeshFilter>().sharedMesh; // Prefabs use the property .sharedMesh instead of .mesh
            }
        }
        BabyModelGO.transform.GetChild(0).GetComponent<MeshFilter>().mesh = newHeadMesh; // The head is the first child (0)
    }

    // Change torso mesh - TODO abstract head/torso into general mesh?
    public void UpdateTorsoMesh()
    {
        Mesh newTorsoMesh = null;
        foreach (GameObject torso in ModelAssets.torsos)
        {
            Debug.Log($"Torso Mesh filter: {torso.gameObject.name}");
            if (torso.gameObject.name.Equals(babyController.BabyModel.ActiveTorsoName)) // search find for the current active head name
            {
                newTorsoMesh = torso.gameObject.GetComponent<MeshFilter>().sharedMesh; // Prefabs use the property .sharedMesh instead of .mesh
            }
        }
        BabyModelGO.transform.GetChild(1).GetComponent<MeshFilter>().mesh = newTorsoMesh; // The torso is the second child (1)
    }
}
