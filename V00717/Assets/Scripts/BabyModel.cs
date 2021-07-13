using System.Collections.Generic;
using UnityEngine;

// The baby's data - also saves and retrieves data to database/json file
public class BabyModel : MonoBehaviour
{
    // The baby's sex.
    private string sex = null;
    // The property for the baby's sex.
    public string Sex { get{ return sex; } set { sex = value; } }
    // The baby's adult height.
    private float adultHeight = 0.0f;
    // The property for the baby's adult height
    public float AdultHeight { get { return adultHeight; } set { adultHeight = value; } }
    // The R value of the material for the skin
    private float skinColorR = 0.0f;
    public float SkinColorR { get { return skinColorR; } set { skinColorR = value; } }
    // The G value of the material for the skin
    private float skinColorG = 0.0f;
    public float SkinColorG { get { return skinColorG; } set { skinColorG = value; } }
    // The B value of the material for the skin
    private float skinColorB = 0.0f;
    public float SkinColorB { get { return skinColorB; } set { skinColorB = value; } }

    // The currently used mesh names for head and torso - to allow View to reload the proper mesh
    private string activeHeadName;
    public string ActiveHeadName { get { return activeHeadName; } set{ activeHeadName = value; } }
    private string activeTorsoName;
    public string ActiveTorsoName { get { return activeTorsoName; } set { activeTorsoName = value; } }

    // Meshes for head and torso
    public List<GameObject> heads;
    public List<GameObject> torsos;
}
