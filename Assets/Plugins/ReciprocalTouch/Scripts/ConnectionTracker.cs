using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTracker : MonoBehaviour
{
    public GameObject HandSender;
    private bool _handSenderIsActive = true;

    public MammothRenderer MammothRenderer;

    void LateUpdate()
    {
        if (_handSenderIsActive != HandSender.activeSelf)
        {
            _handSenderIsActive = HandSender.activeSelf;

            if (HandSender.activeSelf == false)
            {
                Disconnected();
            }

        }

    }

    private void Disconnected()
    {
        MammothInteractable[] interactables = FindObjectsOfType<MammothInteractable>();

        for (int i = 0; i < interactables.Length; i++)
        {
            interactables[i].Disconnect();
        }

        MammothRenderer.Disconnect();
    }
}
