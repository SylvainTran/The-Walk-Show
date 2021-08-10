using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Generates relevant dialogues based
/// on Character Major's personality,
/// moral level, and stress level.
///
/// </summary>
public class ChatGenerator
{
    private BabyModel target = null;
    private ChatDatabase chatDatabase = null;
    public List<string> chats;

    public float lowThreshold = 25.0f;

    public ChatGenerator(BabyModel target, ChatDatabase chatDatabase)
    {
        this.target = target;
        this.chatDatabase = chatDatabase;
        this.chats = new List<string>();
    }

    public Dictionary<string, int> ProfileTarget()
    {
        // Check live status, stress, moral, some personality traits
        int stressWeight = 0;
        if (target.Health <= lowThreshold)
        {
            ++stressWeight;
        }

        if (target.StressTolerance <= lowThreshold)
        {
            ++stressWeight;
        }

        if (target.MoralLevel <= lowThreshold)
        {
            ++stressWeight;
        }

        Dictionary<string, int> profileWeights = new Dictionary<string, int>();
        // For all the events that are achieved, add to profile?
        foreach (KeyValuePair<string, int> kvp in target.eventMarkersMap.EventMarkersFeed)
        {
            profileWeights.Add(kvp.Key, kvp.Value);
        }
        profileWeights.Add(Enums.ToString(Enums.ChatThemes.STRESS_THEME), stressWeight);
        return profileWeights;
    }

    public string GetDialogueTheme()
    {
        Dictionary<string, int> profileWeights = ProfileTarget();
        // TODO some weighting algorithm to be applied?
        // is it even worth it? How to factor morale in the equation?
        string key = profileWeights.Max().Key;
        return key;
    }

    /// <summary>
    /// Gets the string dialogue by weighting the character's current stress and events gone through
    /// </summary>
    /// <returns></returns>
    public string GetDialogueTextByTheme()
    {
        if (chatDatabase == null)
        {
            return null;
        }
        string dialogueTheme = GetDialogueTheme();
        int randInd = 0;
        int randDialogue = 0;
        if (dialogueTheme.Equals(Enums.ToString(Enums.ChatThemes.STRESS_THEME)))
        {
            randInd = UnityEngine.Random.Range(0, chatDatabase.STRESS_THEME.Length);
            return chatDatabase.STRESS_THEME[randInd];
        }
        else
        {
            randInd = UnityEngine.Random.Range(0, 6);
            string theme = null;
            switch (randInd)
            {
                case 1:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.STRESS_THEME.Length);
                    theme = chatDatabase.STRESS_THEME[randDialogue];
                    break;
                case 2:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.REGRET_THEME.Length);
                    theme = chatDatabase.REGRET_THEME[randDialogue];
                    break;
                case 3:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.REALIZATION_THEME.Length);
                    theme = chatDatabase.REALIZATION_THEME[randDialogue];
                    break;
                case 4:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.GUILT_THEME.Length);
                    theme = chatDatabase.GUILT_THEME[randDialogue];
                    break;
                case 5:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.FEAR_THEME.Length);
                    theme = chatDatabase.FEAR_THEME[randDialogue];
                    break;
                default:
                    randDialogue = UnityEngine.Random.Range(0, chatDatabase.STRESS_THEME.Length);
                    theme = chatDatabase.STRESS_THEME[randDialogue];
                    break;
            }
            return theme;
        }
    }

}
