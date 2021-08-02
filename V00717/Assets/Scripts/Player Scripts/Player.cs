using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

// Handles components on the player
public class Player : MonoBehaviour
{
    // The player's current state enum : either as creator, in X menu, or as creature, where X : set of game menus
    public enum STATES { AS_CREATOR, CREATOR_MENU, AS_CREATURE };
    // Current state
    private int state;
    public int State { get { return state; } set { state = value; } }

    // The scriptable object that contains all player reset location references
    public PlayerResetLocations PlayerResetLocations;

    // Enable listeners for creation menu trigger actions
    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += DisablePlayerController;
        CreationMenuController._OnTriggerExitCreationMenuAction += EnablePlayerController;
        StarterAssetsInputs._OnTriggerOpenDashboardOS += DisablePlayerController;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += EnablePlayerController;
    }

    // Disable listeners for creation menu trigger actions
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= DisablePlayerController;
        CreationMenuController._OnTriggerExitCreationMenuAction -= EnablePlayerController;
        StarterAssetsInputs._OnTriggerOpenDashboardOS -= DisablePlayerController;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= EnablePlayerController;
    }

    // Disable the player controller
    public void DisablePlayerController()
    {
        GetComponent<StarterAssets.FirstPersonController>().enabled = false;
    }

    // Enable the player controller with details as per old state/new state context
    public void EnablePlayerController()
    {
        GetComponent<StarterAssets.FirstPersonController>().enabled = true;
    }
}
