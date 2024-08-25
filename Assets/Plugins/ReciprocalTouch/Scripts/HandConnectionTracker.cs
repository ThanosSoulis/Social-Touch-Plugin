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

        var rightHandTrackingEvents = _xrInputModalityManager.rightHand.GetComponentInChildren<XRHandTrackingEvents>();
        var leftHandTrackingEvents = _xrInputModalityManager.leftHand.GetComponentInChildren<XRHandTrackingEvents>();
    
        rightHandTrackingEvents.poseUpdated.AddListener(PoseDiff);
    }

    private void PoseDiff(Pose pose)
    {
        if(_handBinder == null)
            return;

        Pose diff = Pose.identity;
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                break;
                diff =pose.To(_handBinder.LeapHand.GetPalmPose());
                Debug.Log("Left Hand Diff Pos:"+diff.position+" Rot:"+diff.rotation);
                break;    
                
            case Chirality.Right:
                diff =pose.To(_handBinder.LeapHand.GetPalmPose());
                Debug.Log("Right Hand Diff Pos:"+diff.position+" Rot:"+diff.rotation);
                break;
        }
        
    }

    protected override void HandReset()
    {
        HideXRIHand();
        MammothRendererActivate();
    }

    protected override void HandFinish()
    {
        ShowXRIHand();
        MammothRendererDeactivate();
    }

    private void HideXRIHand()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                _xrInputModalityManager.leftHand.SetActive(false);
                break;    
                
            case Chirality.Right:
                _xrInputModalityManager.rightHand.SetActive(false);
                break;
        }
    }

    private void ShowXRIHand()
    {
        if(_handBinder == null)
            return;
        
        switch (_handBinder.Chirality)
        {
            case Chirality.Left:
                _xrInputModalityManager.leftHand.SetActive(true);
                break;    
                
            case Chirality.Right:
                _xrInputModalityManager.rightHand.SetActive(true);
                break;
        }
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
