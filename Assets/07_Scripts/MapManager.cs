using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject[] decoObjects;
    [SerializeField] Terrain mapTerrain;
    [SerializeField] int count;
    [SerializeField] float margin;


    void Start()
    {
        float terrainWidth = mapTerrain.terrainData.size.x;
        float terrainLength = mapTerrain.terrainData.size.z;

        for(int i=0; i<count; i++)
        {
            float x = Random.Range(margin, terrainWidth - margin);
            float z = Random.Range(margin, terrainLength - margin);

            float y = mapTerrain.SampleHeight(new Vector3(x, 0, z));

            Vector3 position = new Vector3(x, y, z) + mapTerrain.transform.position;

            int random = Random.Range(0, decoObjects.Length);
            Instantiate(decoObjects[random], decoObjects[random].transform.position + position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        
    }
}
