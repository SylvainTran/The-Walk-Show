using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer
{
    protected string name;
    public string Name { get { return name; } set { name = value; } }

    private float subscribeChance;
    private float donationChance;
    private int donationAmount;

    public delegate void NewSubscriberAction(string name);
    public static event NewSubscriberAction _OnNewSubscriberAction;

    public delegate void NewDonationAction(string donatorName, int donationAmount);
    public static event NewDonationAction _OnNewDonationAction;

    public Viewer(string name, float subscribeChance)
    {
        this.name = name;
        this.subscribeChance = subscribeChance;
        this.donationChance = UnityEngine.Random.Range(0, 100); // TODO decide if scale with actual game design parameters

        int roll = UnityEngine.Random.Range(0, 100);
        int donationAmount = UnityEngine.Random.Range(1, 100); // TODO scale with actual game design parameters

        if(roll >= subscribeChance)
        {
            new Subscriber(name);
            // Notification pops up
            _OnNewSubscriberAction(name);
        }

        if(roll >= donationChance)
        {
            _OnNewDonationAction(name, donationAmount);
        }
    }
}
