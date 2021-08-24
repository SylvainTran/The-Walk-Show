using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subscriber
{
    private string name;
    public string Name { get { return name; } set { name = value; } }
    private float requestActionChance = 15.0f;
    public delegate void NewSubscriberRequestAction(string subscriberName, string message);
    public static event NewSubscriberRequestAction _OnNewSubscriberRequestAction;
    public Subscriber(string name)
    {
        this.name = name;

        int roll = UnityEngine.Random.Range(0, 100);

        if (roll <= requestActionChance)
        {
            new Subscriber(name);
            // Notification pops up
            _OnNewSubscriberRequestAction(name, $"Hi, I'm {name}. Nice channel.");
        }
    }
}
