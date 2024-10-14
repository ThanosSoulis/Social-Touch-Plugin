using System;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TeleportPlayersClient : NetworkBehaviour
{
    [SerializeField] KeyCode recenterButton = KeyCode.Space;
    [SerializeField] Transform target;

    private ClientStudyController _clientStudyController;

    private void OnEnable()
    {
        _clientStudyController = FindAnyObjectByType<ClientStudyController>();
    }

    [Rpc(SendTo.Server)]
    public void TeleportPlayerRPC(ulong clientID, Vector3 teleportPos, Quaternion teleportRot)
    {
        if(OwnerClientId != clientID)
            return;
        
        var oldPosition = transform.position;
        var oldRotation = transform.rotation;
        transform.position = teleportPos;
        transform.rotation = teleportRot;
        var newPosition = transform.position;
        var newRotation = transform.rotation;

        target = transform;

        _clientStudyController.OpenExperimentStartPanel();
        
        print($"{nameof(TeleportPlayerRPC)}() -> {nameof(OwnerClientId)}: {OwnerClientId} --- {nameof(oldPosition)}: {oldPosition} || {nameof(oldRotation)}: {oldRotation}" +
              $" --- {nameof(newPosition)}: {newPosition} || {nameof(newRotation)}: {newRotation}");
    }

    private void RecenterPlayer()
    {
        XROrigin xrOrigin = GetComponent<XROrigin>();

        xrOrigin.MoveCameraToWorldLocation(target.position);
        xrOrigin.MatchOriginUpCameraForward(target.up, target.forward);

        xrOrigin.transform.position = xrOrigin.transform.position + new Vector3(0,xrOrigin.CameraYOffset,0);

        Debug.Log("Recentered");
    }

    private void Update()
    {
        if(Input.GetKeyDown(recenterButton))
            RecenterPlayer();
    }
}
