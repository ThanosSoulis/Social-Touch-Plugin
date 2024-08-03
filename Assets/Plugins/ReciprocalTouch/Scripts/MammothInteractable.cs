using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MammothInteractable : MonoBehaviour
{
    private List<Transform> _contacts = new List<Transform>();
    public bool DrawGizmoSpheres;

    public MammothRenderer MammothRenderer;

    public void AddContactPoint(Transform childTransform)
    {
        MammothRenderer.AddContactPoint(childTransform);
        //_contacts.Add(childTransform);
    }

    public void RemoveContactPoint(Transform childTransform)
    {
        MammothRenderer.RemoveContactPoint(childTransform);
        //_contacts.Remove(childTransform);
    }

    void OnDrawGizmosSelected()
    {
        if (DrawGizmoSpheres == false)
        {
            return;
        }
        Gizmos.color = Color.green;

        for (int i = 0; i < _contacts.Count; i++)
        {
            Gizmos.DrawSphere(_contacts[i].position, 0.005f);

        }
    }

    public void Disconnect()
    {
        _contacts.Clear();
    }

    public List<Vector3> FetchPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < _contacts.Count; i++)
        {
            points.Add(_contacts[i].position);
        }
        return points;
    }
}
