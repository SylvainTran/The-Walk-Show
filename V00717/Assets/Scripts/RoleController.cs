using UnityEngine;
using static SeasonController;

public class RoleController : MonoBehaviour
{
    public GameObject slayerHatPrefab;
    public GameObject vampireHatPrefab;
    public GameObject graveDiggerHatPrefab;
    public GameObject farmerHatPrefab;

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
            default:
                break;
        }
        return role;
    }
}
