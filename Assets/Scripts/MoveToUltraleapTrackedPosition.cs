using System;
using UnityEngine;

public class MoveToUltraleapTrackedPosition : MonoBehaviour
{
    private Transform targetTransform;
    private int elapsedFrames = 0;
    [SerializeField] private String targetObjectName = "Ultraleap Tracking Offset Stable";
    [SerializeField] private int interpolationFramesCount = 45; // Number of frames to completely interpolate between the 2 positions
    
    private void Start()
    {
        GameObject target = GameObject.Find("Ultraleap Tracking Offset Stable");
        targetTransform = target?.transform;
    }

    private void FixedUpdate()
    {
        var currentPosition = transform.position;
        var targetPosition = targetTransform.position;
        var distance = Vector3.Distance(currentPosition, targetPosition);

        if (distance > 0.001f)
        {
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, interpolationRatio);

            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
        }
    }
}
