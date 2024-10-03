using UnityEngine;

public class StudySettings : MonoBehaviour
{
    public int participantPairId = 0;
    public int assessmentRound = 0;
    public EmotionalImage image;
    public Participant participant;

    public void UpdateParticipantPairID(string value)
    {
        participantPairId = int.Parse(value);
        Debug.Log("Updated Participant Pair: "+participantPairId);
    }
}
