using UnityEngine;

public class DistributerChild : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MammothInteractable interactable))
        {
            interactable.AddContactPoint(transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MammothInteractable interactable))
        {
            interactable.RemoveContactPoint(transform);
        }
    }

}
