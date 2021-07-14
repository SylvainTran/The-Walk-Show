using System;
using UnityEngine;
using TMPro;

// Deals with view render changes
public class View : MonoBehaviour
{
    // The Baby Model GO to render, and the scriptable object to pull data from
    public GameObject BabyModelGO;
    private BabyModel babyModel;
    // Scriptable object with assets
    public ModelAssets ModelAssets;

    // Adult height marker
    public GameObject adultHeightMarker;
    // Unique colonist personnel ID in colonist creation screen
    public GameObject uniqueColonistPersonnelID_CC;
    // Tool tip GO to update
    public GameObject tooltipGO;

    // Cache the BabyModel component
    public void Awake()
    {
        babyModel = BabyModelGO.GetComponent<BabyModel>();
    }

    // Attach the event listeners
    public void OnEnable()
    {
        BabyController._OnAdultHeightChanged += UpdateAdultHeightLabel;
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

    // Update tool tip box content on pointer enter some UI elements
    public void UpdateToolTip(string text)
    {
        tooltipGO.GetComponent<TMP_Text>().SetText(text);
    }

    public void ClearToolTip()
    {
        tooltipGO.GetComponent<TMP_Text>().SetText("");
    }

    // Procedure to update the skin color of BabyModelGO's game object's children
    // Gets the renderer of the go's children and updates its materials with the new rgb values
    public void UpdateSkinColor()
    {
        Renderer rendHead = BabyModelGO.transform.GetChild(0).GetComponent<Renderer>(); // Head is the first child (0)
        Renderer rendTorso = BabyModelGO.transform.GetChild(1).GetComponent<Renderer>(); // Torso is the second child (1)
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.SetColor("_Color", new Color(babyModel.SkinColorR, babyModel.SkinColorG, babyModel.SkinColorB));
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
            if (head.gameObject.name.Equals(babyModel.ActiveHeadName)) // search find for the current active head name
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
            if (torso.gameObject.name.Equals(babyModel.ActiveTorsoName)) // search find for the current active head name
            {
                newTorsoMesh = torso.gameObject.GetComponent<MeshFilter>().sharedMesh; // Prefabs use the property .sharedMesh instead of .mesh
            }
        }
        BabyModelGO.transform.GetChild(1).GetComponent<MeshFilter>().mesh = newTorsoMesh; // The torso is the second child (1)
    }
}
