using UnityEngine;

public class TombstoneSpawner : MonoBehaviour
{
    [Range(2.5f, 100.0f)]
    [Header("The range for which the bot will decide to check out a dead participant")]
    [Tooltip("Doesn't make sense if too far, probably")]
    public float tombstoneViewBehaviourRange = 5.0f;

    [Range(1.5f, 3.0f)]
    [Header("The scalar to transform multiply the local scale of the dead participant")]
    [Tooltip("Doesn't make sense if too large or too small, probably")]
    public float tombstoneScalingSize = 2.5f;

    private CharacterCreationView CharacterCreationView;

    private void OnEnable()
    {
        //Bot._OnMainActorIsDead += SpawnTombstone;
    }

    private void OnDisable()
    {
        //Bot._OnMainActorIsDead -= SpawnTombstone;
    }

    private void Start()
    {
        CharacterCreationView = GameObject.FindObjectOfType<CharacterCreationView>();
    }

    public void SpawnTombstone(GameObject go)
    {
        // TODO define equals method to compare UUIDs by default and other things
        if(go.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ == go.GetComponent<CharacterModel>().UniqueColonistPersonnelID_)
        {
            SpawnMeshAtLocation(go);
        } else
        {
            // Go check out the tombstone and react - if within appropriate range
            DecideToSeekTombstone(go);
        }
    }

    public void DecideToSeekTombstone(GameObject go)
    {
        if (Vector3.Distance(transform.position, go.transform.position) <= tombstoneViewBehaviourRange)
        {
            GetComponent<Bot>().ViewTombstoneBehaviour(go);
        }
    }

    public void SpawnMeshAtLocation(GameObject go)
    {
        this.transform.localScale *= tombstoneScalingSize;
        this.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        //CharacterCreationView.UpdateSkinColor(go);
    }
}
