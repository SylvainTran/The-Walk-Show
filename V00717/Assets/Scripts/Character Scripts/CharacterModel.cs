using System;
using UnityEngine;

[Serializable]
public class CharacterModel : Element, ISerializableObject, ICombatant
{
    /// <summary>
    /// The current day's schedule
    /// </summary>
    public Schedule schedule;
    /// <summary>
    /// Approval with the player
    /// </summary>
    public float approval;
    [SerializeField] private string nickName = null;
    [SerializeField] private int uniqueColonistPersonnelID_ = 0;
    [SerializeField] private float health = 500.0f;
    [SerializeField] private int level = 0;
    [SerializeField] private int skillPoints = 1;
    [SerializeField] private string gender;
    private float physicalSkill = 15.0f;
    [SerializeField] private string activeHeadName;
    [SerializeField] private string activeTorsoName;
    private bool isInPendingCall = false;
    [SerializeField] public EventMarkersMap eventMarkersMap = null;
    [SerializeField] private string lastEvent = null;
    private int trackLanePosition = 0;
    [SerializeField] private int inQuadrant = -1;
    public int UniqueColonistPersonnelID_ { get => uniqueColonistPersonnelID_; set => uniqueColonistPersonnelID_ = value; }
    public int Level { get => level; set => level = value; }
    public float Health { get => health; set => health = value; }
    public string Gender { get => gender; set => gender = value; }
    public int SkillPoints { get => skillPoints; set => skillPoints = value; }
    public float PhysicalSkill { get => physicalSkill; set => physicalSkill = value; }
    public string ActiveHeadName { get => activeHeadName; set => activeHeadName = value; }
    private int goldInventory = 0;
    public string ActiveTorsoName { get => activeTorsoName; set => activeTorsoName = value; }
    public bool IsInPendingCall { get => isInPendingCall; set => isInPendingCall = value; }
    public string LastEvent { get => lastEvent; set => lastEvent = value; }
    public int TrackLanePosition { get => trackLanePosition; set => trackLanePosition = value; }
    public int InQuadrant { get => inQuadrant; set => inQuadrant = value; }
    public int GoldInventory { get => goldInventory; set => goldInventory = value; }
    public string NickName { get => nickName; set => nickName = value; }

    public void InitCharacterModel(CharacterModelObject other)
    {
        this.nickName = other.NickName;
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
        return $"Nickname: {nickName}, Level: {level}, Status/Health: {health}";
    }
    public delegate void OnGameClockEventProcessed(GameClockEvent e);
    public static OnGameClockEventProcessed _OnGameClockEventProcessed;
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
