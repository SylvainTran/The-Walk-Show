using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
    }

    public SplatHeights[] splatHeights;

    public void PaintTerrain()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        // Setting the alpha map height
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            // Setting the alpha map width
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);
                float[] splat = new float[splatHeights.Length];

                // Use the height at y,x to decide if layer opacity should be 1 (max)
                for (int i = 0; i < splatHeights.Length; i++)
                {
                    bool paint = terrainHeight >= splatHeights[i].startingHeight;
                    if (paint)
                    {
                        if (i == splatHeights.Length - 1)
                            splat[i] = 1;
                        else if (terrainHeight <= splatHeights[i + 1].startingHeight)
                            splat[i] = 1;
                    }
                }
                // Setting the alpha map layer
                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatMapData[x, y, j] = splat[j];
                }
            }
        }
        // Set the map
        terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    // Start is called before the first frame update
    void Start()
    {
        PaintTerrain();
    }
}
