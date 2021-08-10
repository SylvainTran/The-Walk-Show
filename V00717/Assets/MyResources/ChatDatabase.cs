using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "ChatDatabase", menuName = "ScriptableObjects/ChatDatabase", order = 5)]
public class ChatDatabase : ScriptableObject
{
    public string[] STRESS_THEME;
    public string[] REGRET_THEME;
    public string[] REALIZATION_THEME;
    public string[] DENIAL_THEME;
    public string[] GUILT_THEME;
    public string[] FEAR_THEME;
}
