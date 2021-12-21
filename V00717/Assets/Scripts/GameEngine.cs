using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    private static GameEngine Instance;
    private static List<LogObserver> logObservers;
    public List<LogObserver> LogObserver { get => logObservers; set => logObservers = value; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            Instance = this;
            logObservers = new List<LogObserver>();
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public static GameEngine GetGameEngine()
    {
        return Instance;
    }
    public void AddLogObserver(LogObserver o)
    {
        logObservers.Add(o);
    }
}
