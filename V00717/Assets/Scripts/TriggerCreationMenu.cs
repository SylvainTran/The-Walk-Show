using UnityEngine;

// TODO - Would be specialized of any UI interactible menu triggers
public class TriggerCreationMenu : MonoBehaviour
{
    // Delegate and event for player triggered creation menu collider
    public delegate void TriggerCreationMenuAction();
    public static event TriggerCreationMenuAction _OnTriggerCreationMenuAction;

    // When player triggered with the collider, set cursor to visible and alert all listeners
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>())
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _OnTriggerCreationMenuAction();
        }
    }
}
