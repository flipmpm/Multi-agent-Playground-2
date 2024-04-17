using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OpenAI;
using UnityEngine.EventSystems;
using StarterAssets; // Import the FirstPersonController namespace

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        public AgentController agentController;

        [SerializeField] public InputField inputField;
        [SerializeField] public Button button;
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        [SerializeField] private FirstPersonController firstPersonController; // Reference to the FirstPersonController component


        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private bool isWriting = false; // Track if the user is currently writing

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are a wild imaginary otherwordly being that exists in a unknown world. This world is an virtual reality experience where you and many other agents like you will behave like a animal population " +
            "and be studied and observed by a human being, that is the user of the expirience. You'll be engaged by the user and will answer to his questions and answer always based on your personallity traits. Your personallity " +
            "should be clear and justify all of your arguments, don't hesitate in mention it. If you personallity is strong and you have good life conditions, you should advise the user on life moral lesons. In the other way, if you feel tired and your personality" +
            " is weak and insecure, you should ask for advises from the user on how to live and serach for meaning in live. You have to mention your relationship with the other agents, that depends on the amount of interaction you had with them." +
            " Focus on mental health, terapy and filosophy";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
            inputField.interactable = false; // Disable input field initially
        }

        private void Update()
        {
            // Check if the player clicked on an agent
            if (Input.GetKeyDown(KeyCode.T))
            {
                // If the input field is not interactable and T is pressed, activate writing mode
                if (!inputField.interactable)
                {
                    inputField.interactable = true;
                    inputField.Select();
                    inputField.ActivateInputField();
                    firstPersonController.enabled = false;
                    // Deactivate placeholder text
                    inputField.placeholder.enabled = false;
                }
                else
                {
                    // If the input field is already interactable, check if it's focused
                    if (!inputField.isFocused)
                    {
                        // If it's not focused, deactivate writing mode
                        inputField.DeactivateInputField();
                        EventSystem.current.SetSelectedGameObject(null);
                        firstPersonController.enabled = true;
                        // Reactivate placeholder text
                        inputField.placeholder.enabled = true;
                    }
                }
            }

            // Check if the input field is activated and the user presses the 'Enter' key
            if (inputField.interactable && Input.GetKeyDown(KeyCode.Return))
            {
                // Send the reply
                SendReply();
                // Reactivate placeholder text
                inputField.placeholder.enabled = true;
            }
        }


        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                Debug.LogWarning("Cannot send empty message.");
                return;
            }

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            messages.Add(newMessage);
            AppendMessage(newMessage);

            // Update the prompt when appending a message
            string ConcatenatedAgentData = UpdatePrompt();

            newMessage.Content += ConcatenatedAgentData;


            inputField.text = ""; // Clear input field after sending message
            inputField.interactable = false; // Disable input field after sending message

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            // Enable the FirstPersonController movement after sending the message
            firstPersonController.enabled = true;

        }

        // Method to handle agent selection
        public void SelectAgent(AgentData selectedAgentData)
        {
            agentController.SelectAgent(selectedAgentData);
            // Call UpdatePrompt method to update the prompt
            UpdatePrompt();
        }

        // Method to update the prompt based on the selected agent's data
        private string UpdatePrompt()
        {
            if (agentController != null)
            {
                Debug.Log("AgentController is assigned.");

                if (agentController.SelectedAgentData != null)
                {
                    Debug.Log("SelectedAgentData is assigned.");
                    // Call ConstructPrompt method to update the prompt
                    return ConstructPrompt(agentController.SelectedAgentData);
                }
                else
                {
                    Debug.LogWarning("SelectedAgentData is not assigned.");
                }
            }
            else
            {
                Debug.LogWarning("AgentController is not assigned.");
            }

            return "";
        }


        private string ConstructPrompt(AgentData agentData)
        {


            // Construct the prompt based on agent's behavior data
            string prompt = "Agent behavior analysis:\n" +
                            "Distance Traveled: " + agentData.distanceTraveled + "\n" +
                            "Energy: " + agentData.energy + "\n" +
                            "Interaction Counter: " + agentData.interactionCounter + "\n" +
                            "Reproduction Energy: " + agentData.reproductionEnergy + "\n" +
                            "Time Alive: " + agentData.timeAlive + "\n" +
                            "Personality Traits: " + DeterminePersonalityTraits(agentData) + "\n";

            return prompt;
        }

        private string DeterminePersonalityTraits(AgentData agentData)
        {
            // Example: Determine personality traits based on agent's behavior data
            string personalityTraits = "";

            if (agentData.energy > 50)
            {
                personalityTraits += "Energetic, view on life is very positive and entusiastic. Dont' feel tired, and are up for anything. Don't want to stop the conversation and talk with a lot of exclamation. " +
                    "Should ask the user about his prespective on the agent species and ask forr advices on where to go next.";
            }
            else
            {
                personalityTraits += "Tired, cautious, life is hard, and everything is problematic. Feel that everything done isn't enough, and always seek help. Asks the user for food insistently.";
            }

            if (agentData.interactionCounter > 1000)
            {
                personalityTraits += "Social, keen to interaction and talk. Great knowledge and empathy. Should mention art and contemplate the species. Should aproach the user personallity and ask the user to share his best experiences.";
            }
            else
            {
                personalityTraits += "Reserved, live with the least amount of interactions possible. Like to be alone and can be rude. Should always try to end the conversation";
            }

            if (agentData.distanceTraveled > 200)
            {
                personalityTraits += "Adventured, life is too short to stay in the same place. Doesn't have longlasting relations and like to know new places. Should compare the user to someone knowned previoulsy";
            }
            else
            {
                personalityTraits += "Confortable, doesn't take risks. Likes routine and have very strong habits. Should ask the user about his habits";
            }

            if (agentData.timeAlive > 30)
            {
                personalityTraits += "Experienced, have seen enought to be an elder of his own species. Wonders about his past and is future. Always asks the user to share any past experience based on his age." +
                    " Should share moral short stories at any first chance. ";
            }
            else
            {
                personalityTraits += "Young, doesnt know a lot about evrything, always ask for help and tips. Is insecure and confused and should only talk with questions. Should ask the user about the next steps in life.";
            }

            if (agentData.reproductionEnergy > 10)
            {
                personalityTraits += "Horny, it's ready to mate so evreything mentioned should be about it. Asks if the user knows about a good partner around.";
            }
            else
            {
                personalityTraits += "Focused, survivalist. Life should be all about eating and surviving. Wonders about mating and if one he will deserve a partner. Asks the user about relationships.";
            }

            // Add more rules based on other behavior data...

            return personalityTraits.TrimEnd(' ', ','); // Remove trailing comma and space
        }

    }
}