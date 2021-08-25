using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointItem : MonoBehaviour
{
    /// <summary>
    /// The delay before destroying this item automatically
    /// </summary>
    public float destroyDelay = 30.0f;

    /// <summary>
    /// The amount of gold this waypoint item gives
    /// </summary>
    public int goldValue = 1;

    /// <summary>
    /// For items that can subtract or add health (e.g., predators or healing items)
    /// </summary>
    public int healthValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyWaypointItem(this.gameObject, destroyDelay));       
    }

    public IEnumerator DestroyWaypointItem(GameObject waypointItem, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(waypointItem.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("CharacterBot"))
        {
            other.GetComponent<CharacterModel>().GoldInventory += goldValue;
            other.GetComponent<CharacterModel>().Health += healthValue;
            if(this.gameObject)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
