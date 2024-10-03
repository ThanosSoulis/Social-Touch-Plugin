using TMPro;
using UnityEngine;
using Unity.Netcode;

// Based on the Button, the corresponding panel on the correct participant is opened.
// Panel IDs:
// 0-3: Prepare Initiate Touch with Emotional Image
// 4:   Prepare Receive Touch
// 5:   Prepare Respond Touch
// 6:   Assess Input Touch
// 7:   Assess Output Touch
public class ServerStudyController : NetworkBehaviour
{
    private int _participantPairID;
    private NetworkManager _networkManager;
    private StudySettings _studySettings;
    private DataLogger _dataLogger;

    private void Start()
    {
        _studySettings = FindAnyObjectByType<StudySettings>();
        _networkManager = NetworkManager.Singleton;
        _dataLogger = FindAnyObjectByType<DataLogger>();
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

            Debug.LogWarning("Expected -- Participant " + participant.ToString() + " is not running on ClientID: " + clientID);
        }
        return null;
    }
    private int GetEmotionalImagePanelIndex(EmotionalImage image)
    {
        switch (image)
        {
            case EmotionalImage.HighValenceLowArousal:
                return 0;
            case EmotionalImage.HighValenceHighArousal:
                return 1;
            case EmotionalImage.LowValenceLowArousal:
                return 2;
            case EmotionalImage.LowValenceHighArousal:
                return 3;
            default:
                return -1;
        }
    }
    private void SetStudySettingsServer(EmotionalImage image, Participant participant)
    {
        _studySettings.participant = participant;
        _studySettings.image = image;
        _studySettings.assessmentRound++;
        
        //participantPairID has already been set in server
    }
    public void PrepareInitiateTouch(EmotionalImage image, Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        var panelIndex = GetEmotionalImagePanelIndex(image);

        if(panelIndex < 0)
        {
            Debug.LogError("Unexpected Image panel requested");
            return;
        }

        SetStudySettingsServer(image, participant);

        // Send to the correct Client the panel Index to enable
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.OpenPanelRPC(client.ClientId, participant, _studySettings.participantPairId, panelIndex);
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
            clientStudyController.OpenPanelRPC(client.ClientId, participant, _studySettings.participantPairId, panelIndex);
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
            clientStudyController.OpenPanelRPC(client.ClientId, participant, _studySettings.participantPairId, panelIndex);
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
            clientStudyController.OpenPanelRPC(client.ClientId, participant, _studySettings.participantPairId, panelIndex);
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
            clientStudyController.OpenPanelRPC(client.ClientId, participant, _studySettings.participantPairId, panelIndex);
    }
    public void CloseAllPanels(Participant participant)
    {
        var client = WhichParticipant(participant);
        if (client == null)
        {
            Debug.LogError("No valid participant");
            return;
        }

        // Send the command to Client to close all panels
        if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
            clientStudyController.CloseAllPanelsRPC(client.ClientId,participant);            
    }

    [Rpc(SendTo.Server)]
    public void LogEmotionalResponseServerRPC(float valence, float arousal, Participant participant,
        EmotionalImage image, bool isInputEmotion)
    {
        _dataLogger.SaveEmotionalCategorization(_studySettings.participantPairId, participant, image,
            isInputEmotion, _studySettings.assessmentRound ,valence, arousal);
    }
}

public enum EmotionalImage
{
    HighValenceLowArousal,
    HighValenceHighArousal,
    LowValenceLowArousal,
    LowValenceHighArousal
}

public enum Participant { A, B }
