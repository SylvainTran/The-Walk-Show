using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float SkinColorR { set { skinColorR = value; } }
    // The G value of the material for the skin
    private float skinColorG = 0.0f;
    public float SkinColorG { set { skinColorG = value; } }
    // The B value of the material for the skin
    private float skinColorB = 0.0f;
    public float SkinColorB { set { skinColorB = value; } }

    //Setter for baby's sex via Unity's built-in event system.
    public void OnSexChanged(string sex)
    {
        this.sex = sex;
        Debug.Log($"Baby's sex was changed to: {sex}");
    }

    //Setter for baby's adult height via Unity's built-in event system.
    public void OnAdultHeightChanged(float adultHeight)
    {
        this.adultHeight = adultHeight;
        Debug.Log($"Baby's adult height was changed to: {adultHeight}");
    }

    //Setter for skin color changed (red slider)
    public void OnSkinColorChanged_R(float r)
    {
        skinColorR = r;
        UpdateSkinColor();
    }

    //Setter for skin color changed (green slider)
    public void OnSkinColorChanged_G(float g)
    {
        skinColorG = g;
        UpdateSkinColor();
    }

    //Setter for skin color changed (blue slider)
    public void OnSkinColorChanged_B(float b)
    {
        skinColorB = b;
        UpdateSkinColor();
    }

    // Procedure to update the skin color of this game object's children
    // Gets the renderer of this go's children and updates its materials with the new rgb values
    public void UpdateSkinColor()
    {
        Renderer rendHead = transform.GetChild(0).GetComponent<Renderer>();
        Renderer rendTorso = transform.GetChild(1).GetComponent<Renderer>();
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.SetColor("_Color", new Color(skinColorR, skinColorG, skinColorB));
        rendHead.material = newMat;
        rendTorso.material = newMat;
    }
}
