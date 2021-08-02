using System.Collections;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    // The delay between clock event ticks
    public float eventTickDelay = 3.0f;
    // Publisher event for IEventClockTickSubscribers
    public delegate void UpdateEventClock();
    public static event UpdateEventClock _OnUpdateEventClock;

    // Starts the event clock
    private void Start()
    {
        StartCoroutine("UpdateEventTick");
    }

    // Notifies subscribers (BabyController.cs's colonists list)
    private IEnumerator UpdateEventTick()
    {
        yield return new WaitForSeconds(eventTickDelay);
        _OnUpdateEventClock();
        StartCoroutine("UpdateEventTick");
    }
}
