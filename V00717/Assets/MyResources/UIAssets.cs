using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UIAssets", menuName = "ScriptableObjects/UIAssets", order = 3)]
public class UIAssets : ScriptableObject
{
    public Canvas creationCanvas;
    public Canvas dashboardOSCanvas;
    public GameObject colonistIcon;
    public GameObject colonistName;
    public GameObject eventLogText;
    public Texture2D[] UIQuadrantUIActionIcons; // NE to SE cartesian order
    public GameObject[] UIQuadrantIconsGO;
    public GameObject UIQuadrantIcon; // Fallback
    public GameObject UIAlertIcon;
}
