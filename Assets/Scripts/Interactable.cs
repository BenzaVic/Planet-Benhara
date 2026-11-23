using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string prompt = "Interact";
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract.Invoke();
    }
}