using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class CreationMenuController : PageController, IWorldOverlayable
{
    // The parent world and overlay canvases
    public Canvas parentCanvasWorld;
    public Canvas parentCanvasOverlay;

    public delegate void TriggerExitCreationMenuAction();
    public static event TriggerExitCreationMenuAction _OnTriggerExitCreationMenuAction;

    private void OnEnable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += SetOverlayMode;
        TriggerCreationMenu._OnTriggerCreationMenuAction += SetActiveMenuCanvas;
        StarterAssetsInputs._OnTriggerCloseActiveMenu += ClearActiveMenuCanvas;
    }

    private void OnDisable()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= SetOverlayMode;
        TriggerCreationMenu._OnTriggerCreationMenuAction -= SetActiveMenuCanvas;
        StarterAssetsInputs._OnTriggerCloseActiveMenu -= ClearActiveMenuCanvas;
    }

    public void SetOverlayMode()
    {
        parentCanvasWorld.GetComponent<Canvas>().enabled = false;
        parentCanvasOverlay.GetComponent<Canvas>().enabled = true;
    }

    public void SetWorldMode()
    {
        parentCanvasOverlay.GetComponent<Canvas>().enabled = false;
        parentCanvasWorld.GetComponent<Canvas>().enabled = true;
    }

    public override void SetActiveMenuCanvas()
    {
        StarterAssetsInputs.SetActiveMenuCanvas(parentCanvasOverlay);
    }

    public void ClearActiveMenuCanvas()
    {
        SetWorldMode();
        _OnTriggerExitCreationMenuAction(); // Listened by Player.cs (re-enable player controller) and CameraController.cs (re-enable camera)
    }
}
