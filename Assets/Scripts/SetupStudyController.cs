using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetupStudyController : MonoBehaviour
{
    [SerializeField] Button[] ParticipantAButtons;
    [SerializeField] Button[] ParticipantBButtons;

    [SerializeField] Button ClosePanelsButton;
    [SerializeField] Button TeleportParticipantsButton;
    private ServerStudyController _serverController;
    private TeleportPlayersManager _teleportPlayersManager;
    
    void OnEnable()
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


        SetupParticipantButtons(Participant.A, ParticipantAButtons);
        SetupParticipantButtons(Participant.B, ParticipantBButtons);

        ClosePanelsButton.onClick.AddListener(()=> {_serverController.CloseAllPanels(Participant.A); _serverController.CloseAllPanels(Participant.B);});
        TeleportParticipantsButton.onClick.AddListener(()=> _teleportPlayersManager.TeleportPlayers());
    }

    private void SetupParticipantButtons(Participant participant, Button[] buttons)
    {
        buttons[0].onClick.AddListener(() =>_serverController.PrepareInitiateTouch(EmotionalImage.PositiveValenceLowArousal, participant));
        buttons[1].onClick.AddListener(() =>_serverController.PrepareInitiateTouch(EmotionalImage.PositiveValenceHighArousal, participant));
        buttons[2].onClick.AddListener(() =>_serverController.PrepareInitiateTouch(EmotionalImage.NegativaValenceLowArousal, participant));
        buttons[3].onClick.AddListener(() =>_serverController.PrepareInitiateTouch(EmotionalImage.NegativeValenceHighArousal, participant));
        buttons[4].onClick.AddListener(() =>_serverController.PrepareReceiveTouch(participant));
        buttons[5].onClick.AddListener(() =>_serverController.PrepareRespondTouch(participant));
        buttons[6].onClick.AddListener(() =>_serverController.AssessOutputTouch(participant));
        buttons[7].onClick.AddListener(() =>_serverController.AssessInputTouch(participant));
    }
}
