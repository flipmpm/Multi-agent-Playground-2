using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    private GameObject[] agentList;
    public int floorScale = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        agentList = GameObject.FindGameObjectsWithTag("Agent");

        // if there are no agents in the scene, spawn one at a random location. 
        // This is to ensure that there is always at least one agent in the scene.
        if (agentList.Length < 1)
        {
            SpawnCreature();
            SpawnCreature();
            SpawnCreature();
            SpawnCreature();

        }
        else if (agentList.Length <= 2)
        {
            SpawnCreature();

        }
    }

    void SpawnCreature()
    {
        int x = Random.Range(-80, 81)*floorScale;
        int z = Random.Range(-80, 81)*floorScale;
        Instantiate(agentPrefab, new Vector3((float)x, 0.75f, (float)z), Quaternion.identity);
    }
}
