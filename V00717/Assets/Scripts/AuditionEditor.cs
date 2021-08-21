using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuditionEditor : MonoBehaviour
{
    public GameController GameController;
    public TMP_Text stageNamePlaceholderField;
    public TMP_Text inputStageNameField;
    public TMPro.TMP_InputField ipf;
    public TMP_Text stageNameChoice;

    public void Start()
    {
        GameController = GameObject.FindObjectOfType<GameController>();
        RandomizeFields();
    }

    public void SetNameChoice(string nickName)
    {
        // TODO different roles / cast
        stageNameChoice.SetText(nickName + "\nRole: One of the actors.");
    }

    public void RandomizeFields()
    {
        GameController = GameObject.FindObjectOfType<GameController>();
        string actorName = GameController.randomizedAuditionDatabase.actors[UnityEngine.Random.Range(0, 1000)].name;
        stageNamePlaceholderField.SetText(actorName);
        inputStageNameField.SetText(actorName);
        ipf.text = actorName;
        ipf.ForceLabelUpdate();
        SetNameChoice(actorName);
        CharacterCreationView ccv = GetComponent<CharacterCreationView>();
        GameObject newCharacterModelInstance = ccv.newCharacterModelInstance;
        if(newCharacterModelInstance == null) 
        {
            GetComponent<CharacterCreationView>().newCharacterModelInstance = Instantiate(GameController.characterModelPrefab, CharacterCreationView.characterModelPrefabInstanceCoords, Quaternion.identity);
        } else
        {
            newCharacterModelInstance.GetComponent<CharacterModel>().NickName = actorName;
        }
    }
}
