using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // wrapper class to serialize (can't with monobehaviours)
public class CharacterModelObject : ISerializableObject
{
    // The newborn's nickname - set during creation
    [SerializeField] private string nickName = null;
    public string NickName { get { return nickName; } set { nickName = value; } }

    // Colonist unique personnel ID
    public static int uniqueColonistPersonnelID = 0;
    [SerializeField] private int uniqueColonistPersonnelID_ = 0;
    public int UniqueColonistPersonnelID_ { get { return uniqueColonistPersonnelID_; } set { uniqueColonistPersonnelID_ = value; } }

    // Colonist health - Can be reduced by conditions, damage taken during certain events?
    [SerializeField] private float health = 100.0f;
    public float Health { get { return health; } set { health = value; } }
    // Virus strains/diseases/conditions active on the colonist
    //[SerializeField] private List<Condition> activeConditions = null;

    // Colonist level (for progression)
    [SerializeField] private int level = 0;
    public int Level { get { return level; } set { level = value; } }
    // Colonist skill points (acquired so far)
    [SerializeField] private int skillPoints = 1;
    public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    // Colonist genetic points (determined throughout progression)

    public string Name()
    {
        return nickName;
    }
    // Physical skill (has damage too)
    private float physicalSkill = 15.0f;
    public float PhysicalSkill { get { return physicalSkill; } set { physicalSkill = value; } }

    // The currently used mesh names for head and torso - to allow View to reload the proper mesh
    [SerializeField] private string activeHeadName;
    public string ActiveHeadName { get { return activeHeadName; } set { activeHeadName = value; } }
    [SerializeField] private string activeTorsoName;
    public string ActiveTorsoName { get { return activeTorsoName; } set { activeTorsoName = value; } }

    // Tags/event markers map used for obituary generation: a string key is mapped to a frequency count.
    // Possible keys are in Enums.CharacterAchievements (Enum.cs)
    private bool isInPendingCall = false;
    public bool IsInPendingCall { get { return isInPendingCall; } set { isInPendingCall = value; } }

    // Instance of the eventMarkersMap
    [SerializeField] public EventMarkersMap eventMarkersMap = null;

    // Last event is the cause of death
    [SerializeField] private string lastEvent = null;
    public string LastEvent { get { return lastEvent; } set { lastEvent = value; } }

    // To serialize wrapper fill-in the actual characterModel
    public void InitCharacterModel(CharacterModel other)
    {
        this.nickName = other.NickName;
        this.activeHeadName = other.ActiveHeadName;
        this.activeTorsoName = other.ActiveTorsoName;
        this.uniqueColonistPersonnelID_ = other.UniqueColonistPersonnelID_;
        this.eventMarkersMap = other.eventMarkersMap;
        this.lastEvent = other.LastEvent;
    }
}
