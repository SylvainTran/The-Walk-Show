using UnityEngine;
using StarterAssets;

// TODO - Would be specialized of any UI interactible menu triggers
public class TriggerCreationMenu : MonoBehaviour
{
    // Delegate and event for player triggered creation menu collider
    public delegate void TriggerCreationMenuAction();
    public static event TriggerCreationMenuAction _OnTriggerCreationMenuAction;
    // Whether this game object is currently interactible with - Set by input key trigger events in StarterAssetsInputs.cs
    private bool isInteractable = false;
    public Canvas dashboardOS = null;

    private void OnEnable()
    {
        StarterAssetsInputs._OnPlayerInteracted += SetInteractiveState;
    }

    private void OnDisable()
    {
        StarterAssetsInputs._OnPlayerInteracted -= SetInteractiveState;
    }

    // Toggle interactive state
    public void SetInteractiveState()
    {
        isInteractable = !isInteractable;
    }

    // When player triggered with the collider, set cursor to visible and alert all listeners
    private void OnTriggerStay(Collider other)
    {
        // Open the creation menu if there is no currently active menu canvas
        if (other.gameObject.GetComponent<Player>())
        {
            if (dashboardOS && dashboardOS.enabled)
            {
                return;
            }
            if (StarterAssetsInputs.activeMenuCanvas == null || !StarterAssetsInputs.activeMenuCanvas.activeInHierarchy)
            {
                if (isInteractable)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    _OnTriggerCreationMenuAction();
                    SetInteractiveState(); // Need to repress Enter again to retrigger
                }
            }
        }
    }
}
