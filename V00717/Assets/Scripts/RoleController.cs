using UnityEngine;
using static SeasonController;

public class RoleController : MonoBehaviour
{
    public GameObject slayerHatPrefab;
    public GameObject vampireHatPrefab;
    public GameObject graveDiggerHatPrefab;
    public GameObject farmerHatPrefab;
    public GameObject mainActorHatPrefab;
    public GameObject dancerHatPrefab;

    public GameObject RoleFactory(ACTOR_ROLES roleIndex)
    {
        GameObject role = null;
        GameObject t = new GameObject();
        switch (roleIndex)
        {
            case ACTOR_ROLES.VAMPIRE:
                break;
            case ACTOR_ROLES.GRAVEDIGGER:
                Debug.Log("Assgning a grave digger role through toolbelt!");
                return Instantiate(graveDiggerHatPrefab, t.transform, true);
            case ACTOR_ROLES.SLAYER:
                Debug.Log("Assgning a slayer role through toolbelt!");
                return Instantiate(slayerHatPrefab, t.transform, true);
            case ACTOR_ROLES.FARMER:
                break;
            case ACTOR_ROLES.HUMAN:
                Debug.Log("Assgning a main actor role through toolbelt!");
                return Instantiate(mainActorHatPrefab, t.transform, true);
                break;
            case ACTOR_ROLES.DANCER:
                Debug.Log("Assgning a dancer role through toolbelt!");
                return Instantiate(dancerHatPrefab, t.transform, true);
                break;
            default:
                break;
        }
        return role;
    }
}
