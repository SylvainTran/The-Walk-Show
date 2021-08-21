using System;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static Enums.CharacterAchievements;

public abstract class BattleEvent : GameClockEvent
{
    private Enemy enemyEntity;
    private string endOfBattleMessage;

    public delegate void OnBattleEnded(BattleEvent b);
    public static event OnBattleEnded _OnBattleEnded;

    public BattleEvent(float triggerChance) : base(triggerChance)
    {
        this.endOfBattleMessage = "End of battle.";
    }

    public override bool ApplyEvent(CharacterModel b)
    {
        if(!base.ApplyEvent(b))
        {
            return false;
        }
        Message = $"[Battle Event] {b.Name()} is fighting an entity on the ship.";
        Enemy e = GenerateEntity();
        while(!CheckIfDead(b) && !CheckIfDead(e))
        {
            e.DealDamage(b);
            b.DealDamage(e);
        }
        string colonistName = b.Name();
        Message += $" The battle between {colonistName} and {e.Name()} is over.";
        AddToEventMarkersFeed(b);
        if (b.Health > 0.0f)
        {
            Message += $" {colonistName} won! {e.Name()} was savagely killed.";
        } else
        {
            Message += $" {colonistName} has died in a gruesome way {e.Name()} will be laughing at {colonistName} hysterically for all eternity...";
            b.SetLastEvent("Died in Combat");
            base.NotifyIsDead(b.gameObject);
        }
        // TODO rewards, notification pop-ups, etc. especially if colonist has died
        _OnBattleEnded(this);
        return true;
    }

    public Enemy GenerateEntity()
    {
        return new Enemy();
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        if(b.eventMarkersMap.EventMarkersFeed == null)
        {
            return;
        }
        int battleEventAchievement = (int)GOT_BATTLE;
        string key = Enum.GetName(typeof(CharacterAchievements), battleEventAchievement);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(key))
        {
            b.eventMarkersMap.EventMarkersFeed[key]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(key, 1);
        }
    }

    public override Texture2D GetEventIcon()
    {
        throw new NotImplementedException();
    }
}
