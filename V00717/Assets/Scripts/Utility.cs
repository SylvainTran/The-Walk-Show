using System;
using System.Collections;

public class Utility
{
    public Utility()
    {
    }

    public static int Count(SaveSystem.SavedArrayObject toCount)
    {
        int count = 0;
        foreach (BabyModel c in toCount.colonists)
        {
            if (c != null)
            {
                ++count;
            }
        }
        return count;
    }

    internal static int Count(BabyModel[] toCount)
    {
        int count = 0;
        foreach (BabyModel c in toCount)
        {
            if (c != null)
            {
                ++count;
            }
        }
        return count;
    }
}
