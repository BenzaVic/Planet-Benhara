using UnityEngine;

public class TurtleWander : MonoBehaviour
{
    public Transform head;
    public float radius = 0.8f;         // size of the circle he walks around
    public float speed = 0.25f;         // walking speed (metres per second)
    public float lookDistance = 4f;     // stops and looks at you when you're this close
    public bool flipFacing = true;      // tick/untick if he walks backwards
    public float turnSpeed = 3f;        // how fast he turns (lower = slower, smoother)

    [Header("Head bob")]
    public float bobSpeed = 6f;
    public float bobAmount = 0.015f;

    Vector3 centre;
    float angle;
    Transform player;
    Vector3 headStart;

    void Start()
    {
        centre = transform.position;
        GameObject p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
        if (head) headStart = head.localPosition;
    }

    Quaternion Face(Vector3 dir)
    {
        return Quaternion.LookRotation(flipFacing ? -dir : dir);
    }

    void Update()
    {
        Quaternion targetRot = transform.rotation;
        bool walking = false;

        // Is the player close?
        bool near = false;
        Vector3 toPlayer = Vector3.zero;
        if (player)
        {
            toPlayer = player.position - transform.position;
            toPlayer.y = 0;
            near = toPlayer.magnitude < lookDistance;
        }

        if (near)
        {
            // Stop and (slowly) turn to look at the player
            if (toPlayer.sqrMagnitude > 0.0001f) targetRot = Face(toPlayer.normalized);
        }
        else
        {
            // Walk around the circle
            angle += (speed / Mathf.Max(radius, 0.01f)) * Time.deltaTime;
            Vector3 next = centre + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Vector3 move = next - transform.position;
            move.y = 0;

            transform.position = new Vector3(next.x, transform.position.y, next.z);
            if (move.sqrMagnitude > 0.000001f) targetRot = Face(move.normalized);
            walking = true;
        }

        // Always turn smoothly - never snap
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Head bob while walking, settle when still
        if (head)
        {
            Vector3 localUp = head.parent.InverseTransformDirection(Vector3.up);
            float bob = walking ? Mathf.Sin(Time.time * bobSpeed) * bobAmount : 0f;
            head.localPosition = Vector3.Lerp(head.localPosition, headStart + localUp * bob, Time.deltaTime * 10f);
        }
    }
}