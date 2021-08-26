using System;
using System.Collections.Generic;
using UnityEngine;

// The baby's data - also provides methods to save and retrieve data to database/json file
[Serializable]
public class CharacterModel : Element, ISerializableObject, ICombatant
{
    // The newborn's nickname - set during creation
    [SerializeField] private string nickName = null;
    public string NickName { get { return nickName; } set { nickName = value; } }

    // Colonist unique personnel ID
    [SerializeField] private int uniqueColonistPersonnelID_ = 0;
    public int UniqueColonistPersonnelID_ { get { return uniqueColonistPersonnelID_; } set { uniqueColonistPersonnelID_ = value; } }

    // Colonist health - Can be reduced by conditions, damage taken during certain events?
    [SerializeField] private float health = 500.0f;
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
    [SerializeField] private int geneticPoints = 10;
    public int GeneticPoints { get { return geneticPoints; } set { geneticPoints = value; } }

    // The baby's sex.
    [SerializeField] private string sex = null;
    // The property for the baby's sex.
    public string Sex { get{ return sex; } set { sex = value; } }
    // The newborn's age -> progresses overtime
    [SerializeField] private int age = 1;
    public int Age { get { return age; } set { age = value; } }
    // Life expectancy: starts at 100, changes throughout colonist's life depending on disease/viral strains/injury/moral/etc.
    [SerializeField] private int lifeExpectancy = 100;
    public int LifeExpectancy { get { return lifeExpectancy; } set { lifeExpectancy = value; } }
    // Quality of life years expected (how many years the colonist is expected to thrive healthily)
    [SerializeField] private int qalys;
    public int Qalys { get { return qalys; } set { qalys = value; } }

    // The R value of the material for the skin
    [SerializeField] private float skinColorR = 0.0f;
    public float SkinColorR { get { return skinColorR; } set { skinColorR = value; } }
    // The G value of the material for the skin
    [SerializeField] private float skinColorG = 0.0f;
    public float SkinColorG { get { return skinColorG; } set { skinColorG = value; } }
    // The B value of the material for the skin
    [SerializeField] private float skinColorB = 0.0f;
    public float SkinColorB { get { return skinColorB; } set { skinColorB = value; } }

    // Stable temperament - general master variable
    [SerializeField] private float temperament = 0.0f;
    // The property for the baby's temperament
    public float Temperament { get { return temperament; } set { temperament = value; } }

    // Openness
    [SerializeField] private float openness = 0.0f;
    // The property for the baby's openness
    public float Openness { get { return openness; } set { openness = value; } }

    // Conscientitiousness
    [SerializeField] private float conscientitiousness = 0.0f;
    // The property for the baby's conscientitiousness
    public float Conscientitiousness { get { return conscientitiousness; } set { conscientitiousness = value; } }

    // Extraversion
    [SerializeField] private float extraversion = 0.0f;
    // The property for the baby's extraversion
    public float Extraversion { get { return extraversion; } set { extraversion = value; } }

    // Agreeableness
    [SerializeField] private float agreeableness = 0.0f;
    // The property for the baby's agreeableness
    public float Agreeableness { get { return agreeableness; } set { agreeableness = value; } }

    // Neuroticism
    [SerializeField] private float neuroticism = 0.0f;
    // The property for the baby's neuroticism
    public float Neuroticism { get { return neuroticism; } set { neuroticism = value; } }

    /// <summary>
    /// Stress levels: influences dialogue
    /// </summary>
    private float stressTolerance = 0.0f;
    public float StressTolerance { get { return stressTolerance; } set { stressTolerance = value; } }

    /// <summary>
    /// Moral levels: influences dialogue
    /// </summary>
    private float moralLevel = 0.0f;
    public float MoralLevel { get { return moralLevel; } set { moralLevel = value; } }

    // Physical skill (has damage too)
    private float physicalSkill = 15.0f;
    public float PhysicalSkill { get { return physicalSkill; } set { physicalSkill = value; } }

    // The currently used mesh names for head and torso - to allow View to reload the proper mesh
    [SerializeField] private string activeHeadName;
    public string ActiveHeadName { get { return activeHeadName; } set{ activeHeadName = value; } }
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

    // Current camera track lane occupation
    private int trackLanePosition = 0;
    public int TrackLanePosition { get { return trackLanePosition; } set { trackLanePosition = value; } }

    // Quadrant location -1 means not assigned yet - this is checked in Bot.cs
    [SerializeField]
    private int inQuadrant = -1;
    public int InQuadrant { get { return inQuadrant; } set { inQuadrant = value; } }

    [SerializeField]
    private int goldInventory = 0;
    public int GoldInventory { get { return goldInventory; } set { goldInventory = value; } }

    public void InitCharacterModel(CharacterModelObject other)
    {
        this.nickName = other.NickName;
        this.sex = other.Sex;
        this.skinColorR = other.SkinColorR;
        this.skinColorG = other.SkinColorG;
        this.skinColorB = other.SkinColorB;
        this.activeHeadName = other.ActiveHeadName;
        this.activeTorsoName = other.ActiveTorsoName;
        this.uniqueColonistPersonnelID_ = other.UniqueColonistPersonnelID_;
    }

    public void InitEventsMarkersFeed()
    {
        eventMarkersMap = new EventMarkersMap();
        string bornEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 0);
        eventMarkersMap.EventMarkersFeed = new SerializableDictionary<string, int>();
        eventMarkersMap.EventMarkersFeed.Add(bornEventAchievement, 1);
        lastEvent = null;
    }


    public void InitEventsMarkersFeed(CharacterModelObject other)
    {
        this.eventMarkersMap = other.eventMarkersMap;
        this.lastEvent = other.LastEvent;
    }


    /// <summary>
    /// ToString override
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Nickname: {nickName}, Level: {level}, Age: {age}, Status/Health: {health}";
    }

    public delegate void OnGameClockEventProcessed(GameClockEvent e);
    public static OnGameClockEventProcessed _OnGameClockEventProcessed;

    // Handler for game clock event on this model
    public void OnGameClockEventGenerated(GameClockEvent e)
    {
        if(isDead() || eventMarkersMap.EventMarkersFeed == null || isInPendingCall)
        {
            //Debug.LogError("Make sure that eventmarkersfeed is initialized properly.");
            return;
        }

        int randIndex = UnityEngine.Random.Range(0, 100);
        if(randIndex > e.TriggerChance) //DEBUG MODE: Set this to > 0; randIndex > e.TriggerChance
        {
            e.ApplyEvent(this);
 
            if (!e.GetType().Name.Equals(Enums.ToString(Enums.CharacterAchievements.GOT_BATTLE)))
            {
                _OnGameClockEventProcessed(e);
            }
        } else
        {
            Debug.Log($"Event failed to occur by chance");
        }
    }

    public void DealDamage(ICombatant opponent)
    {
        opponent.TakeDamage(physicalSkill);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsEnemyAI()
    {
        return false;
    }

    public void SetLastEvent(string lastEvent)
    {
        this.lastEvent = lastEvent;
    }

    public bool isDead()
    {
        return health <= 0.0f;
    }

    public override int GetHashCode()
    {
        return uniqueColonistPersonnelID_;
    }

    public bool Equals(CharacterModel other)
    {
        if (other == null) return false;
        return (this.GetHashCode().Equals(other.GetHashCode()));
    }

    public string Name()
    {
        return nickName;
    }
}
