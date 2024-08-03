using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathSensation))]
public class MammothRenderer : MonoBehaviour
{
    private PathSensation _pathSensation;

    public Transform ServiceProvider;
    private Vector3 UltraLeapAlignment = new Vector3(0f, 0.1210f, 0f);

    private float MinSmoothingSeparation = 0.008f;

    public bool DisableHaptics;
    public bool VizualiseInterpolation;
    private Vector3[] twoOptPoints = new Vector3[0];

    [Header("Interpolation")]
    public float DefaultInterpolationSeparation = 0.00035f;
    public float MinimumFrequency = 60f;
    public float MaximumFrequency = 200f;
    public bool DynamicCapping = true;

    private InterpolationClass _interpolationClass = new InterpolationClass();

    public float UpdateRate = 0.1f;
    private float _t;

    /*[Header("Filters")]
    public float SmoothingSeparation = 0.007f;
    public bool MovingAverage;
    [Range(1, 20)]
    public int mvaPeriod = 4;*/

    private List<Transform> _contacts = new List<Transform>();

    private Vector3[] _bufferPoints;
    private int _bufferPointsCount;
    public int PointBufferSize = 2048;
    private int[] _bufferEdgeIncrements;

    // For DrawGizmos
    //public bool VizualiseRaw;
    //public bool VizualiseExclusion;

    /*public bool VizualiseTwoOpt;
    public bool VizualiseInitial;
    public bool VizualiseBarreiro;
    private List<Vector3> preTransformPoints = new List<Vector3>();
    private List<Vector3> _rawPoints = new List<Vector3>();
    private List<Vector3> smoothPoints = new List<Vector3>();
    private List<Vector3> barreiroPoints = new List<Vector3>();*/


    private void OnValidate()
    {
        _interpolationClass.DefaultInterpolationSeparation = DefaultInterpolationSeparation;
        _interpolationClass.MinimumFrequency = MinimumFrequency;
        _interpolationClass.MaximumFrequency = MaximumFrequency;
        _interpolationClass.DynamicCapping = DynamicCapping;
    }

    private void Awake()
    {
        _bufferPoints = new Vector3[PointBufferSize];
        _bufferEdgeIncrements = new int[PointBufferSize];

        _pathSensation = GetComponent<PathSensation>();
    }
    void FixedUpdate()
    {
        _t += Time.deltaTime;

        if (_t > UpdateRate)
        {
            GetPointSnapshot();
            if (_bufferPointsCount > 0)
            {
                ShortestPath();
            }
            else
            {
                _pathSensation.SetEmptyPath();
            }
            _t -= UpdateRate;
        }
    }

    float time;

    private void ShortestPath()
    {
        // Smoothing: remove points within MinSmoothingSeparation from each other
        _bufferPointsCount = GraphFilters.SmoothPoints(_bufferPoints, _bufferPointsCount, MinSmoothingSeparation);

        // 2-Opt algorithm
        float distance = MathT.TwoOpt(_bufferPoints, _bufferPointsCount, 5);

        //Gizmo Visualization
        twoOptPoints = new Vector3[_bufferPointsCount];
        Array.Copy(_bufferPoints, twoOptPoints, _bufferPointsCount);

        // Transform from LeapMotion coordinate space to UltraHaptics space, accounting for position in Unity
        TransformContacts(_bufferPoints, _bufferPointsCount);

        // Get the dynamic interpolation separation
        float interpolationSeparation = MathT.GetInterpolationSeparation(distance, _interpolationClass, 40000);

        //Debug.Log("Frequency: " + (40000f / (distance/interpolationSeparation)));

        // Get the number ofinterpolated points to use per edge
        _bufferEdgeIncrements = MathT.GetEdgeIncrements(_bufferEdgeIncrements, _bufferPoints, _bufferPointsCount, interpolationSeparation);

        // Send to UltraHaptics
        if (DisableHaptics == false)
        {
            _pathSensation.SetPath(_bufferPoints, _bufferPointsCount, _bufferEdgeIncrements, interpolationSeparation);
        }
    }


    private void TransformContacts(Vector3[] contacts, int length)
    {
        for (int i = 0; i < length; i++)
        {
            OffsetPosition(contacts, i);
            TransformPosition(contacts, i);
        }
    }

    private void OffsetPosition(Vector3[] contacts, int i)
    {
        contacts[i] -= ServiceProvider.position;
    }

    private void TransformPosition(Vector3[] contacts, int i)
    {
        contacts[i] = new Vector3(contacts[i].x, -(-contacts[i].z - UltraLeapAlignment.y), contacts[i].y);
    }
    private void GetPointSnapshot()
    {
        _bufferPointsCount = _contacts.Count;
        for (int i = 0; i < _bufferPointsCount; i++)
        {
            _bufferPoints[i] = _contacts[i].position;
        }
    }

    public void AddContactPoint(Transform childTransform)
    {
        _contacts.Add(childTransform);
    }

    public void RemoveContactPoint(Transform childTransform)
    {
        _contacts.Remove(childTransform);
    }

    public void Disconnect()
    {
        _contacts.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (VizualiseInterpolation == true)
        {
            Gizmos.color = Color.blue;

            for (int i = 0; i < twoOptPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(twoOptPoints[i], twoOptPoints[i + 1]);
            }
        }

        /*if (VizualiseInterpolation == true)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < preTransformPoints.Count - 1; i++)
            {
                Gizmos.DrawSphere(preTransformPoints[i], 0.001f);
            }
            Gizmos.color = Color.blue;

            for (int i = 0; i < twoOptPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(twoOptPoints[i], twoOptPoints[i + 1]);
            }
        }
        if (VizualiseRaw == true)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < _rawPoints.Count; i++)
            {
                Gizmos.DrawSphere(_rawPoints[i], 0.001f);
            }
        }

        if (VizualiseExclusion == true)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < smoothPoints.Count; i++)
            {
                Gizmos.DrawSphere(smoothPoints[i], 0.001f);
            }
        }

        if (VizualiseTwoOpt == true)
        {
            Gizmos.color = Color.blue;

            for (int i = 0; i < twoOptPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(twoOptPoints[i], twoOptPoints[i + 1]);
            }
        }

        if (VizualiseInitial == true)
        {
            Gizmos.color = Color.blue;

            for (int i = 0; i < smoothPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(smoothPoints[i], smoothPoints[i + 1]);
            }
        }

        if (VizualiseBarreiro == true)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < barreiroPoints.Count - 1; i++)
            {
                Gizmos.DrawSphere(barreiroPoints[i], 0.001f);
            }
            Gizmos.color = Color.blue;

            for (int i = 0; i < twoOptPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(twoOptPoints[i], twoOptPoints[i + 1]);
            }
        }*/
    }
}
