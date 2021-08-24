using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelController : MonoBehaviour
{
    public List<Viewer> viewers;
    public List<Subscriber> subscribers;
    // STRETCH: public List<Patron> patrons;
    // List of random names
    public GameController GameController;
    // The max amount of viewers per tick that can be created
    public int addViewerTickMax = 10;
    // The max amount of viewers and subscribers (not needed to be big)
    public int MAX_VIEWERS = 1000;
    public int MAX_SUBSCRIBERS = 1000;
    public float SPAWN_DELAY = 5.0f;
    // Corpus json TODO

    // Chance that a viewer converts to a subber
    public float subscribeChance;
    public Coroutine GenerateRandomViewersCoroutine;

    private void Start()
    {
        viewers = new List<Viewer>(MAX_VIEWERS);
        subscribers = new List<Subscriber>(MAX_SUBSCRIBERS);
        // STRETCH: patrons = new List<Patron>();
    }

    public IEnumerator GenerateRandomViewers(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (viewers.Count > MAX_VIEWERS || GameController == null || GameController.randomizedAuditionDatabase.actors == null || GameController.randomizedAuditionDatabase.actors.Length == 0)
        {
            yield return null;
        }
        int total = UnityEngine.Random.Range(0, addViewerTickMax);

        for(int i = 0; i < total; i++)
        {
            if(viewers.Count < MAX_VIEWERS)
            {
                string newName = GameController.randomizedAuditionDatabase.actors[UnityEngine.Random.Range(0, 1000)].name;
                viewers.Add(new Viewer(newName, subscribeChance));
                Debug.Log($"New viewer: {newName}");
            }
            else
            {
                break;
            }
        }
        StartCoroutine(GenerateRandomViewers(UnityEngine.Random.Range(0, SPAWN_DELAY)));
    }

    public GameClockEventReaction GenerateReaction(GameClockEvent e)
    {
        if(viewers.Count == 0)
        {
            return null;
        }

        Viewer randomViewer = viewers[UnityEngine.Random.Range(0, viewers.Count)];
        Subscriber randomSubscriber;

        if (subscribers.Count == 0)
        {
            randomSubscriber = null;
        }
        else
        {
            randomSubscriber = subscribers[UnityEngine.Random.Range(0, subscribers.Count)];
        }
        string emoji = GameController.cuteKaomojiDatabase.cuteKaomoji[Random.Range(0, GameController.cuteKaomojiDatabase.cuteKaomoji.Length)];
        string interjection = GameController.interjectionDatabase.interjections[Random.Range(0, GameController.interjectionDatabase.interjections.Length)];
        string expletive = GameController.expletivesDatabase.expletives[Random.Range(0, GameController.expletivesDatabase.expletives.Length)];
        string encouragement = GameController.encouragementDatabase.encouragements[Random.Range(0, GameController.encouragementDatabase.encouragements.Length)];
        string message = $"{randomViewer.Name}: {interjection} {expletive} {encouragement} {emoji}";

        if (randomSubscriber != null)
        {
            // Chance for notification or supporter special request 
        }

        return new GameClockEventReaction(message, randomViewer, randomSubscriber);
    }
}
