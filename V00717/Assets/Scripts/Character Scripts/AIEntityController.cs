using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SeasonController;
public class AIEntityController : MonoBehaviour
{
    public List<GameObject> humanPopulation;
    public int MAX_HUMAN_POPULATION = 5;
    public int MAX_ZOMBIE_POPULATION = 5;
    public int MAX_PREDATOR_POPULATION = 5;

    public List<GameObject> zombiePopulation;
    public List<GameObject> predatorPopulation;

    public GameObject humanPrefab;
    public GameObject zombiePrefab;
    public GameObject snakePrefab; // Predator
    public List<GameObject> predatorPrefabs;

    public GameWaypoint[] humanCityWaypoints;
    public GameWaypoint[] zombieWaypoints;
    public GameWaypoint[] predatorWaypoints;

    public float zombieRoll = 45;
    public float humanRoll = 65;
    public float predatorRoll = 35;

    /// <summary>
    /// Toolbelt buttons (generic / all uses)
    /// </summary>

    public GameObject toolBeltButton1;
    public GameObject toolBeltButton2;
    public GameObject toolBeltButton3;
    public GameObject toolBeltButton4;

    private void OnEnable()
    {
        //TimeController._OnUpdateEventClock += GenerateAIEntityContent;
    }

    private void OnDisable()
    {
        //TimeController._OnUpdateEventClock -= GenerateAIEntityContent;
    }

    public void Start()
    {
        predatorPrefabs.Add(snakePrefab);
        // DEBUG
        toolBeltButton3.GetComponent<Button>().onClick.AddListener(delegate { EntityFactory(ACTOR_ROLES.ZOMBIE); });
    }

    /// <summary>
    /// Fill world Content Tools (the generic random tool)
    /// Called from a button in the Content Creator Toolbelt.
    /// 
    /// </summary>
    public void GenerateAIEntityContent()
    {
        int roll = UnityEngine.Random.Range(0, 100);
        int count = UnityEngine.Random.Range(0, 10);

        if (zombiePopulation.Count < MAX_ZOMBIE_POPULATION && roll <= zombieRoll)
        {
            for(int i = 0; i < count; i++)
            {
                zombiePopulation.Add(EntityFactory(ACTOR_ROLES.ZOMBIE));
            }
        }
        roll = UnityEngine.Random.Range(0, 100);
        count = UnityEngine.Random.Range(0, 10);
        if (humanPopulation.Count < MAX_HUMAN_POPULATION && roll <= humanRoll)
        {
            for (int i = 0; i < count; i++)
            {
                humanPopulation.Add(EntityFactory(ACTOR_ROLES.HUMAN));
            }
        }
        roll = UnityEngine.Random.Range(0, 100);
        count = UnityEngine.Random.Range(0, 10);
        if (predatorPopulation.Count < MAX_PREDATOR_POPULATION && roll <= predatorRoll)
        {
            for (int i = 0; i < count; i++)
            {
                predatorPopulation.Add(EntityFactory(ACTOR_ROLES.PREDATOR));
            }
        }
    }

    /// <summary>
    /// Also called from a button in the Content Creator Toolbelt.
    /// </summary>
    public void DestroyAll()
    {
        humanPopulation.ForEach(h => Destroy(h.gameObject));
        zombiePopulation.ForEach(z => Destroy(z.gameObject));
        predatorPopulation.ForEach(p => Destroy(p.gameObject));
    }

    /// <summary>
    /// This is called via the Content Creator Tool menu... have to add event listeners to buttons with the specific role.
    /// Done from quadrant mode itself or drag and drop from "content belt"?
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public GameObject EntityFactory(ACTOR_ROLES role)
    {
        GameWaypoint spawnWaypoint;
        switch (role)
        {
            case ACTOR_ROLES.ZOMBIE:
                spawnWaypoint = zombieWaypoints[UnityEngine.Random.Range(0, zombieWaypoints.Length)];
                GameObject zombie = Instantiate(zombiePrefab, spawnWaypoint.transform);
                zombie.gameObject.name = "Zombie"; // Temp, the prefab will have name and tag setup already
                zombie.gameObject.tag = "Zombie";
                if(zombie.GetComponent<Zombie>() == null)
                {
                    zombie.AddComponent<Zombie>();
                }
                zombie.GetComponent<Zombie>().BehaviourSetup(spawnWaypoint);
                return zombie;
            case ACTOR_ROLES.HUMAN:
                GameObject human = Instantiate(humanPrefab, humanCityWaypoints[UnityEngine.Random.Range(0, humanCityWaypoints.Length)].transform);
                human.gameObject.name = "Human";
                human.gameObject.tag = "Human";
                return human;
            case ACTOR_ROLES.PREDATOR:
                GameObject predator = Instantiate(predatorPrefabs[UnityEngine.Random.Range(0, predatorPrefabs.Count)], predatorWaypoints[UnityEngine.Random.Range(0, predatorWaypoints.Length)].transform);
                predator.gameObject.name = "Predator";
                predator.gameObject.name = "Predator";
                return predator;
            default:
                break;
        }
        return null;
    }
}
