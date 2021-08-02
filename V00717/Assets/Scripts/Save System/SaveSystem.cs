using System;
using System.IO;
using UnityEngine;
using static BabyController;

// Deals with loading, saving, and auto-save behaviours
public class SaveSystem : MonoBehaviour
{
    // This action is to tell any other components that we have a successful save (e.g., when updating the colonist uuid)
    public static Action _SuccessfulSaveAction;

    private void OnEnable()
    {
        _OnSaveAction += Save;
    }

    private void OnDisable()
    {
        _OnSaveAction += Save;
    }

    // Save to list and then to file
    public void Save(string key, BabyModel[] colonists, BabyModel babyModel, string path)
    {
        // The count of characters should be x+1 where x is the current count
        SavedArrayObject savedObject = new SavedArrayObject(colonists);
        int nbElements = Utility.Count(savedObject);

        if (nbElements < MAX_COLONISTS)
        {
            colonists[nbElements] = babyModel;
            // Make the UUID - TODO this doesn't work? It sets the previous ids back to 0
            BabyModel.uniqueColonistPersonnelID++;
            babyModel.UniqueColonistPersonnelID_ = BabyModel.uniqueColonistPersonnelID;            
            SaveToJSONFile(key, nbElements, savedObject, path, "Save successful");
        }
        else
        {
            Debug.Log("Save game impossible :-(. Full capacity reached.");
        }
    }

    // For JSON deserialization - needs a wrapper class to serialize an array type
    public struct SavedArrayObject
    {
        public BabyModel[] colonists;

        public SavedArrayObject(BabyModel[] colonists)
        {
            this.colonists = colonists;
        }
    }

    // Save multiple objects to a text file
    public void SaveToJSONFile(string key, int nbElements, SavedArrayObject savedObject, string path, string successMessage)
    {
        string output = "{\n\t";
        int len = nbElements + 1;
        output += "\"" + key + "\":" + "[";

        for (int i = 0; i < len; i++)
        {
            // ToJSON the colonist at the current index i
            output += "\t\t" + JsonUtility.ToJson(savedObject.colonists[i]);
            // Add a comma if more than one entry and not last entry
            if (len > 1 && i < len - 1)
            {
                output += ",\n\t";
            }
        }

        output += "]\n\t}";
        // Write to text file (synchronously)
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            outputFile.WriteLine(output);
            Debug.Log(successMessage);
            // Success event
            _SuccessfulSaveAction();
        }
    }
    // Checks if save file exists
    public static bool SaveFileExists(string path)
    {
        return System.IO.File.Exists(path);
    }
}
