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
    private DistanceTracker distanceTracker;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Attempt to get the Creature component from the same GameObject
        creature = GetComponent<Creature>();
        if (creature == null)
        {
            Debug.LogWarning("Creature component not found on the same GameObject as AgentData.");
        }

        // Attempt to get the DistanceTracker component from the same GameObject
        distanceTracker = GetComponent<DistanceTracker>();
        if (distanceTracker == null)
        {
            Debug.LogWarning("DistanceTracker component not found on the same GameObject as AgentData.");
        }
    }

    // Method to update the data from the Creature and DistanceTracker scripts
    public void UpdateData()
    {
        Debug.Log("UpdateData() called.");

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
        }
        else
        {
            Debug.LogWarning("DistanceTracker reference is null. Make sure to set it before updating data.");
        }
    }
}