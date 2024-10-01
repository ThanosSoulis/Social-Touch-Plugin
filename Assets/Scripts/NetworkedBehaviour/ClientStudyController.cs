using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ClientStudyController : NetworkBehaviour
{
    private SetupStudyController _setupController;
    private XRUIInputModule _xruiInputModule;
    private InputSystemUIInputModule _uiInputModule;

    // Panels have a size of 8.
    [SerializeField] private GameObject[] participantPanels;
    
    private void OnEnable()
    {
        _setupController = FindAnyObjectByType<SetupStudyController>();
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();
        
        // Disable the XR UI Input Module if we are the server
        if (IsServer)
        {
            _xruiInputModule.enabled = false;
        }
        
        // Disable the Canvas & simple UI Input Module if we are the client
        if (IsClient)
        {
            _setupController.gameObject.SetActive(false);
            _uiInputModule.enabled = false;
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
