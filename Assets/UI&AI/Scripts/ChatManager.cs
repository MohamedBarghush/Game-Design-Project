using UnityEngine;
using TMPro;
using LLMUnity; // Ensure this matches your language model integration

public class ChatManager : MonoBehaviour
{
    public TMP_InputField userInputField; // Assign in Inspector
    public TextMeshProUGUI responseText;  // Assign in Inspector
    public LLMCharacter llmCharacter;     // Assign in Inspector

    void Start()
    {
        // Add a listener to handle input submission when the user presses Enter
        userInputField.onSubmit.AddListener(HandleUserInput);
    }

    void HandleUserInput(string input)
    {
        SendMessageToModel(input);
        userInputField.text = ""; // Clear the Input Field
        userInputField.ActivateInputField(); // Reactivate the Input Field
    }

    void SendMessageToModel(string message)
    {
        // Send the message to the model and define a callback to handle the response
        _ = llmCharacter.Chat(message, HandleModelResponse);
    }

    void HandleModelResponse(string reply)
    {
        // Display the model's response in the Text component
        responseText.text = reply;
    }
}
