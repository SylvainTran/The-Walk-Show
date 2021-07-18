using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PageController._OnTriggerExitCreationMenuAction += EnablePlayerController;
    }

    // Disable listeners for creation menu trigger actions
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= DisablePlayerController;
        PageController._OnTriggerExitCreationMenuAction -= EnablePlayerController;
    }

    // Disable the player controller
    public void DisablePlayerController()
    {
        GetComponent<StarterAssets.FirstPersonController>().enabled = false;
    }

    // Enable the player controller with details as per old state/new state context
    public void EnablePlayerController(int newState, int oldState)
    {
        GetComponent<StarterAssets.FirstPersonController>().enabled = true;
        switch(oldState)
        {
            case (int)STATES.CREATOR_MENU:
                // resetLocations[0] is the creation menu reset position
                transform.SetPositionAndRotation(PlayerResetLocations.resetLocations[0].gameObject.transform.position, PlayerResetLocations.resetLocations[0].gameObject.transform.rotation);
                break;
            default:
                break;
        }
        // Go to new state
        state = newState;
    }
}
