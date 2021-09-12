using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = GetComponent<Terrain>().terrainData;

        for(int i =0; i < terrainData.alphamapWidth; i++)
        {
            for(int j = 0; j < terrainData.alphamapHeight; j++)
            {


            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
