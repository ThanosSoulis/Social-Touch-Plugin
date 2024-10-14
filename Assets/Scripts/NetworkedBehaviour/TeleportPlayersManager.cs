using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class TeleportPlayersManager : MonoBehaviour
{
    [Header("User Transforms")]
    public Transform userTransformA, userTransformB;

    [Header("Scene Components")]
    [SerializeField] private Camera sceneCamera;
    
    private NetworkManager _networkManager;
    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
        
        // Disable the Scene camera by default
        sceneCamera.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) TeleportPlayers();
    }

    public void TeleportPlayers()
    {
        // Only allow the server to teleport players
        if(_networkManager.IsClient)
            return;
        
        // Enable the Scene Camera, since we are the server
        sceneCamera.gameObject.SetActive(true);
        
        foreach (var clientPair in _networkManager.ConnectedClients)
        {
            var clientID = clientPair.Key;
            var client = clientPair.Value;

            // Odd number clientIDs will always be Participant B
            var spawnTransform = userTransformA;
            if (clientID % 2 != 0)
                spawnTransform = userTransformB;

            if (client.PlayerObject.TryGetComponent(out TeleportPlayersClient teleportPlayersClient))
                teleportPlayersClient.TeleportPlayerRPC(clientID, spawnTransform.position, spawnTransform.rotation);
            
            // Opens the Experiment Starts Panel on Client <--- hacky way of doing that but w/e
            if (client.PlayerObject.TryGetComponent(out ClientStudyController clientStudyController))
                clientStudyController.OpenExperimentStartPanel();

        }
    }
}
