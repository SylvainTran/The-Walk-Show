using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameCharacterDatabase", menuName = "ScriptableObjects/GameCharacterDatabase", order = 4)]
public class GameCharacterDatabase : ScriptableObject
{
    public List<BabyModel> colonistRegistry; // keeps the UUID etc.
    public int colonistUUIDCount = 0; // the UUID to retrieve
}
