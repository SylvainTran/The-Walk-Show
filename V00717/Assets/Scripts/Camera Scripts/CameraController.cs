using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class CameraController : MonoBehaviour
{
    // The character creation cam - TODO put in so
    public GameObject characterCreationCam;
    // The max virtual camera priority value
    private int MAX_PRIORITY = 100;
    // The default virtual camera priority value
    private int DEFAULT_PRIORITY = 0;
    // To manage the camera lane feeds
    public GameController GameController;

    // Enable listeners
    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += SetCreationMenuCamPriority;
        CreationMenuController._OnTriggerExitCreationMenuAction += ResetCamPriority;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += ResetCamPriority;
    }

    // Disable listeners
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= SetCreationMenuCamPriority;
        CreationMenuController._OnTriggerExitCreationMenuAction -= ResetCamPriority;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= ResetCamPriority;
    }

    // Sets the creation menu's virtual cam live
    public void SetCreationMenuCamPriority()
    {
        characterCreationCam.GetComponent<CinemachineVirtualCamera>().Priority = MAX_PRIORITY;
    }

    // Reset all virtual cameras priorities that aren't the PlayerFollowCamera
    public void ResetCamPriority()
    {
        foreach (CinemachineVirtualCamera c in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            if (!c.gameObject.name.Equals("PlayerFollowCamera"))
            {
                c.gameObject.GetComponent<CinemachineVirtualCamera>().Priority = DEFAULT_PRIORITY;
            }
        }
    }

    public void LateUpdate()
    {
        if (GameController.CreationController == null || GameController.CreationController.LaneFeedCams.Length == 0)
        {
            Debug.LogError("You probably forgot to setup the creation controller in the inspector of this script, or there are no camera lanes serialized over there.");
            return;
        }
        // Update the camera live feed (alive only get to be watched?)
        int trackLanePositionUpdated = 0;

        for (int i = 0; i < GameController.CreationController.LaneFeedCams.Length; i++)
        {
            // If the target is null (not occupied yet) or dead, then the new character can evict them/take their position
            CharacterTracker characterTracker = GameController.CreationController.LaneFeedCams[i].GetComponent<CharacterTracker>();

            if (characterTracker.Target == null || characterTracker.Target.GetComponent<CharacterModel>().isDead())
            {
                trackLanePositionUpdated = i;
                // Tv screen noise effect on that camera?
                ApplySignalNoiseEffect(trackLanePositionUpdated);
            }
        }
    }

    public void ApplySignalNoiseEffect(int tracklane)
    {
        // Apply a noise effect on the render texture or some other meaningful image, or at least cut the live feed temporarily
    }
}
