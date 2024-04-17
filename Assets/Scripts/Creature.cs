using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public bool mutateMutations = true;
    public GameObject agentPrefab;
    public bool isUser = false;
    public bool canEat = true;
    public float viewDistance = 20;
    public float size = 1.0f;
    public float energy = 60;
    public float energyGained = 10;

    public float timeAlive = 0f; // Variable to track the time the agent is alive


    // Adds:
    public float interactionCounter = 0;
    public float interactionGained = 1;
    public float interactionEnergyThreshold = 10;

    private bool isInteracting = false;
    private float interactionTimer = 0f;
    private float interactionDurationThreshold = 5f; // Minimum duration for an interaction to count

    public float reproductionEnergyGained = 1;
    public float reproductionEnergy = 0;
    public float reproductionEnergyThreshold = 10;
    public float FB = 0;
    public float LR = 0;
    // Add:
    public float IN = 0;

    public int numberOfChildren = 1;
    private bool isMutated = false;
    float elapsed = 0f;
    public float lifeSpan = 0f;
    public float[] distances = new float[6];

    public float mutationAmount = 0.8f;
    public float mutationChance = 0.2f;
    public NN nn;
    public Movement movement;

    float relativeFoodX;
    float relativeFoodZ;

    private List<GameObject> edibleFoodList = new List<GameObject>();

    public bool isDead = false;

    [SerializeField] private Renderer agentRenderer; // Reference to the agent's renderer component
    private Material agentMaterial; // Material for this agent
    private Color originalColor; // Original color of the agent's material
    private bool isDying = false; // Flag to track if the agent is currently dying
    private float deathTimer = 0f; // Timer for the death animation

    // Reference to the AgentData component
    public AgentData agentData;

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to get the AgentData component from the same GameObject
        agentData = GetComponent<AgentData>();
        if (agentData == null)
        {
            Debug.LogWarning("AgentData component not found on the same GameObject as Creature.");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        nn = gameObject.GetComponent<NN>();
        movement = gameObject.GetComponent<Movement>();
        distances = new float[12];

        this.name = "Agent";

        // Get the renderer component of the agent
        //agentRenderer = GetComponent<Renderer>();

        // Create a new material instance for this agent
        agentMaterial = new Material(agentRenderer.material);
        agentRenderer.material = agentMaterial;

        // Store the original color of the agent's material
        originalColor = agentRenderer.material.color;

        // Attempt to get the AgentData component from the same GameObject
        agentData = GetComponent<AgentData>();
        if (agentData == null)
        {
            Debug.LogWarning("AgentData component not found on the same GameObject as Creature.");
        }
    }

    // Method to update the creature's data
    void UpdateCreatureData()
    {
        Debug.Log("UpdateCreatureData() called.");

        // Update the creature's data here (e.g., energy, interactionCounter, etc.)
        // For example:
        if (agentData != null && this != null) // Make sure 'this' refers to the current instance of Creature
        {
            agentData.energy = energy; // Assuming 'energy' is a variable declared in the Creature script
            agentData.interactionCounter = interactionCounter; // Assuming 'interactionCounter' is a variable declared in the Creature script
            agentData.reproductionEnergy = reproductionEnergy; // Assuming 'reproductionEnergy' is a variable declared in the Creature script
            agentData.timeAlive = timeAlive; // Assuming 'timeAlive' is a variable declared in the Creature script
            agentData.distanceTraveled = distances[0]; // Example: Assigning distance traveled
        }
        else
        {
            Debug.LogWarning("AgentData or Creature reference is null.");
        }

        // After updating the data, call the UpdateData() method of the AgentData component
        if (agentData != null)
        {
            agentData.UpdateData();
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // Update the time alive
        timeAlive += Time.deltaTime;

        // Update the creature's energy
        ManageEnergy();

        // Update the agent data
        UpdateAgentData();

        // Only do this once
        if (!isMutated)
        {
            // Call mutate function to mutate the neural network
            MutateCreature();
            this.transform.localScale = new Vector3(size, size, size);
            isMutated = true;
            energy = 50;
        }

        // This section of code is for the new food detection system (Raycasts)
        // Set up a variable to store the number of raycasts to use
        int numRaycasts = 5;

        // Set up a variable to store the angle between raycasts
        float angleBetweenRaycasts = 30;

        // Set up an array to store the distances to the food objects detected by the raycasts
        RaycastHit hit;
        bool otherAgent = false;
        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = ((2 * i + 1 - numRaycasts) * angleBetweenRaycasts / 2);
            // Rotate the direction of the raycast by the specified angle around the y-axis of the agent
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rayDirection = rotation * transform.forward * -1;
            // Increase the starting point of the raycast by 0.1 units
            Vector3 rayStart = transform.position + Vector3.up * 2f;
            if (Physics.Raycast(rayStart, rayDirection, out hit, viewDistance))
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                if (hit.transform.gameObject.tag == "Food")
                {
                    // Use the length of the raycast as the distance to the food object
                    distances[i] = hit.distance / viewDistance;
                }
                else if (hit.transform.gameObject.tag == "Agent")
                {
                    if (energy > 1.0f)
                    {
                        distances[i] = (hit.distance / viewDistance);
                        otherAgent = true;
                    }
                    else
                    {
                        distances[i] = -(hit.distance / viewDistance);
                    }
                }
                else
                {
                    // If no food object is detected, set the distance to the maximum length of the raycast
                    distances[i] = 1;
                }
            }
            else
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(rayStart, rayDirection * viewDistance, Color.red);
                // If no food object is detected, set the distance to the maximum length of the raycast
                distances[i] = 1;
            }
        }

        // Setup inputs for the neural network
        float[] inputsToNN = distances;

        // Get outputs from the neural network
        float[] outputsFromNN = nn.Brain(inputsToNN);

        // Store the outputs from the neural network in variables
        FB = outputsFromNN[0];
        LR = outputsFromNN[1];
        IN = outputsFromNN[2];

        // If the agent is the user, use the inputs from the user instead of the neural network
        if (isUser)
        {
            FB = Input.GetAxis("Vertical");
            LR = Input.GetAxis("Horizontal") / 10;
        }

        // Move the agent using the move function
        movement.Move(FB, LR);

        // New code block handling interactions with other agents
        if (otherAgent)
        {
            if (IN > 0)
            {
                // Start the interaction timer when a positive interaction begins
                if (!isInteracting)
                {
                    isInteracting = true;
                    interactionTimer = 0f;
                }

                energy -= 0.02f;

                // Increment the interaction timer
                interactionTimer += Time.deltaTime;

                // Check if the interaction duration exceeds the threshold
                if (interactionTimer >= interactionDurationThreshold)
                {
                    // Increment the interaction counter
                    interactionCounter++;
                }
            }
            else
            {
                // Reset the interaction timer when the interaction ends
                isInteracting = false;
                interactionTimer = 0f;
            }
        }

        // Check if the agent is dying
        if (isDying)
        {
            // Update the death timer
            deathTimer += Time.deltaTime;

            // Change the color of the agent smoothly over 3 seconds
            agentRenderer.material.SetColor("_BaseColor", Color.Lerp(originalColor, Color.black, deathTimer / 3f)) ;

            // Check if the death animation is complete
            if (deathTimer >= 3f)
            {
                // Destroy the agent object
                Destroy(gameObject);
            }
        }

        // Increment the time alive
        timeAlive += Time.deltaTime;


        Starve();
    }



    // This function gets called whenever the agent collides with a trigger. (Which in this case is the food)
    void OnTriggerEnter(Collider col)
    {
        // If the agent collides with a food object, it will eat it and gain energy.
        if (col.gameObject.tag == "Food" && canEat)
        {
            energy += energyGained;
            if (energy >= interactionCounter)
            {
                reproductionEnergy += reproductionEnergyGained;
            }
            Destroy(col.gameObject);
        }
    }

    public void ManageEnergy()
    {
        elapsed += Time.deltaTime;
        lifeSpan += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;

            // Subtract energy per second
            energy -= 1f;
            if (interactionCounter > 0)
            {
                interactionCounter -= 0.1f;
            }

            // If agent has enough energy to reproduce, reproduce
            if (reproductionEnergy >= reproductionEnergyThreshold)
            {
                reproductionEnergy = 0;
                Reproduce();
            }

            Starve();
        }
    }

    void UpdateAgentData()
    {
        // Ensure the AgentData component exists
        if (agentData != null)
        {
            // Update the agent data fields
            agentData.energy = energy;
            agentData.interactionCounter = interactionCounter;
            agentData.reproductionEnergy = reproductionEnergy;
            agentData.timeAlive = timeAlive;

            // Update other relevant data fields if needed
            // For example, distances traveled, etc.

            // Call the UpdateData method of AgentData to synchronize the data
            agentData.UpdateData();
        }
        else
        {
            Debug.LogWarning("AgentData reference is null.");
        }
    }

    public void Starve()
    {
        // Check if the agent needs to die due to starvation
        if (energy <= 0 || transform.position.y < -10)
        {
            // Disable movement component
            GetComponent<Movement>().enabled = false;

            // Set the flag to start the death animation
            isDying = true;
        }
    }

    private void MutateCreature()
    {
        if (mutateMutations)
        {
            mutationAmount += Random.Range(-1.0f, 1.0f) / 100;
            mutationChance += Random.Range(-1.0f, 1.0f) / 100;
        }

        // Make sure mutation amount and chance are positive using max function
        mutationAmount = Mathf.Max(mutationAmount, 0);
        mutationChance = Mathf.Max(mutationChance, 0);

        nn.MutateNetwork(mutationAmount, mutationChance);
    }

    // This function was used for the old input system (relative food position and angle)
    GameObject FindClosestFood()
    {
        GameObject closestFood = null;
        float agentX;
        float agentZ;
        float foodX = 0;
        float foodZ = 0;

        float minFoodDistance = -1;
        float foodDistance = 0;
        int minFoodIndex = -1;
        bool foodInRange = false;

        agentX = this.transform.position.x;
        agentZ = this.transform.position.z;

        // TODO: dynamically change the size of the sphere cast until it finds food to increase performance

        // Use a sphere cast to find all food in range (determined by viewDistance) of the agent and add them to a list of edible food.
        // This helps optimize the code by not having to check every food object in the scene.
        if (Random.value * 100 < 5)
        {
            edibleFoodList.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, viewDistance);
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject.tag == "Food")
                {
                    edibleFoodList.Add(hit.gameObject);
                }
            }
        }

        // Find closest food in range of agent
        if (Random.value * 100 < 50)
        {
            for (int i = 0; i < edibleFoodList.Count; i++)
            {
                if (edibleFoodList[i] != null)
                {
                    foodX = edibleFoodList[i].transform.position.x;
                    foodZ = edibleFoodList[i].transform.position.z;

                    foodDistance = Mathf.Sqrt((Mathf.Pow(foodX - agentX, 2) + Mathf.Pow(foodZ - agentZ, 2)));
                    if (foodDistance < minFoodDistance || minFoodDistance < 0)
                    {
                        minFoodDistance = foodDistance;
                        minFoodIndex = i;
                        if (minFoodDistance < viewDistance)
                        {
                            closestFood = edibleFoodList[i];
                            foodInRange = true;
                        }
                    }
                }
            }
        }

        return closestFood;
    }

    public void Reproduce()
    {
        // Replicate
        for (int i = 0; i < numberOfChildren; i++) // I left this here so I could possibly change the number of children a parent has at a time.
        {
            // Create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
            GameObject child = Instantiate(agentPrefab, new Vector3(
                (float)this.transform.position.x + Random.Range(-10, 11),
                0.75f,
                (float)this.transform.position.z + Random.Range(-10, 11)),
                Quaternion.identity);

            // Copy the parent's neural network to the child
            child.GetComponent<NN>().layers = GetComponent<NN>().copyLayers();
        }
        reproductionEnergy = 0;
    }
}
