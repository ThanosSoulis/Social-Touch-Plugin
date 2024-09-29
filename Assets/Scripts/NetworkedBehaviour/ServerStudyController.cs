using UnityEngine;
using Unity.Netcode;

// Based on the Button, the corresponding panel on the correct participant is opened.
// Panel IDs:
// 0-3: Prepare Initiate Touch with Emotional Image
// 4:   Prepare Receive Touch
// 5:   Prepare Respond Touch
// 6:   Assess Input Touch
// 7:   Assess Output Touch
public class ServerStudyController : MonoBehaviour
{
    [SerializeField] private Canvas serverControllerCanvas;
    private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
    }

    private NetworkClient WhichParticipant(Participant participant)
    {
        foreach (var clientPair in _networkManager.ConnectedClients)
        {
            var clientID = clientPair.Key;
            var client = clientPair.Value;

            // Participant A
            if (participant == Participant.A && clientID % 2 == 0)
                return client;
            // Participant B
            else if (participant == Participant.B && clientID % 2 != 0)
                return client;

            Debug.LogWarning("Expected -- Participant " + participant.ToString() + "is not running on ClientID:" + clientID);
        }
        return null;
    }

    private int GetEmotionalImagePanelIndex(EmotionalImage image)
    {
        switch (image)
        {
            case EmotionalImage.PositiveValenceLowArousal:
                return 0;
            case EmotionalImage.PositiveValenceHighArousal:
                return 1;
            case EmotionalImage.NegativaValenceLowArousal:
                return 2;
            case EmotionalImage.NegativeValenceHighArousal:
                return 3;
            default:
                return -1;
        }
    }

    public void PrepareInitiateTouch(EmotionalImage image, Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        var imageIndex = GetEmotionalImagePanelIndex(image);

        if(imageIndex < 0)
        {
            Debug.LogError("Unexpected Image panel requested");
            return;
        }
    }

    public void PrepareReceiveTouch(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }
        
        // Enable the 5th Client panel
        var panelIndex = 4;
        
        // Send to the correct Client the panel Index to enable
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.OpenPanelRPC(client.ClientId, participant, panelIndex);
    }
    public void PrepareRespondTouch(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        // Enable the 6th Client panel
        var panelIndex = 5;
        
        // Send to the correct Client the panel Index to enable
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.OpenPanelRPC(client.ClientId, participant, panelIndex);
    }
    public void AssessOutputTouch(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        // Enable the 7th Client panel
        var panelIndex = 6;
        
        // Send to the correct Client the panel Index to enable
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.OpenPanelRPC(client.ClientId, participant, panelIndex);
    }

    public void AssessInputTouch(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        // Enable the 8th Client panel
        var panelIndex = 7;
        
        // Send to the correct Client the panel Index to enable
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.OpenPanelRPC(client.ClientId, participant, panelIndex);
    }

    public void CloseAllPanels(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        // Send the command to Client to close all panels
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.CloseAllPanelsRPC(client.ClientId,participant);            
    }
}

public enum EmotionalImage
{
    PositiveValenceLowArousal,
    PositiveValenceHighArousal,
    NegativaValenceLowArousal,
    NegativeValenceHighArousal
}

public enum Participant { A, B }
