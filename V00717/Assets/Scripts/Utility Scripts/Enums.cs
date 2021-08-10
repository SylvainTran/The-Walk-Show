using System;
public class Enums
{
    public enum Genders { MALE, FEMALE };
    public enum DashboardPageIndexes { LOGIN, DESKTOP, DATABASE };
    public enum DataRequests { LIVE_COLONISTS, DEAD_COLONISTS };
    public enum CharacterAchievements { WAS_BORN, GOT_DISEASE, GOT_BATTLE, GOT_INJURY, GOT_REPAIR_SUCCESS, GOT_REPAIR_FAILURE, PENDING_CALLS };
    public enum Sentiments { STRESSED, FEARFUL, ANGRY, SAD, JOYFUL };
    public enum ChatThemes { STRESS_THEME, REGRET_THEME, REALIZATION_THEME, DENIAL_THEME, GUILT_THEME, FEAR_THEME };

    public static string ToString(Enums.ChatThemes c)
    {
        string key = null;
        var chatThemes = typeof(ChatThemes);

        switch (c)
        {
            case ChatThemes.STRESS_THEME:
                key = Enum.GetName(chatThemes, 0);
                break;
            case ChatThemes.REGRET_THEME:
                key = Enum.GetName(chatThemes, 1);
                break;
            case ChatThemes.REALIZATION_THEME:
                key = Enum.GetName(chatThemes, 2);
                break;
            case ChatThemes.DENIAL_THEME:
                key = Enum.GetName(chatThemes, 3);
                break;
            case ChatThemes.GUILT_THEME:
                key = Enum.GetName(chatThemes, 4);
                break;
            case ChatThemes.FEAR_THEME:
                key = Enum.GetName(chatThemes, 5);
                break;
            default:
                break;
        }
        return key;
    }

    public static string ToString(Enums.Sentiments s)
    {
        string key = null;
        var sentiments = typeof(Sentiments);

        switch (s)
        {
            case Sentiments.STRESSED:
                key = Enum.GetName(sentiments, 0);
                break;
            case Sentiments.FEARFUL:
                key = Enum.GetName(sentiments, 0);
                break;
            case Sentiments.ANGRY:
                key = Enum.GetName(sentiments, 0);
                break;
            case Sentiments.SAD:
                key = Enum.GetName(sentiments, 0);
                break;
            case Sentiments.JOYFUL:
                key = Enum.GetName(sentiments, 0);
                break;
            default:
                break;
        }
        return key;
    }

    public static string ToString(Enums.CharacterAchievements e)
    {
        string key = null;
        var characterAchievement = typeof(CharacterAchievements);
        switch (e)
        {
            case CharacterAchievements.WAS_BORN:
                key = Enum.GetName(characterAchievement, 0);
                break;
            case CharacterAchievements.GOT_DISEASE:
                key = Enum.GetName(characterAchievement, 1);
                break;
            case CharacterAchievements.GOT_BATTLE:
                key = Enum.GetName(characterAchievement, 2);
                break;
            case CharacterAchievements.GOT_INJURY:
                key = Enum.GetName(characterAchievement, 3);
                break;
            case CharacterAchievements.GOT_REPAIR_SUCCESS:
                key = Enum.GetName(characterAchievement, 4);
                break;
            case CharacterAchievements.GOT_REPAIR_FAILURE:
                key = Enum.GetName(characterAchievement, 5);
                break;
            case CharacterAchievements.PENDING_CALLS:
                key = Enum.GetName(characterAchievement, 6);
                break;
            default:
                break;
        }
        return key;
    }
}
