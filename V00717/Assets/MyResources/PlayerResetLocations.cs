using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The reset locations for each interactible stations
[CreateAssetMenu(fileName = "PlayerResetLocations", menuName = "ScriptableObjects/PlayerResetLocations", order = 2)]
public class PlayerResetLocations : ScriptableObject
{
    public List<GameObject> resetLocations;
}
