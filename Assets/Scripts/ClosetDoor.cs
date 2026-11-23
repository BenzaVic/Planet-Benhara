using UnityEngine;

public class ClosetDoor : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [System.Serializable]
    public class DoorPart
    {
        public Transform door;
        public float openAngle = 100f;   // use a minus number to swing the other way
    }

    public Axis hingeAxis = Axis.Z;      // Blender models are usually Z (Blender's "up")
    public DoorPart[] doors;
    public float speed = 3f;             // higher = faster swing
    public string openPrompt = "Open closet";
    public string closePrompt = "Close closet";

    Quaternion[] closedRot;
    bool open = false;
    float t = 0f;
    Interactable interactable;

    void Start()
    {
        closedRot = new Quaternion[doors.Length];
        for (int i = 0; i < doors.Length; i++)
            closedRot[i] = doors[i].door.localRotation;

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

        Vector3 axis = hingeAxis == Axis.X ? Vector3.right : hingeAxis == Axis.Y ? Vector3.up : Vector3.forward;

        for (int i = 0; i < doors.Length; i++)
            doors[i].door.localRotation = closedRot[i] * Quaternion.AngleAxis(doors[i].openAngle * s, axis);
    }

    void UpdatePrompt()
    {
        if (interactable) interactable.prompt = open ? closePrompt : openPrompt;
    }
}