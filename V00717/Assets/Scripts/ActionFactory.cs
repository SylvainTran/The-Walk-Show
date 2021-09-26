using System;
using UnityEngine;

public class ActionFactory
{
    public interface IToolbeltAction { public GameObject GetAction(); }
    class ContentSpawner
    {
        protected Vector3 areaOfAction;
        protected AIEntityController AIEntityController;

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
            AIEntityController = GameObject.FindObjectOfType<AIEntityController>();
        }
    }
    class SpawnPredator : ContentSpawner, IToolbeltAction
    {
        public SpawnPredator(Vector3 areaOfAction) : base(areaOfAction) {
        }
        public GameObject GetAction()
        {
            Debug.Log("Spawning a predator through the toolbelt!");
            return AIEntityController.EntityFactory(SeasonController.ACTOR_ROLES.PREDATOR, areaOfAction);
        }
    }

    class SpawnZombie : ContentSpawner, IToolbeltAction
    {
        public SpawnZombie(Vector3 areaOfAction) : base(areaOfAction)
        {
        }
        public GameObject GetAction()
        {
            Debug.Log("Spawning a zombie through the toolbelt!");
            return AIEntityController.EntityFactory(SeasonController.ACTOR_ROLES.ZOMBIE, areaOfAction);
        }
    }

    class SpawnSnow : ContentSpawner, IToolbeltAction
    {
        public SpawnSnow(Vector3 areaOfAction) : base(areaOfAction) { }
        public GameObject GetAction()
        {
            Debug.Log("Spawning snow through the toolbelt!");
            return AIEntityController.WeatherFactory(SeasonController.WEATHER_TYPES.SNOW, areaOfAction);
        }
    }

    class SpawnRain : ContentSpawner, IToolbeltAction
    {
        public SpawnRain(Vector3 areaOfAction) : base(areaOfAction) { }
        public GameObject GetAction()
        {
            Debug.Log("Spawning rain through the toolbelt!");
            return AIEntityController.WeatherFactory(SeasonController.WEATHER_TYPES.RAIN, areaOfAction);
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

    class Role : IToolbeltAction
    {
        RoleController RoleController;
        private int roleIndex;
        public Role(int roleIndex)
        {
            this.roleIndex = roleIndex;
            RoleController = GameObject.FindObjectOfType<RoleController>();
        }
        public GameObject GetAction()
        {
            Debug.Log("Assigning a role through the toolbelt!");
            return RoleController.RoleFactory((SeasonController.ACTOR_ROLES)roleIndex);
        }
    }

    public Func<GameObject> GetActionByIndex(int index, Vector3 areaOfAction = default)
    {
        if(index >= 0 && index <= 5)
        {
            return new Role(index).GetAction;
        }  
        switch (index)
        {
            case 6:
                return new SpawnPredator(areaOfAction).GetAction;
            case 7:
                return new SpawnZombie(areaOfAction).GetAction;
            case 8:
                return new SpawnSnow(areaOfAction).GetAction;
            case 9:
                return new SpawnRain(areaOfAction).GetAction;
            default:
                return new SpawnPredator(areaOfAction).GetAction;
        }
    }
}
