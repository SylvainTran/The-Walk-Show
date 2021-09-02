using System;
using UnityEngine;

public class ActionFactory
{
    public interface IToolbeltAction { public GameObject GetAction(); }
    class ContentSpawner
    {
        protected Vector3 areaOfAction;

        public ContentSpawner(Vector3 areaOfAction)
        {
            if (areaOfAction != null)
            {
                this.areaOfAction = areaOfAction;
            }
            else
            {
                // Might not have an area of action, in that case it's a "no position" action
                this.areaOfAction = default;
            }
        }
    }
    class SpawnPredator : ContentSpawner, IToolbeltAction
    {
        AIEntityController AIEntityController;

        public SpawnPredator(Vector3 areaOfAction) : base(areaOfAction) {
            AIEntityController = GameObject.FindObjectOfType<AIEntityController>();
        }
        public GameObject GetAction()
        {
            Debug.Log("Spawning a predator through the toolbelt!");
            return AIEntityController.EntityFactory(SeasonController.ACTOR_ROLES.PREDATOR, areaOfAction);
        }
    }

    class SpawnGift : ContentSpawner, IToolbeltAction
    {
        public SpawnGift(Vector3 areaOfAction) : base(areaOfAction) { }
        public GameObject GetAction()
        {
            Debug.Log("Spawning a gift through the toolbelt!");
            throw new NotImplementedException();
        }
    }

    public Func<GameObject> GetActionByIndex(int index, Vector3 areaOfAction = default)
    {
        switch (index)
        {
            case 0:
                return new SpawnPredator(areaOfAction).GetAction;
                break;
            case 1:
                return new SpawnGift(areaOfAction).GetAction;
                break;
            default:
                return new SpawnGift(areaOfAction).GetAction;
                break;
        }
    }
}
