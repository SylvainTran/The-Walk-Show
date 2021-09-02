using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
///Get the cached game object's data being handled
// match it with the actionActorTarget property
// Get that target's transform position in world and drop the item 3D model
// with gravity etc.
/// </summary>
public class DraggedActionHandler : MonoBehaviour, IDropHandler
{
    public ActionBelt actionBelt;
    /// <summary>
    /// Should be set during auditioning stage.
    /// </summary>
    /// <param name="UUID"></param>
    private int actionActorTargetUUID = -1;
    public int ActionActorTargetUUID { get { return actionActorTargetUUID; } set { actionActorTargetUUID = value; } }
    
    private void Awake()
    {
        if(actionBelt == null)
        {
           actionBelt = FindObjectOfType<ActionBelt>();
        }
    }

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log("Nudging actor");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped on me: " + actionActorTargetUUID);
        actionBelt.InvokeActionActorTarget(actionActorTargetUUID);
    }
}
