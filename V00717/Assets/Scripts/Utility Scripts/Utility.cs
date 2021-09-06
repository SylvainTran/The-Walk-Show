using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static void DisableAnimations(Animator animator)
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(animator.parameters[i].name, false);
            }
        }
    }

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
