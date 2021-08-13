using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreationMenuController : PageController, IWorldOverlayable
{
    // The parent world and overlay canvases
    //public Canvas parentCanvasWorld;
    //public GameObject parentCanvasOverlay;
    public List<GameObject> inputFieldList;
    public static bool validEntry = false;
    // The one liner poetry TMP asset that regenerates after each confirm
    public TMP_Text oneLinerPoetryAsset;

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
        //parentCanvasWorld.enabled = false;
        //parentCanvasOverlay.SetActive(true);
    }

    public void SetWorldMode()
    {
        //parentCanvasOverlay.enabled = false;
        //parentCanvasWorld.SetActive(true);
    }

    public override void SetActiveMenuCanvas()
    {
        //StarterAssetsInputs.SetActiveMenuCanvas(parentCanvasOverlay);
    }

    public void ClearActiveMenuCanvas()
    {
        if(!CreationMenuController.validEntry)
        {
            return;
        }
        SetWorldMode();
        _OnTriggerExitCreationMenuAction(); // Listened by Player.cs (re-enable player controller) and CameraController.cs (re-enable camera)
        StarterAssets.StarterAssetsInputs.ClearActiveMenu();
        foreach (GameObject f in inputFieldList)
        {
            if (!f.GetComponent<TMPro.TMP_InputField>())
            {
                continue;
            }
            TMPro.TMP_InputField ipf = f.GetComponent<TMPro.TMP_InputField>();
            if (ipf.text != null || ipf.text.Length >= 0)
            {
                ipf.text = null;
            }
        }
    }

    public override void ChangePage(int pageIndex)
    {
        // Cache the active page index
        StarterAssets.StarterAssetsInputs.activeMenuCanvas = pages[pageIndex];
        base.ChangePage(pageIndex);
        SetTextAssetEnabled(oneLinerPoetryAsset, false);
    }

    public void SetTextAssetEnabled(TMP_Text textAsset, bool enabledOverride)
    {
        textAsset.enabled = enabledOverride;
    }

    public override void ConfirmPage()
    {
        base.ConfirmPage();

        // TODO Regenerate new one liner
        // oneLinerPoetryAsset
        SetTextAssetEnabled(oneLinerPoetryAsset, true);
    }

    //Must validate all input fields before going on (no empty fields allowed)
    public void ValidateFields()
    {
        // If we validated earlier, reset
        if(validEntry)
        {
           validEntry = false;
        }
        foreach(GameObject f in inputFieldList)
        {
            if (!f.GetComponent<TMPro.TMP_InputField>())
            {
                continue;
            }
            if(f.GetComponent<TMPro.TMP_InputField>().text == null || f.GetComponent<TMPro.TMP_InputField>().text.Length <= 0)
            {
                Debug.Log("Invalid Entry");
                validEntry = false;
                return;
            }
        }
        validEntry = true;
    }

    public void ResetFields()
    {
        // Clear fields after immediately
        foreach (GameObject f in inputFieldList)
        {
            f.GetComponent<TMPro.TMP_InputField>().text = null;
        }
        validEntry = false;
    }
}
