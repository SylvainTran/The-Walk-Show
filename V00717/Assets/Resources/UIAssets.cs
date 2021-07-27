using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "UIAssets", menuName = "ScriptableObjects/UIAssets", order = 3)]
public class UIAssets : ScriptableObject
{
    public Canvas creationCanvas;
    public Canvas dashboardOSCanvas;
    public GameObject colonistIcon;
    public GameObject colonistName;
}
