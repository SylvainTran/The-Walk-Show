using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulingWindow : MonoBehaviour
{
    void Awake()
    {
        GameEngine.SchedulingWindow = GameObject.FindObjectOfType<SchedulingWindow>().gameObject;
        GameEngine.SchedulingWindow.SetActive(false);
    }

    private void OnEnable()
    {
        GameEngine.GetGameEngine().Scheduling();
    }
}
