using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;


public class DataLogger : MonoBehaviour
{
    private StreamWriter _writer;
    private StudySettings _settings;
    
    public string path = "Assets/Resources/Pilot-Study/Logs/";

    private void Start()
    {
        _settings = FindAnyObjectByType<StudySettings>();
    }

    public void InitiateLoggerServer()
    {
        // Start logging for server
        if (!NetworkManager.Singleton.IsServer)
            return;
        
        var participantPairID = _settings.participantPairId;
            
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss"); //("yyyy-MM-dd\\THH:mm:ss");
        _writer = new StreamWriter(path + participantPairID + "_" + dateTime + ".txt", false);
        _writer.WriteLine("ParticipantPairId,Participant,EmotionalImage,Valence,Arousal,isInputEmotion,EmotionalAssessmentRound");

        Debug.Log("Started Logging in Server");
    }

    public void InitiateLoggerClient()
    {
        // Start logging for client
        if (!NetworkManager.Singleton.IsClient)
            return;
        
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd\\THH-mm-ss"); //("yyyy-MM-dd\\THH:mm:ss");
        _writer = new StreamWriter(path + "Client" + "_" + dateTime + ".txt", false);
        _writer.WriteLine("ParticipantPairId,Participant,EmotionalImage,Valence,Arousal,isInputEmotion,EmotionalAssessmentRound");

        Debug.Log("Started Logging in Client");
    }
    
    public void SaveEmotionalCategorization(int participantPairID, Participant participant, EmotionalImage emotionalImage, 
        bool isInputEmotion, int assessmentRound, float valence, float arousal)
    {
        string data = participantPairID + "," + participant + "," + emotionalImage + "," + valence + 
                      "," + arousal + "," + isInputEmotion + "," + assessmentRound;
        
        Debug.Log(data);
        _writer.WriteLine(data);
    }

    public void CloseWriter() 
    { 
        _writer?.Close();
        Debug.Log("Stopped Logging");
    }

    void OnDestroy() { CloseWriter(); }
}