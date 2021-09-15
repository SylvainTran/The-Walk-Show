using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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
        if (gameController == null) gameController = FindObjectOfType<GameController>();
        if (actionFactory == null) actionFactory = new ActionFactory();
    }

    public void InvokeActionActorTarget(int actionActorTargetUUID)
    {
        if (actionActorTargetUUID == -1 || actionIndex == -1 || actionFactory == null) return;
        activeActionActorTargetUUID = actionActorTargetUUID;
        GameObject actionActorTarget = gameController.Colonists.Find(x => x.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == activeActionActorTargetUUID);
        if (actionActorTarget == null) return;

        GameObject hatTransform = null;

        // Invoke the action referred to by actionIndex
        if (actionActorTarget != null)
        {
            if (actionActorTarget.transform.GetChild(0))
            {
                hatTransform = actionActorTarget.transform.GetChild(0).gameObject; // hat
            }
            Debug.Log($"Invoking {actionIndex} on {actionActorTarget.GetComponent<CharacterModel>().NickName}");

            Func<GameObject> actionMethod = actionFactory.GetActionByIndex(actionIndex, actionActorTarget.transform.position);
            GameObject actionGameObject = actionMethod();
            // Spawn position
            if (actionGameObject.GetComponentInChildren<Snake>())
            {
                actionGameObject.transform.position = hatTransform.transform.position + new Vector3(UnityEngine.Random.Range(1.5f, 3.0f), 0.0f, UnityEngine.Random.Range(1.5f, 3.0f));
            }

            if (actionGameObject.GetComponentInChildren<Snake>() || actionGameObject.GetComponentInChildren<Zombie>()) return;
            // Setup - like freezing the actor temporarily
            Bot botRole = hatTransform.GetComponentInChildren<Bot>();
            if (botRole)
            {
                //botRole.FreezeAgent();
                //StartCoroutine(botRole.ResetAgentIsStopped(5.0f));
                actionActorTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            }
            // Disable animations
            Animator anim = actionActorTarget.GetComponent<Animator>();
            Utility.DisableAnimations(anim);

            Component[] components = { new Combatant(), new Bot(), new MainActor(), new SlayerHat(), new GraveDiggerHat(), new Dancer() };
            // We want to strip the actionGameObject without caring what component it has - we already determined it's the things we want
            foreach(Component t in components)
            {
                if (botRole.GetComponentInChildren(t.GetType()) != null)
                {
                    Destroy(hatTransform.GetComponentInChildren(t.GetType()).gameObject);
                    break;
                }
            }
            actionGameObject.transform.SetParent(hatTransform.transform);
            actionGameObject.transform.position = hatTransform.transform.position;
        }
        // Reset
        actionActorTargetUUID = -1;
        actionIndex = -1;
    }
}
