using UnityEngine;

public class DistributerChild : MonoBehaviour
{
    private MammothRenderer _mammothRenderer;

    private void Start()
    {
        _mammothRenderer = GetComponentInParent<MammothRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MammothInteractable interactable))
        {
            if(interactable.MammothRenderer == null)
                interactable.MammothRenderer = _mammothRenderer;
            
            interactable.AddContactPoint(transform);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MammothInteractable interactable))
        {
            interactable.RemoveContactPoint(transform);
            
            if (_mammothRenderer.IsRendererClear())
                interactable.MammothRenderer = null;
        }
    }

}
