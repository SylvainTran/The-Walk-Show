using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CreationMenuController : PageController
{
    // The parent world and overlay canvases
    //public Canvas parentCanvasWorld;
    //public GameObject parentCanvasOverlay;
    public List<GameObject> inputFieldList;
    public bool validEntry = false;
    // The one liner poetry TMP asset that regenerates after each confirm
    public TMP_Text oneLinerPoetryAsset;
    public GameController GameController;
    // The lonely confirm page button
    public Button confirmPageButton;
    // The finalize button which saves the edits and exits the menu
    public Button finalizeButton;

    public Button idPage;
    public Button profilePage;
    public Button itemPage;

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

    public void Start()
    {
        GameController = GameObject.FindObjectOfType<GameController>();
        // Add event listeners to all buttons
        // Add the handle mouse event trigger
        AddEventListener(confirmPageButton.gameObject, EventTriggerType.PointerClick, 0, null, ConfirmPage);
        // TODO button onClick handler or event trigger for all these?
        //AddEventListener(finalizeButton.gameObject, EventTriggerType.PointerClick, 0, null, GetComponent<CharacterCreationView>().AddNewColonistToRegistry);
        AddEventListener(idPage.gameObject, EventTriggerType.PointerClick, 0, ChangePage, null);
        AddEventListener(profilePage.gameObject, EventTriggerType.PointerClick, 1, ChangePage, null);
        AddEventListener(itemPage.gameObject, EventTriggerType.PointerClick, 4, ChangePage, null);
    }

    public void AddEventListener(GameObject target, EventTriggerType triggerType, int pageIndex, Action<int> callbackA, Action callbackB)
    {
        EventTrigger evt = target.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;// EventTriggerType.PointerClick;
        evt.triggers.Add(entry);
        if (callbackA != null)
        {
            entry.callback.AddListener((eventData) => { callbackA(pageIndex); });
        }
        if (callbackB != null)
        {
            entry.callback.AddListener((eventData) => { callbackB(); });
        }
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
        if(!validEntry)
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
        GameObject previouslyActiveCanvas = null;
        for(int i = 0; i < pages.Length; i++)
        {
            if(pages[i].activeInHierarchy)
            {
                previouslyActiveCanvas = pages[i];
                break;
            }
        }

        StarterAssetsInputs.previouslyActiveCanvas = previouslyActiveCanvas;
        // Cache the active page index
        StarterAssets.StarterAssetsInputs.activeMenuCanvas = pages[pageIndex];
        base.ChangePage(pageIndex);

        // Re-enable the confirm button
        if (!confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(true);
        }
        // Disable the finalize button
        if (finalizeButton.gameObject.activeInHierarchy)
        {
            finalizeButton.gameObject.SetActive(false);
        }

        SetTextAssetEnabled(oneLinerPoetryAsset, false);
    }

    public void SetTextAssetEnabled(TMP_Text textAsset, bool enabledOverride)
    {
        textAsset.enabled = enabledOverride;
    }

    public override void ConfirmPage()
    {
        base.ConfirmPage();

        // Hide confirm page button
        if (confirmPageButton.gameObject.activeInHierarchy)
        {
            confirmPageButton.gameObject.SetActive(false);
        }
        // Show finalize page button
        if (!finalizeButton.gameObject.activeInHierarchy)
        {
            finalizeButton.gameObject.SetActive(true);
        }

        // TODO Regenerate new one liner
        // oneLinerPoetryAsset
        SetTextAssetEnabled(oneLinerPoetryAsset, true);
    }

    //Must validate all input fields before going on (no empty fields allowed)
    public bool ValidateFields()
    {
        if(GameController.Colonists.Count > CreationController.MAX_COLONISTS)
        {
            Debug.LogError("Max characters reached already. Some need to die to leave space for others.");
            validEntry = false;
            return false;
        }
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
            f.GetComponent<TMPro.TMP_InputField>().ForceLabelUpdate();
            if(f.GetComponent<TMPro.TMP_InputField>().text == null || f.GetComponent<TMPro.TMP_InputField>().text.Length <= 0)
            {
                Debug.Log("Invalid Entry");
                validEntry = false;
                return false;
            }
        }
        validEntry = true;
        return true;
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

    public void DestroyEditor()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Reshuffles
    /// </summary>
    public void RejectCandidate()
    {
        // Destroy its go
        Destroy(GetComponent<CharacterCreationView>().newCharacterModelInstance.gameObject);
        if(GameController.Colonists.Count < CreationController.MAX_COLONISTS)
        {
            GameController.StartAuditionsAfterDelay(2);
        }
    }
}
