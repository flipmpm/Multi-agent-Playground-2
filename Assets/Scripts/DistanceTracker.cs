using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    public Text infoText; // Reference to the UI Text element

    public Vector3 previousPosition;
    public float totalDistance;

    // Reference to the Creature script
    private Creature creature;

    void Start()
    {
        // Get reference to the Creature script attached to this GameObject
        creature = GetComponent<Creature>();

        // Initialize the previous position to the starting position
        previousPosition = transform.position;
    }

    void Update()
    {
        // Calculate the distance traveled since the last frame
        float stepDistance = Vector3.Distance(previousPosition, transform.position);

        // Add the distance traveled in this step to the total distance
        totalDistance += stepDistance;

        // Update the previous position for the next frame
        previousPosition = transform.position;

        // Update the UI text to display the distance traveled and other information
        infoText.text = string.Format("Distance Traveled: {0:F2} units\n" +
                                      "Energy: {1:F2}\n" +
                                      "Interaction Counter: {2:F2}\n" +
                                      "Interaction Gained: {3:F2}\n" +
                                      "Reproduction Energy: {4:F2}\n" +
                                      "Is Dead: {5}\n" +
                                      "Time Alive: {6:F2} seconds",
                                      totalDistance, creature.energy,
                                      creature.interactionCounter, creature.interactionGained,
                                      creature.reproductionEnergy, creature.isDead ? "Yes" : "No",
                                      creature.timeAlive);
    }
}
