using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ClientStudyController : NetworkBehaviour
{
    private SetupStudyController _setupController;
    private XRUIInputModule _xruiInputModule;
    private InputSystemUIInputModule _uiInputModule;
    private StudySettings _studySettings;
    private DataLogger _dataLogger;

    // Panels have a size of 8.
    [SerializeField] private GameObject[] participantPanels;
    [SerializeField] private GameObject experimentStartPanel;
    
    private void OnEnable()
    {
        _setupController = FindAnyObjectByType<SetupStudyController>();
        _xruiInputModule = FindAnyObjectByType<XRUIInputModule>();
        _uiInputModule = FindAnyObjectByType<InputSystemUIInputModule>();
        _studySettings = FindAnyObjectByType<StudySettings>();
        _dataLogger = FindAnyObjectByType<DataLogger>();
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
        
        // Disable the XR UI Input Module if we are the server
        if (IsServer)
        {
            _xruiInputModule.enabled = false;
        }
        
        // Disable the Canvas & simple UI Input Module if we are the client. Initiate Logger.
        if (IsClient)
        {
            _setupController?.gameObject.SetActive(false);
            _uiInputModule.enabled = false;
            
            _dataLogger.InitiateLoggerClient();
        }
    }

    public void OpenExperimentStartPanel()
    {
        experimentStartPanel.SetActive(true);
    }

    private void CloseExperimentStartPanel()
    {
        experimentStartPanel.SetActive(false);
    }

    [Rpc(SendTo.NotServer)]
    public void OpenPanelRPC(ulong clientID, Participant participant, int participantPairId ,int panelID)
    {
        if(OwnerClientId != clientID)
            return;

        if(panelID < 0)
        {
            Debug.LogError("Unexpected Panel requested");
            return;
        }
        
        SetStudySettingsClient(participant, panelID, participantPairId);
        
        Debug.Log("Received an OpenPanel | Client ID: " + clientID + " Panel ID: " + panelID + " Participant: " + participant.ToString());
        
        // Disable all Panels except the correct image panel one
        for (int i = 0; i < participantPanels.Length; i++)
            participantPanels[i].SetActive(i==panelID);
        
        CloseExperimentStartPanel();
    }
    
    [Rpc(SendTo.NotServer)]
    public void CloseAllPanelsRPC(ulong clientID, Participant participant)
    {
        if(OwnerClientId != clientID)
            return;

        // Disable all Panels
        for (int i = 0; i < participantPanels.Length; i++)
            participantPanels[i].SetActive(false);
        
        CloseExperimentStartPanel();

        Debug.Log("Closed All Panels");
    }
    
    private void SetStudySettingsClient(Participant participant, int panelID, int participantPairId)
    {
        _studySettings.participant = participant;
        _studySettings.participantPairId = participantPairId;

        switch (panelID)
        {
            case 0:
                _studySettings.image = EmotionalImage.HighValenceLowArousal;
                _studySettings.assessmentRound++;
                break;
            case 1:
                _studySettings.image = EmotionalImage.HighValenceHighArousal;
                _studySettings.assessmentRound++;
                break;
            case 2:
                _studySettings.image = EmotionalImage.LowValenceLowArousal;
                _studySettings.assessmentRound++;
                break;
            case 3:
                _studySettings.image = EmotionalImage.LowValenceHighArousal;
                _studySettings.assessmentRound++;
                break;
        }
    }
    
}
