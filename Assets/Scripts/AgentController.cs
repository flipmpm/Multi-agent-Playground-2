using UnityEngine;

public class AgentController : MonoBehaviour
{
    // Declare the property with private setter
    public AgentData SelectedAgentData { get; private set; }

    // Method to update the selected agent data
    public void SetSelectedAgentData(AgentData agentData)
    {
        SelectedAgentData = agentData;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player clicked to confirm selection
        if (Input.GetMouseButtonDown(0))
        {
            // Check if an agent is already selected
            if (SelectedAgentData != null)
            {
                // Deselect the agent if it's destroyed
                DeselectAgentIfDestroyed();
            }
            else
            {
                // Ray cast from the center of the screen
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                RaycastHit hit;

                // Perform raycast and check if it hits an agent
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object has an AgentData component
                    AgentData agentData = hit.collider.GetComponent<AgentData>();
                    if (agentData != null)
                    {
                        // Set the selected agent data to the clicked agent's data
                        SelectAgent(agentData);
                        Debug.Log("Selected agent: " + agentData.name);
                    }
                    else
                    {
                        Debug.Log("No agent found at the clicked position.");
                    }
                }
                else
                {
                    Debug.Log("No object hit by the ray.");
                }
            }
        }
    }

    // Method to select an agent and update the selected agent data
    public void SelectAgent(AgentData selectedAgentData)
    {
        SelectedAgentData = selectedAgentData;
    }

    // Method to deselect the current agent if it's destroyed
    private void DeselectAgentIfDestroyed()
    {
        // Check if the selected agent's game object has been destroyed
        if (SelectedAgentData != null && SelectedAgentData.gameObject != null)
        {
            Debug.Log("Selected agent has been destroyed. Deselecting...");
            DeselectAgent();
        }
        else
        {
            Debug.Log("Selected agent still exists.");
        }
    }

    // Method to deselect the current agent
    private void DeselectAgent()
    {
        SelectedAgentData = null;
    }
}
