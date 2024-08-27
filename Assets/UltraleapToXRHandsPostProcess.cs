using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.HandsModule;
using Leap.Unity.Preview.HandRays;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class UltraleapToXRHandsPostProcess : PostProcessProvider 
{
    [SerializeField] private XRInputModalityManager _xrInputModalityManager;
    private XRHandSkeletonDriver _leftHandDriver, _rightHandDriver;

    private void Awake()
    {
        if(_xrInputModalityManager == null)
            _xrInputModalityManager = GetComponentInParent<XRInputModalityManager>();
        if(_xrInputModalityManager == null)
            Debug.LogWarning("XR Input Modality Manager is not set - Expect errors on hand tracking status changes");

        _leftHandDriver = _xrInputModalityManager.leftHand.GetComponentInChildren<XRHandSkeletonDriver>();
        _rightHandDriver = _xrInputModalityManager.rightHand.GetComponentInChildren<XRHandSkeletonDriver>();
    }

    // For both Hands find the Palm pose of the OpenXR hand and set the Ultraleap hand Palm Pose to it 
    public override void ProcessFrame(ref Frame inputFrame)
    {
        // This mostly avoids a NullReferenceException in the console
        if(_leftHandDriver == null || _rightHandDriver == null)
            return;

        foreach (var hand in inputFrame.Hands)
        {
            var jointTransformReferences = 
                hand.IsLeft ? _leftHandDriver.jointTransformReferences.AsReadOnlyCollection() : _rightHandDriver.jointTransformReferences.AsReadOnlyCollection();

            Pose xrPalmPose = Pose.identity;
            foreach (var joint in jointTransformReferences)
            {
                if(joint.xrHandJointID == XRHandJointID.Palm)
                {
                    xrPalmPose = joint.jointTransform.ToWorldPose();
                    hand.SetPalmPose(xrPalmPose);

                    break;
                }  
            }
        }

    }
}
