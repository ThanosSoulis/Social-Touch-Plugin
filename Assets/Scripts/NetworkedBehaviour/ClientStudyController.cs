using UnityEngine;
using Unity.Netcode;

public class ClientStudyController : NetworkBehaviour
{
    private SetupStudyController _setupController;

    // Panels have a size of 8.
    [SerializeField] private GameObject[] participantPanels;

    private void OnEnable()
    {
        _setupController = FindAnyObjectByType<SetupStudyController>();
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();

        // Disable the Canvas if we are not the server
        if (!IsServer)
        {
            _setupController.gameObject.SetActive(false);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void OpenPanelRPC(ulong clientID, Participant participant, int panelID)
    {
        if(OwnerClientId != clientID)
            return;

        if(panelID < 0)
        {
            Debug.LogError("Unexpected Panel requested");
            return;
        }

        Debug.Log("Received an OpenPanel | Client ID: " + clientID + " Panel ID: " + panelID + " Participant: " + participant.ToString());


        // Disable all Panels except the correct image panel one
        for (int i = 0; i < participantPanels.Length; i++)
            participantPanels[i].SetActive(i==panelID);
    }

    [Rpc(SendTo.NotServer)]
    public void CloseAllPanelsRPC(ulong clientID, Participant participant)
    {
        if(OwnerClientId != clientID)
            return;

        // Disable all Panels
        for (int i = 0; i < participantPanels.Length; i++)
            participantPanels[i].SetActive(false);

        Debug.Log("Closed All Panels");
    }

}