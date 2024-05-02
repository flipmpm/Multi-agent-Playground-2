using UnityEngine;

public class AgentData : MonoBehaviour
{
    // Reference to the Creature script
    public Creature creature;

    public float distanceTraveled;
    public float energy;
    public float interactionCounter;
    public float reproductionEnergy;
    public float timeAlive;

    // Reference to the DistanceTracker script
    public DistanceTracker distanceTracker;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If Creature is not already assigned, try to find it 
        if (creature == null)
        {
            creature = GetComponent<Creature>();
        }
        if (creature == null)
        {
            Debug.LogWarning("Creature component not found on the same GameObject as AgentData.");
        }

        // If DistanceTracker is not already assigned, try to find it
        if (distanceTracker == null)
        {
            distanceTracker = GetComponent<DistanceTracker>();
        }
        if (distanceTracker == null)
        {
            Debug.LogWarning("DistanceTracker component not found on the same GameObject as AgentData.");
        }

        
    }

    // Method to update the data from the Creature and DistanceTracker scripts
    public void UpdateData()
    {
        //Debug.Log("UpdateData() called.");

        if (creature != null)
        {
            // Extract data from the Creature script
            energy = creature.energy;
            interactionCounter = creature.interactionCounter;
            reproductionEnergy = creature.reproductionEnergy;
            timeAlive = creature.timeAlive;
        }
        else
        {
            Debug.LogWarning("Creature reference is null. Make sure to set it before updating data.");
        }

        if (distanceTracker != null)
        {
            // Extract data from the DistanceTracker script
            distanceTraveled = distanceTracker.totalDistance;
            //Debug.Log("distanceTraveled value is: " + distanceTraveled);
        }
        else
        {
            Debug.LogWarning("DistanceTracker reference is null. Make sure to set it before updating data.");
        }
    }

    // Excluding agentPrefab from serialization
    [System.NonSerialized]
    public GameObject agentPrefab;
}
