using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract instance;

    public float range = 3f;
    public TextMeshProUGUI interactText;
    public bool debug = false;          // tick to print what you're looking at in the Console

    // Set by Seat while sitting: lets you keep interacting and shows the stand-up hint
    [HideInInspector] public bool seated = false;
    [HideInInspector] public string seatedHint = "";

    // Other scripts can put a message here to show on the interact text
    [HideInInspector] public string overrideText = "";

    PlayerMovement move;
    Transform playerRoot;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        move = GetComponentInParent<PlayerMovement>();
        playerRoot = move ? move.transform : transform;   // the Player object, not the whole scene
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(overrideText))
        {
            if (interactText) interactText.text = overrideText;
            return;
        }

        // No interacting during the intro (but allowed while seated)
        if (move && !move.enabled && !seated)
        {
            if (interactText) interactText.text = "";
            return;
        }

        Interactable current = FindInteractable(out string hitName);

        // While seated, ignore other seats
        if (seated && current && current.GetComponent<Seat>()) current = null;

        if (debug && Keyboard.current.eKey.wasPressedThisFrame)
            Debug.Log("Looking at: " + hitName + (current ? "  -> Interactable: " + current.name : "  -> nothing usable"));

        if (interactText)
        {
            if (current) interactText.text = "[E] " + current.prompt;
            else interactText.text = seated ? seatedHint : "";
        }

        if (current && Keyboard.current.eKey.wasPressedThisFrame)
            current.Interact();
    }

    // Looks along the crosshair. Trigger colliders only count if they have an Interactable
    // (so water zones, crystals etc. don't block). The first solid object stops the search.
    Interactable FindInteractable(out string hitName)
    {
        hitName = "nothing";
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, range, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Ignore the player's own collider only
            if (hit.collider.transform == playerRoot || hit.collider.transform.IsChildOf(playerRoot)) continue;

            Interactable it = hit.collider.GetComponentInParent<Interactable>();

            if (hit.collider.isTrigger)
            {
                if (it && it.enabled) { hitName = hit.collider.name; return it; }
                continue;   // a trigger with nothing to use - look past it
            }

            // A solid object: use it if it's interactable, otherwise it blocks the view
            hitName = hit.collider.name;
            return (it && it.enabled) ? it : null;
        }
        return null;
    }
}