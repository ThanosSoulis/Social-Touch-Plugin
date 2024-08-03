using UnityEngine;
using System;
using Ultraleap.Haptics;
using SVector3 = System.Numerics.Vector3;
using System.Collections.Generic;

public class PathSensation : MonoBehaviour
{
    public bool DebugOn;

    public float ManualAlignmentX;
    public float ManualAlignmentY;
    public float ManualAlignmentZ;

    [Range(0, 1)]
    public float Intensity = 1f;
    private float _intensity;

    private bool _newPathAvailable;
    private Vector3[] _incomingPath;
    private int _incomingLength;
    private Vector3[] _path;
    private int[] _incomingEdgeIncrements = new int[0];
    //private int[] _edgeIncrements;
    private int _edgeIncrementSize;
    private float _incomingSeparation;
    private float _separation;

    private Vector3[] _bufferPath = new Vector3[2048];
    private int[] _bufferEdgeIncrements = new int[2048];

    private int _pathIncrement;
    private int _edgeIncrement;
    private int _pointIncrement;
    private int _length;

    private Vector3 position;

    private StreamingEmitter _emitter;
    private Ultraleap.Haptics.Transform _transform;

    float x;
    float y;
    float z;

    SVector3 _p = new SVector3();

    void Start()
    {
        _intensity = Intensity;

        using Library lib = new Library();
        lib.Connect();
        using IDevice device = lib.FindDevice();
        _emitter = new StreamingEmitter(lib);
        _emitter.Devices.Add(device);

        _transform = device.GetKitTransform();

        _emitter.SetControlPointCount(1, AdjustRate.None);
        _emitter.EmissionCallback = Callback;

        _emitter.Start();      
        Debug.Log("Start");

    }

    // This callback is called every time the device is ready to accept new control point information
    private void Callback(StreamingEmitter emitter, StreamingEmitter.Interval interval, DateTimeOffset submissionDeadline)
    {
        //Debug.Log("Callback");
        try
        {
            if (_newPathAvailable == true)
            {
                _length = _incomingLength;

                for (int i = 0; i < _length; i++)
                {
                    _bufferPath[i] = _incomingPath[i];
                    _bufferEdgeIncrements[i] = _incomingEdgeIncrements[i];
                }

                if (_length < 2)
                {
                    return;
                }

                _pointIncrement = 0;
                _edgeIncrement = 0;
                _separation = _incomingSeparation;
                _length = _incomingLength;
                _newPathAvailable = false;

                _edgeIncrementSize = MathT.GetEdgeIncrement(_bufferPath[0], _bufferPath[1], _separation);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        if (_length < 2)
        {
            return;
        }

        try
        {
            foreach (var sample in interval)
            {
                position = MathT.InterpolatePath(_bufferPath[_pointIncrement], _bufferPath[_pathIncrement + 1], _separation, _edgeIncrement);
                //_p.X = _path[_pointIncrement].x;
                //_p.Y = _path[_pointIncrement].y;
                //_p.Z = _path[_pointIncrement].z;

                _p.X = position.x + ManualAlignmentX;
                _p.Y = position.y + ManualAlignmentY;
                _p.Z = position.z + ManualAlignmentZ;

                sample.Points[0].Position = _p;
                sample.Points[0].Intensity = _intensity;

                _edgeIncrement++;
                if (_edgeIncrement == _bufferEdgeIncrements[_pointIncrement])
                {
                    _edgeIncrement = 0;

                    _pointIncrement++;
                    if (_pointIncrement == _length - 1)
                    {
                        _pointIncrement = 0;
                    }
                    //_edgeIncrementSize = Mathf.RoundToInt(Vector3.Distance(_bufferPath[_pointIncrement], _bufferPath[_pointIncrement+1]) / _separation);
                    //_edgeIncrementSize = MathT.GetEdgeIncrements(_bufferPath[_pointIncrement], _bufferPath[_pointIncrement+1], _separation);
                    //Debug.Log(_edgeIncrementSize);
                }

            }
            if (DebugOn)
            {
                Debug.Log("Post: " + _p.X + ", " + _p.Y + ", " + _p.Z);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    internal void SetEmptyPath()
    {
        _newPathAvailable = true;
        _incomingPath = new Vector3[0];
        _incomingLength = 0;
        //_incomingPath = new List<Vector3>();
        _intensity = 0f;
    }

    void OnDisable()
    {
        if (_emitter != null)
        {
            _emitter.Stop();
        }
    }

    void OnDestroy()
    {
        if (_emitter != null)
        {
            _emitter.Stop();
        }

    }

    public void SetPath(List<Vector3> path)
    {
        _intensity = Intensity;
        _newPathAvailable = true;
        //_incomingPath = path;
    }

    public void SetPath(Vector3[] path)
    {
        _intensity = Intensity;
        _newPathAvailable = true;
        _incomingPath = path;
    }

    public void SetPath(Vector3[] path, int length, int[] edgeIncrements, float separation)
    {
        _intensity = Intensity;
        _newPathAvailable = true;
        _incomingPath = path;
        _incomingEdgeIncrements = edgeIncrements;
        _incomingLength = length;
        _incomingSeparation = separation;
    }
}