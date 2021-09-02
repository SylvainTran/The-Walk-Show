using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
//using MilkShake;

public class CameraController : MonoBehaviour
{
    // The max virtual camera priority value
    // private int MAX_PRIORITY = 100;
    // The default virtual camera priority value
    private int DEFAULT_PRIORITY = 0;
    // To manage the camera lane feeds
    public GameController GameController;
    // The shake camera parameters
    //public ShakePreset shakepreset;

    // Transforms for parenting pending calls
    public Transform cameraLane1TargetCallTransform;
    public Transform cameraLane2TargetCallTransform;
    public Transform cameraLane3TargetCallTransform;
    public Transform cameraLane4TargetCallTransform;

    // Enable listeners
    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += SetCreationMenuCamPriority;
        CreationMenuController._OnTriggerExitCreationMenuAction += ResetCamPriority;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += ResetCamPriority;
        GameClockEvent._OnColonistIsDead += ShakeCamera;
    }

    // Disable listeners
    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= SetCreationMenuCamPriority;
        CreationMenuController._OnTriggerExitCreationMenuAction -= ResetCamPriority;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= ResetCamPriority;
        GameClockEvent._OnColonistIsDead -= ShakeCamera;
    }

    // Sets the creation menu's virtual cam live
    public void SetCreationMenuCamPriority()
    {
        //characterCreationCam.GetComponent<CinemachineVirtualCamera>().Priority = MAX_PRIORITY;
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

    public void ShakeCamera(GameClockEvent e, GameObject go)
    {
        //GetComponent<Shaker>().Shake(shakepreset);
    }

    public void LateUpdate()
    {
        if (GameController.CreationController == null || GameController.CreationController.LaneFeedCams.Length == 0 || GameController.CreationController.LaneFeedCams == null)
        {
            //Debug.LogError("You probably forgot to setup the creation controller in the inspector of this script, or there are no camera lanes serialized over there.");
            return;
        }
        // Update the camera live feed (alive only get to be watched?)
        int trackLanePositionUpdated = 0;

        for (int i = 0; i < GameController.CreationController.LaneFeedCams.Length; i++)
        {
            // If the target is null (not occupied yet) or dead, then the new character can evict them/take their position
            CharacterTracker characterTracker = GameController.CreationController.LaneFeedCams[i].GetComponent<CharacterTracker>();
            if (characterTracker.Target == null)
            {
                return;
            }
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

    /// <summary>
    /// if there is such a moment as "high priority" like a zombie chase, or anything that involves more than
    /// one actor and colliders, activate a special camera mode to follow it
    /// WIP
    /// </summary>
    public void TrackActionHighlights(int trackLane)
    {
        // The camera mode should clearly display the moment and prioritize main actors
        // Get the front vector of the main actor in the highlight
        Camera camera = GameController.laneFeedCams[trackLane];
        Transform mainActor = camera.GetComponent<CharacterTracker>().Target;
        float desiredYaw = mainActor.rotation.eulerAngles.y;
        Vector3 perp = Vector3.Cross(mainActor.rotation.eulerAngles, camera.transform.rotation.eulerAngles);

        // Lerp to the new position over a few frames
        float n = 0;

        while(n < desiredYaw)
        {
            n += Time.deltaTime;
            //camera.transform.rotation;
        }
        // Close-up SIDE
        // Close-up ORBIT 45
        // Close-up BACK/FRONT
        // Get the camera at trackLane
        // Determine what kind of highlight it is
        // Move the camera to display judicious angles
    }
}
