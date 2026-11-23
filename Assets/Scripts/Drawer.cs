using UnityEngine;

public class Drawer : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public Axis slideAxis = Axis.Y;     // the Position axis that changes in the Inspector
    public float openDistance = 0.4f;   // how far it slides out (minus number = other way)
    public float speed = 3f;            // higher = faster
    public string openPrompt = "Open drawer";
    public string closePrompt = "Close drawer";

    Vector3 closedPos;
    bool open = false;
    float t = 0f;
    Interactable interactable;

    void Start()
    {
        closedPos = transform.localPosition;
        interactable = GetComponent<Interactable>();
        UpdatePrompt();
    }

    public void Toggle()
    {
        open = !open;
        UpdatePrompt();
    }

    void Update()
    {
        t = Mathf.MoveTowards(t, open ? 1f : 0f, Time.deltaTime * speed);
        float s = Mathf.SmoothStep(0f, 1f, t);

        Vector3 axis = slideAxis == Axis.X ? Vector3.right : slideAxis == Axis.Y ? Vector3.up : Vector3.forward;
        transform.localPosition = closedPos + axis * openDistance * s;
    }

    void UpdatePrompt()
    {
        if (interactable) interactable.prompt = open ? closePrompt : openPrompt;
    }
}