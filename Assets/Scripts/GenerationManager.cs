using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GenerationManager : MonoBehaviour
{
    public Text saveButtonText;
    public Text importButtonText;
    public string saveFileName = "generation_data.json";
    public GameObject agentPrefab; // Make sure to assign the correct agent prefab here

    List<AgentDataToSave> savedAgentData;


    void Update()
    {
        // Save Generation when F1 key is pressed
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveGeneration();
        }

        // Import Progress when F2 key is pressed
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ImportProgress();
        }
    }

    void SaveGeneration()
    {
        saveButtonText.text = "Saving Generation...";

        // Find all AgentData components
        AgentData[] allAgentData = FindObjectsOfType<AgentData>();

        // Create a list to hold the serialized data
        List<AgentDataToSave> agentDataList = new List<AgentDataToSave>();

        // Serialize data from each AgentData component
        foreach (AgentData agentData in allAgentData)
        {
            if (agentData != null)
            {
                agentDataList.Add(new AgentDataToSave(agentData)
                {
                    positionX = agentData.transform.position.x,
                    positionY = agentData.transform.position.y,
                    positionZ = agentData.transform.position.z
                });
            }
            else
            { ;
                Debug.Log("not serialized");
            }
        }

        // Serialize the list of agent data to JSON
        string jsonData = JsonConvert.SerializeObject(agentDataList);
        System.IO.File.WriteAllText(saveFileName, jsonData);

        saveButtonText.text = "Save Generation";
        savedAgentData = agentDataList;  // Store the data

        Debug.Log("Number of agents found for saving: " + allAgentData.Length);
    }

    void ImportProgress()
    {
        importButtonText.text = "Importing Progress...";

        // Read JSON data from file
        string jsonData = System.IO.File.ReadAllText(saveFileName);

        // Deserialize JSON data
        List<AgentDataToSave> importedDataList = JsonConvert.DeserializeObject<List<AgentDataToSave>>(jsonData);

        // Destroy existing agents
        GameObject[] existingAgents = GameObject.FindGameObjectsWithTag("Agent");
        foreach (GameObject obj in existingAgents)
        {
            Destroy(obj);
        }

        // Instantiate new agents based on loaded data
        for (int i = 0; i < importedDataList.Count; i++)
        {
            GameObject newAgent = Instantiate(agentPrefab);
            Debug.Log("Instantiated agent: " + newAgent.name); // Add this line

            AgentData agentDataComponent = newAgent.GetComponent<AgentData>();

            // Get the Creature component
            Creature creatureComponent = newAgent.GetComponent<Creature>();

            if (agentDataComponent == null)
            {
                Debug.LogError("AgentData component not found on the instantiated agent!");
            }
            else
            {
                // Apply the loaded data to the agent's AgentData component
                importedDataList[i].ApplyDataTo(agentDataComponent);

                // Update position based on loaded data
                newAgent.transform.position = new Vector3(
                    importedDataList[i].positionX,
                    importedDataList[i].positionY,
                    importedDataList[i].positionZ
                );
            }

            // Load the data into the Creature component
            if (creatureComponent != null) // Add this null check for safety
            {
                creatureComponent.LoadDataFromSave(importedDataList[i]);
            }
            else
            {
                Debug.LogError("Creature component not found on instantiated agent");
            }
        }

        importButtonText.text = "Import Progress";
    }

    // Helper class for serialization
    [System.Serializable] // Make this class serializable
    public class AgentDataToSave
    {
        public float distanceTraveled;
        public float energy;
        public float interactionCounter;
        public float reproductionEnergy;
        public float timeAlive;
        public float positionX;
        public float positionY;
        public float positionZ;

        public AgentDataToSave(AgentData agentData)
        {
            // Copy properties from the AgentData
            this.distanceTraveled = agentData.distanceTraveled;
            this.energy = agentData.energy;
            this.interactionCounter = agentData.interactionCounter;
            this.reproductionEnergy = agentData.reproductionEnergy;
            this.timeAlive = agentData.timeAlive;
        }

        public void ApplyDataTo(AgentData agentData)
        {
            // Update AgentData with saved values
            agentData.distanceTraveled = this.distanceTraveled;
            agentData.energy = this.energy;
            agentData.interactionCounter = this.interactionCounter;
            agentData.reproductionEnergy = this.reproductionEnergy;
            agentData.timeAlive = this.timeAlive;
        }
    }
}