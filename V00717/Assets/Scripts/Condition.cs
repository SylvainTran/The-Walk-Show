using System;

// The conditions a colonist may contract
public class Condition
{
    private string name = null;
    private int damagePerTick = 0;
    private int boonPerTick = 0;
    private int durationLeft = 5; // Main cycle or event ticks

    public Condition(string name, int damagePerTick, int boonPerTick, int durationLeft)
    {
        this.name = name;
        this.damagePerTick = damagePerTick;
        this.boonPerTick = boonPerTick;
        this.durationLeft = durationLeft;
    }
}
