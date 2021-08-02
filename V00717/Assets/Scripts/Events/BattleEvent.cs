using UnityEngine;

public class BattleEvent : GameClockEvent
{
    private Enemy enemyEntity;
    private string endOfBattleMessage;

    public delegate void OnBattleEnded(BattleEvent b);
    public static event OnBattleEnded _OnBattleEnded;

    public BattleEvent(float triggerChance) : base(triggerChance)
    {
        this.endOfBattleMessage = "End of battle.";
    }

    public override bool ApplyEvent(BabyModel b)
    {
        if(!base.ApplyEvent(b))
        {
            return false;
        }
        Message = $"[Battle Event] {b.Name} is fighting an entity on the ship.";
        Enemy e = GenerateEntity();
        while(!CheckIfDead(b) && !CheckIfDead(e))
        {
            e.DealDamage(b);
            b.DealDamage(e);
        }        
        Message += $" The battle between {b.Name} and {e.CombatName()} is over.";
        if(b.Health > 0.0f)
        {
            Message += $" {b.Name} won! {e.CombatName()} was savagely killed.";
        } else
        {
            Message += $" {b.Name} has died in a gruesome way {e.CombatName()} will be laughing at {b.Name} hysterically for all eternity...";
        }
        // TODO rewards, notification pop-ups, etc. especially if colonist has died
        _OnBattleEnded(this);
        return true;
    }

    public Enemy GenerateEntity()
    {
        return new Enemy();
    }
}
