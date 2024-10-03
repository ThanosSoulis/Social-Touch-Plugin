using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LogEmotionalResponse : MonoBehaviour
{
    [SerializeField] private Slider valenceSlider;
    [SerializeField] private Slider arousalSlider;
    [SerializeField] private bool isInputEmotion;

    private DataLogger _dataLogger;
    private StudySettings _settings;
    private ServerStudyController _serverStudyController;
    private void Awake()
    {
        _dataLogger = FindAnyObjectByType<DataLogger>();
        if(_dataLogger.IsUnityNull())
        {
            Debug.LogError("Data Logger not found! : Cannot log Emotional Response");
            return;
        }
        _serverStudyController = FindAnyObjectByType<ServerStudyController>();
        if(_serverStudyController.IsUnityNull())
        {
            Debug.LogError("ServerStudyController not found! : Cannot send Log RPC to Server");
            return;
        }
        _settings = FindAnyObjectByType<StudySettings>();
        if(_settings.IsUnityNull())
        {
            Debug.LogError("StudySettings not found! : Cannot log Emotional Response");
            return;
        }
    }
    
    // Called by the onClick of the Assess Panels
    public void LogResponse()
    {
        float valence = valenceSlider.value;
        float arousal = arousalSlider.value;
        
        _dataLogger.SaveEmotionalCategorization(_settings.participantPairId, _settings.participant, _settings.image, 
            isInputEmotion, _settings.assessmentRound, valence, arousal);

        // Tell the server to Log the Emotional Categorization too.
        if (NetworkManager.Singleton.IsClient)
        {
            _serverStudyController.LogEmotionalResponseServerRPC(valence, arousal, _settings.participant, _settings.image, isInputEmotion);
        }
    }
}
