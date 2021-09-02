using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    public GameObject EntityFactory(ACTOR_ROLES role, Vector3 spawnLocation = default)
    {
        // The default spawn location is a spawn way point, but can have one provided by the action factory.
        GameWaypoint spawnWaypoint = null;
        GameObject spawn = null;
        switch (role)
        {
            case ACTOR_ROLES.ZOMBIE:
                spawnWaypoint = zombieWaypoints[Random.Range(0, zombieWaypoints.Length)];
                spawn = Instantiate(zombiePrefab, spawnWaypoint.transform);
                spawn.gameObject.name = "Zombie"; // Temp, the prefab will have name and tag setup already
                spawn.gameObject.tag = "Zombie";

                if(spawn.GetComponent<Zombie>() == null)
                {
                    spawn.AddComponent<Zombie>();
                }
                spawn.GetComponent<Zombie>().BehaviourSetup(spawnWaypoint);
                break;
            case ACTOR_ROLES.HUMAN:
                spawn = Instantiate(humanPrefab, humanCityWaypoints[Random.Range(0, humanCityWaypoints.Length)].transform);
                spawn.gameObject.name = "Human";
                spawn.gameObject.tag = "Human";
                break;
            case ACTOR_ROLES.PREDATOR:
                spawn = Instantiate(predatorPrefabs[Random.Range(0, predatorPrefabs.Count)], predatorWaypoints[Random.Range(0, predatorWaypoints.Length)].transform);
                spawn.gameObject.name = "Predator";
                spawn.gameObject.tag = "Predator";
                break;
            default:
                break;
        }
        // The action tool belt's factory specifies a spawnLocation
        if (spawnLocation.magnitude >= 0)
        {
            //StartCoroutine(ResetAgent(spawn, spawnLocation));
            // Find closest point to spawnLocation actor
            spawn.transform.position = spawnLocation;
            spawn.GetComponent<NavMeshAgent>().Warp(spawnLocation);
        }
        return spawn;
    }
    public GameObject spawn = null;
    public IEnumerator ResetAgent(GameObject _spawn, Vector3 spawnLocation)
    {
        spawn = _spawn;
        // parachute effect
        // spawn.transform.position = spawnLocation + new Vector3(0.0f, 20.0f, 0.0f);
        // Physics version
        //spawn.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0.0f, 40.0f, 0.0f));

        // Kinematic version
        //Vector3 closestGroundPoint = Vector3.zero;
        //if (closestGroundPoint.magnitude <= 0.0f)
        //{
        //    RaycastHit? ground = FindClosestGround();
        //    // It can be null so we assign a dump in case and check its collider
        //    RaycastHit _ground = ground ?? new RaycastHit();
        //    closestGroundPoint = _ground.point;

        //    if (_ground.collider != null)
        //    {
        //        // closestGroundPoint = _ground.collider.ClosestPoint(spawn.transform.position);
        //        //spawn.transform.position = closestGroundPoint;
        //    }
        //}

        //while (spawn.transform.position.y >= closestGroundPoint.y - spawn.transform.localScale.y * 1.25f)
        //{
        //    spawn.transform.Translate(Vector3.down * 1.2f * Time.deltaTime, Space.World);
        //    spawn.transform.Rotate(Vector3.forward, 1.5f * Time.deltaTime);
        //}

        yield return new WaitUntil(IsGrounded);
        //StopAllCoroutines();
        //if (!IsGroundedOnNavmesh())
        //{
        //    closestGroundPoint = FindNearestEdgeNavmesh();
        //}
        //spawn.transform.position = closestGroundPoint;
        //spawn.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        // spawn.GetComponent<NavMeshAgent>().Warp(spawnLocation);
    }

    /// <summary>
    /// Straight line down
    /// </summary>
    /// <returns></returns>
    public RaycastHit? FindClosestGround()
    {
        Debug.Log("Finding closest ground...");
        RaycastHit[] hits = Physics.RaycastAll(spawn.transform.position, transform.TransformDirection(Vector3.down), Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                Debug.Log("Found closest ground.");
                return hit;
            }
        }
        return null;
    }

    public bool IsGrounded()
    {
        Debug.Log("Checking if grounded...");
        RaycastHit? ground = FindClosestGround();
        RaycastHit _ground = ground ?? new RaycastHit();

        if(_ground.collider == null)
        {
            return false;
        }
        float dist = Vector3.Distance(spawn.transform.position, _ground.collider.gameObject.transform.position);
        if (dist <= 1.0f)
        {
            Debug.Log("GROUNDED");
            return true;
        }
        return false;
    }

    public bool IsGroundedOnNavmesh()
    {
        NavMeshHit navHit;
        if (NavMesh.FindClosestEdge(spawn.transform.position, out navHit, NavMesh.AllAreas))
        {
            Debug.Log("Grounded on navmesh.");
            return navHit.distance <= 1.0f;
        }
        Debug.Log("Not grounded on navmesh.");
        return false;
    }

    public Vector3 FindNearestEdgeNavmesh()
    {
        Debug.Log("Finding nearest edge on navmesh.");
        NavMeshHit navHit;
        if (NavMesh.FindClosestEdge(spawn.transform.position, out navHit, NavMesh.AllAreas))
        {
            Debug.Log("Found closest point on navmesh.");
            return navHit.position;
        }
        return navHit.position;
    }
}
