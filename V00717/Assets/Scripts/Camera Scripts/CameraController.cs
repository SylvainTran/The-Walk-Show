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
}
