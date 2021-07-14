using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModelAssets", menuName = "ScriptableObjects/ModelAssets", order = 1)]
public class ModelAssets : ScriptableObject
{
    // Meshes for head and torso
    public List<GameObject> heads;
    public List<GameObject> torsos;
}
