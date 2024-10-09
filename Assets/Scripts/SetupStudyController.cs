using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetupStudyController : MonoBehaviour
{
    [Header("Interval Settings")]
    [SerializeField] private float InstructionsTimeInSeconds = 0.5f;
    [SerializeField] private float TouchTimeInSeconds = 0.5f;
    
    [Header("Observer-Participant Buttons Settings")]
    [SerializeField] Button[] ParticipantAButtons;
    [SerializeField] Button[] ParticipantBButtons;

    [Header("Observer Buttons Settings")]
    [SerializeField] Button ClosePanelsButton;
    [SerializeField] Button Initialise_TeleportParticipantsButton;
    
    private ServerStudyController _serverController;
    private TeleportPlayersManager _teleportPlayersManager;
    private DataLogger _dataLogger;
    private StudySettings _studySettings;
    
    // Timer implementation
    // ====================================
    private Action<Participant, Participant> _callback;
    private float _duration;
    public void StartTimer(float duration, Action<Participant, Participant> callback, Participant initiator, Participant receiver)
    {
        _duration = duration;
        _callback = callback;
        StartCoroutine(TimerCoroutine(initiator, receiver));
    }

    private IEnumerator TimerCoroutine(Participant initiator, Participant receiver)
    {
        yield return new WaitForSeconds(_duration);
        _callback?.Invoke(initiator, receiver);
    }
    
    public void StartWatching(Func<bool> condition, Action<Participant, Participant> callback, Participant a, Participant b)
    {
        StartCoroutine(WatchVariableCoroutine(condition, callback, a, b));
    }

    private IEnumerator WatchVariableCoroutine(Func<bool> condition,
        Action<Participant, Participant> callback, Participant a, Participant b)
    {
        while (!condition())
        {
            yield return null;
        }
        callback?.Invoke(a,b);
    }
    
    void Start()
    {
        _serverController = FindAnyObjectByType<ServerStudyController>();
        if(_serverController.IsUnityNull())
        {
            Debug.LogError("Server Study Controller not found! : Cannot setup Participant Buttons");
            return;
        }

        _teleportPlayersManager = FindAnyObjectByType<TeleportPlayersManager>();
        if(_teleportPlayersManager.IsUnityNull())
        {
            Debug.LogError("Teleport Players Manager not found! : Cannot setup Participant Buttons");
            return;
        }
        
        _dataLogger = FindAnyObjectByType<DataLogger>();
        if(_dataLogger.IsUnityNull())
        {
            Debug.LogError("Data Logger not found! : Cannot Log Anything");
            return;
        }

        _studySettings = FindAnyObjectByType<StudySettings>();
        if(_dataLogger.IsUnityNull())
        {
            Debug.LogWarning("Study Settings not found - That is ok");
            return;
        }
        
        SetupParticipantButtons(Participant.A, ParticipantAButtons);
        SetupParticipantButtons(Participant.B, ParticipantBButtons);

        ClosePanelsButton.onClick.AddListener(()=> {_serverController.CloseAllPanels(Participant.A); _serverController.CloseAllPanels(Participant.B);});
        Initialise_TeleportParticipantsButton.onClick.AddListener(()=>
        {
            // Initiating the Data Logger on the Server
            _dataLogger.InitiateLoggerServer();
            _teleportPlayersManager.TeleportPlayers();
        });
        
        Debug.Log("Setup Study Controller | Completed");
    }
    
    private void SetupParticipantButtons(Participant participant, Button[] buttons)
    {
        buttons[0].onClick.AddListener(() =>SetupPrepareInitiateTouch(EmotionalImage.HighValenceLowArousal, participant));
        buttons[1].onClick.AddListener(() =>SetupPrepareInitiateTouch(EmotionalImage.HighValenceHighArousal, participant));
        buttons[2].onClick.AddListener(() =>SetupPrepareInitiateTouch(EmotionalImage.LowValenceLowArousal, participant));
        buttons[3].onClick.AddListener(() =>SetupPrepareInitiateTouch(EmotionalImage.LowValenceHighArousal, participant));
        buttons[4].onClick.AddListener(() =>_serverController.PrepareReceiveTouch(participant));
        buttons[5].onClick.AddListener(() =>_serverController.PrepareRespondTouch(participant));
        buttons[6].onClick.AddListener(() =>_serverController.AssessOutputTouch(participant));
        buttons[7].onClick.AddListener(() =>_serverController.AssessInputTouch(participant));
    }
    
    private void SetupPrepareInitiateTouch(EmotionalImage image, Participant initiator)
    {
        Participant receiver = initiator;
        if (initiator == Participant.A)
            receiver = Participant.B;
        if (initiator == Participant.B)
            receiver = Participant.A;
        
        if(receiver == initiator)
            Debug.LogError("Receiver and Initiator are the same Client");
        
        // In this call the TouchRound state is Initialised
        _serverController.PrepareInitiateTouch(image, initiator);
        _serverController.PrepareReceiveTouch(receiver);

        Action<Participant, Participant> callbackAction = ClosePanelsForTouch;
        StartTimer(InstructionsTimeInSeconds, callbackAction, initiator, receiver);
        
        // Setting up the response from the Receiver 
        StartWatching(() => _serverController.TouchRoundState == TouchRound.Response, SetupRespondTouch, receiver, initiator);

        // Setting up the final response from the Initiator
        StartWatching(() => _serverController.TouchRoundState == TouchRound.Acknowledge, SetupRespondTouch, initiator, receiver);

        // Log when a Condition is Done
        StartWatching(() => _serverController.TouchRoundState == TouchRound.Done, LogConditionDone, initiator, receiver);
    }
    
    // Preparing for a Respond to initial touch.
    // Beware to swap the roles of initiator and receiver when calling this method.
    private void SetupRespondTouch(Participant initiator, Participant receiver)
    {
        _serverController.PrepareRespondTouch(initiator);
        _serverController.PrepareReceiveTouch(receiver);

        Action<Participant, Participant> callbackAction = ClosePanelsForTouch;
        StartTimer(InstructionsTimeInSeconds, callbackAction, initiator, receiver);
    }
    
    // Closing all panels and allowing touch waiting for TouchTimeInSeconds. Then Assessment is setup
    private void ClosePanelsForTouch(Participant initiator, Participant receiver)
    {
        _serverController.CloseAllPanels(initiator);
        _serverController.CloseAllPanels(receiver);
        
        Action<Participant, Participant> callbackAction = SetupAssessment;
        StartTimer(TouchTimeInSeconds, callbackAction, initiator, receiver);
    }
    
    // Setting up Assessment for initiator and receiver
    private void SetupAssessment(Participant initiator, Participant receiver)
    {
        _serverController.AssessInputTouch(receiver);
        _serverController.AssessOutputTouch(initiator);
    }

    private void LogConditionDone(Participant initiator, Participant receiver)
    {
        _serverController.CloseAllPanels(initiator);
        _serverController.CloseAllPanels(receiver);
        
        Debug.LogErrorFormat("Condition -{0}- is Done", _studySettings?.image);
    }
    
}
