using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void OnEnable()
    {
        TimeController._OnUpdateEventClock += GenerateAIEntityContent;
    }

    private void OnDisable()
    {
        TimeController._OnUpdateEventClock -= GenerateAIEntityContent;
    }

    public void Start()
    {
        predatorPrefabs.Add(snakePrefab);
    }

    /// <summary>
    /// Fill world Content Tools (the generic random tool)
    /// </summary>
    public void GenerateAIEntityContent()
    {
        int roll = UnityEngine.Random.Range(0, 100);
        int count = UnityEngine.Random.Range(0, 10);

        if (zombiePopulation.Count < MAX_ZOMBIE_POPULATION &&  roll <= zombieRoll)
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
    /// This is called via the Content Creator Tool menu... have to add event listeners to buttons with the specific role.
    /// Done from quadrant mode itself or drag and drop from "content belt"?
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public GameObject EntityFactory(ACTOR_ROLES role)
    {
        switch (role)
        {
            case ACTOR_ROLES.ZOMBIE:
                    return Instantiate(zombiePrefab, zombieWaypoints[UnityEngine.Random.Range(0, zombieWaypoints.Length)].transform);
            case ACTOR_ROLES.HUMAN:
                   return  Instantiate(humanPrefab, humanCityWaypoints[UnityEngine.Random.Range(0, humanCityWaypoints.Length)].transform);
            case ACTOR_ROLES.PREDATOR:
                return Instantiate(predatorPrefabs[UnityEngine.Random.Range(0, predatorPrefabs.Count)], predatorWaypoints[UnityEngine.Random.Range(0, predatorWaypoints.Length)].transform);
            default:
                break;
        }
        return null;
    }
}
