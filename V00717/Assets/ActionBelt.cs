using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionBelt : MonoBehaviour
{
    private GameController gameController;
    /// <summary>
    /// The camera lane's actor being active. Set by the DraggedActionHandler
    /// </summary>
    private int activeActionActorTargetUUID = -1;
    public int ActiveActionActorTargetUUID { get { return activeActionActorTargetUUID; } set { activeActionActorTargetUUID = value; } }
    private int actionIndex = -1;
    public int ActionIndex { get { return actionIndex; }  set { actionIndex = value; } }

    private ActionFactory actionFactory; 

    private void Awake()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
        }
        if (actionFactory == null)
        {
            actionFactory = new ActionFactory();
        }
    }

    public void InvokeActionActorTarget(int actionActorTargetUUID)
    {
        if(actionActorTargetUUID == -1 || actionIndex == -1 || actionFactory == null)
        {
            return;
        }
        activeActionActorTargetUUID = actionActorTargetUUID;
        GameObject actionActorTarget = gameController.Colonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == activeActionActorTargetUUID);
        // Invoke the action referred to by actionIndex
        if (actionActorTarget != null)
        {
            Debug.Log($"Invoking {actionIndex} on {actionActorTarget.gameObject.GetComponent<CharacterModel>().NickName}");

            Func<GameObject> actionMethod = actionFactory.GetActionByIndex(actionIndex, actionActorTarget.transform.position);
            GameObject actionGameObject = actionMethod();
            // Setup - like freezing the actor temporarily
            actionActorTarget.GetComponent<Bot>().FreezeAgent();
            StartCoroutine(actionActorTarget.GetComponent<Bot>().ResetAgentIsStopped(10.0f));

            SlayerHat hat;
            actionGameObject.TryGetComponent<SlayerHat>(out hat);
            if(hat != null)
            {
                if(actionActorTarget.GetComponent<Slayer>() == null)
                {
                    if(actionActorTarget.GetComponent<MainActor>())
                    {
                        Destroy(actionActorTarget.GetComponent<MainActor>());
                    }
                    actionActorTarget.AddComponent<Slayer>();
                    Debug.Log($"{actionActorTarget.GetComponent<CharacterModel>().NickName} wears the hat of a slayer");
                }
                actionGameObject.transform.SetParent(actionActorTarget.transform.GetChild(0));
            }
            actionGameObject.transform.position = actionActorTarget.transform.GetChild(0).transform.position;
        }

    }

}
