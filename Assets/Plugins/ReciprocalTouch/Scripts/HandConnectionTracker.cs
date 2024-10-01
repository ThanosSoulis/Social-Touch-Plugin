// This script includes part of the HandEnableDisable created by Ultraleap, Inc. 2011-2024.

using Leap.Unity;
using Leap.Unity.HandsModule;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class HandConnectionTracker : HandTransitionBehavior
{
    private MammothRenderer _mammothRenderer;
    private HandBinder _handBinder;
    private XRInputModalityManager _xrInputModalityManager;
    private XRHandMeshController _leftHandMeshController, _rightHandMeshController;

    [SerializeField] bool XRIHandEnabled = false;

    [Tooltip("When enabled, freezes the hand in its current active state")]
    public bool FreezeHandState = false;

    protected override void Awake()
    {
        // Suppress Warnings Related to Kinematic Rigidbodies not supporting Continuous Collision Detection
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in bodies)
        {
            if (body.isKinematic && body.collisionDetectionMode == CollisionDetectionMode.Continuous)
            {
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }

        base.Awake();
    }
    
    private void Start()
    {
        _handBinder = GetComponent<HandBinder>();
        if(_handBinder == null)
            Debug.LogWarning("HandBinder is not set - Expect errors on hand tracking status changes");
        _mammothRenderer = GetComponentInParent<MammothRenderer>();
        if(_mammothRenderer == null)
            Debug.LogWarning("MammothRenderer is not set - Expect errors on hand tracking status changes");        
        _xrInputModalityManager = GetComponentInParent<XRInputModalityManager>();
        if(_xrInputModalityManager == null)
            Debug.LogWarning("XR Input Modality Manager is not set - Expect errors on hand tracking status changes");

        if(XRIHandEnabled)
        {
            _leftHandMeshController = _xrInputModalityManager.leftHand.GetComponentInChildren<XRHandMeshController>();
            _rightHandMeshController = _xrInputModalityManager.rightHand.GetComponentInChildren<XRHandMeshController>();
        
            HideXRIHand();
        }
    }

    void FixedUpdate()
    {
       if(XRIHandEnabled) HideXRIHand();
    }

    protected override void HandReset()
    {
        if (FreezeHandState)
        {
            return;
        }

        gameObject.SetActive(true);
        MammothRendererActivate();

        if(XRIHandEnabled) HideXRIHand();  
    }

    protected override void HandFinish()
    {
        if (FreezeHandState)
        {
            return;
        }
        
        gameObject.SetActive(false);
        MammothRendererDeactivate();

        if(XRIHandEnabled) ShowXRIHand();
    }

    private void HideXRIHand()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                // _xrInputModalityManager.leftHand.SetActive(false);
                if(gameObject.activeInHierarchy)
                    _leftHandMeshController.handMeshRenderer.enabled = false;
                break;    
                
            case Chirality.Right:
                // _xrInputModalityManager.rightHand.SetActive(false);
                if(gameObject.activeInHierarchy)
                    _rightHandMeshController.handMeshRenderer.enabled = false;
                break;
        }

        // Debug.Log("Hide from "+ _handBinder.Chirality);
    }

    private void ShowXRIHand()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                if(!gameObject.activeInHierarchy)
                    _leftHandMeshController.handMeshRenderer.enabled = true;
                break;    
                
            case Chirality.Right:
                if(!gameObject.activeInHierarchy)
                    _rightHandMeshController.handMeshRenderer.enabled = true;
                break;
        }
        // Debug.Log("Show from "+ _handBinder.Chirality);
    }

    private void MammothRendererActivate()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                _mammothRenderer._leftHandActive = true;
                break;    
                
            case Chirality.Right:
                _mammothRenderer._rightHandActive = true;
                break;
        }
    }

    private void MammothRendererDeactivate()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                _mammothRenderer._leftHandActive = false;
                break;    
                
            case Chirality.Right:
                _mammothRenderer._rightHandActive = false;
                break;
        }
        
        _mammothRenderer.Disconnect();
    }
    
}
