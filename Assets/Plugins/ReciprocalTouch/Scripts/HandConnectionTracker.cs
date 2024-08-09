using Leap.Unity;
using Leap.Unity.HandsModule;
using UnityEngine;

public class HandConnectionTracker : HandTransitionBehavior
{
    private MammothRenderer _mammothRenderer;
    private HandBinder _handBinder;

    private void Start()
    {
        _handBinder = GetComponent<HandBinder>();
        if(_handBinder == null)
            Debug.LogWarning("HandBinder is not set - Expect errors on hand tracking status changes");
        _mammothRenderer = GetComponentInParent<MammothRenderer>();
        if(_mammothRenderer == null)
            Debug.LogWarning("MammothRenderer is not set - Expect errors on hand tracking status changes");
    }

    protected override void HandReset()
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

    protected override void HandFinish()
    {
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
