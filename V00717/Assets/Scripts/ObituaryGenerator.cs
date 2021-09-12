using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObituaryGenerator
{
    // Generates a one liner question reflecting the character's most weighted achievements?
    private float HEALTH_EVENT_WEIGHT = 0.3f;
    private float BATTLE_EVENT_WEIGHT = 0.5f;

    // The model to act upon
    private CharacterModel target = null;

    public ObituaryGenerator(CharacterModel target)
    {
        this.target = target;
    }

    // Uses the achievement/frequency map to anchor a main event, and the rest is plausible randomness
    public string GenerateEventFrequencyText()
    {
        if(target.eventMarkersMap.EventMarkersFeed == null || target.eventMarkersMap.EventMarkersFeed.Count == 0)
        {
            return null;
        }
        string sortedEventsByCount = null;
        foreach (KeyValuePair<string, int> achievement in target.eventMarkersMap.EventMarkersFeed.OrderByDescending(key => key.Value))
        {
            sortedEventsByCount += $"Got {achievement.Key} {achievement.Value} times.\n";
        }
        return sortedEventsByCount;
    }

    // Basic cut up technique
    public string GenerateMajorEventText()
    {
        if(target.eventMarkersMap.EventMarkersFeed.Count == 0 || target.eventMarkersMap.EventMarkersFeed == null)
        {
            Debug.LogError("Event markers map should not be null or empty.");
            return null;
        }
            
        string maxFrequency = target.eventMarkersMap.EventMarkersFeed.Aggregate((accumulator, current) => accumulator.Value > current.Value ? accumulator : current).Key;
        int highestFrequencyEvent = target.eventMarkersMap.EventMarkersFeed[maxFrequency];

        string[] verbs = { "got", "managed", "catched", "lucked"};
        string[] prepositions = { "into" };
        string[] determiners = { "a" };

        int rVerb = UnityEngine.Random.Range(0, verbs.Length);
        int rPreposition = UnityEngine.Random.Range(0, prepositions.Length);
        int rDeterminer = UnityEngine.Random.Range(0, determiners.Length);

        string gender = target.Sex == "Male" ? "He" : "Her"; // They?
        string gender2 = target.Sex == "Male" ? "He" : "She"; // 
        string gender3 = target.Sex == "Male" ? "His" : "Her"; // 

        // Using human English skills to generate highly entropic ideas
        string[] WAS_BORN_VARIANTS =
            {
                "For some, being born is the highest achievement.",
                $"The birth of mankind was echoed in {gender.ToLower()} birth.",
                $"The world was forever changed after {gender2.ToLower()} was born."
            };
        string[] GOT_DISEASE_VARIANTS =
            {
                $"Overcoming disease was {gender3.ToLower()} lot in life.",
                $"In the end, {gender2.ToLower()} would have defeated many diseases save death.",
            };
        string[] GOT_BATTLE_VARIANTS =
        {
            $"{gender2} lived as a warrior and died as a warrior.",
            $"{gender3} valor in combat earned them valor and praise from all other colonists."
        };

        string[] GOT_INJURY_VARIANTS =
        {
            $"{gender2} survived injuries until the end.",
            $"{gender3} scars proved their mettle to all the other colonists."
        };

        string[] feed = null;

        switch (maxFrequency)
        {
            case "WAS_BORN":
            {
                feed = WAS_BORN_VARIANTS;
                break;
            }
            case "GOT_DISEASE":
            {
                feed = GOT_DISEASE_VARIANTS;
                break;
            }
            case "GOT_INJURY":
            {
                feed = GOT_INJURY_VARIANTS;
                break;
            }
            case "GOT_BATTLE":
            {
                feed = GOT_BATTLE_VARIANTS;
                break;
            }
            default:
                break;
        }
        string mainAchievement = null;
        if (feed != null)
        {
            mainAchievement = feed[UnityEngine.Random.Range(0, feed.Length)];
        }
        return mainAchievement;
    }

    // Basic AI technique?
    public void GenerateMajorEventTextAI()
    {

    }
}
