using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuditionEditor : MonoBehaviour
{
    public TMP_Text stageNamePlaceholderField;
    public TMP_Text inputStageNameField;
    public TMP_Text stageNameChoice;

    public void Start()
    {
        inputStageNameField.SetText(stageNamePlaceholderField.text);
    }

    public void SetNameChoice()
    {
        // TODO different roles / cast
        stageNameChoice.SetText(inputStageNameField.text + "\nRole: One of the actors.");
    }

    public void RandomizeFields()
    {

    }
}
