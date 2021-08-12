using System;
using System.Collections;
using UnityEngine;

public class Utility
{
    public static int Count(SaveSystem.SavedArrayObject toCount)
    {
        int count = 0;
        foreach (CharacterModelObject c in toCount.colonists)
        {
            if (c != null)
            {
                ++count;
            }
        }
        return count;
    }
}
