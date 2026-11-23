using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Seat : MonoBehaviour
{
    public Transform sitPoint;          // where your eyes are when sitting (blue arrow = facing)
    public float moveTime = 0.5f;       // how long it takes to sit / stand
    public float lookRange = 100f;      // how far you can turn left/right while seated
    public float mouseSensitivity = 0.1f;
    public string standPrompt = "[Space] Stand up";

    [Header("Up / down view")]
    [Range(-80f, 80f)]
    public float lookAngle = 0f;        // starting view: minus = look up, plus = look down
    public float minLookAngle = -80f;   // how far up you can look while seated
    public float maxLookAngle = 80f;    // how far down you can look while seated

    Transform player, cam;
    PlayerMovement move;
    CharacterController cc;
    bool seated = false, busy = false;
    Vector3 standPos;
    float baseYaw, yaw, pitch;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        player = p.transform;
        move = p.GetComponent<PlayerMovement>();
        cc = p.GetComponent<CharacterController>();
        cam = move.cam;
    }

    public void Sit()
    {
        // Can't sit if already sitting, or if the player can't move (intro / another seat)
        if (seated || busy || !move.enabled) return;
        StartCoroutine(SitDown());
    }

    IEnumerator SitDown()
    {
        busy = true;

        // Remember where we stood so we can return there
        standPos = player.position;

        move.enabled = false;
        cc.enabled = false;

        Vector3 targetPos = sitPoint.position - Vector3.up * cam.localPosition.y;
        baseYaw = sitPoint.eulerAngles.y;
        Quaternion targetRot = Quaternion.Euler(0, baseYaw, 0);
        Quaternion targetCam = Quaternion.Euler(lookAngle, 0, 0);

        Vector3 p0 = player.position;
        Quaternion r0 = player.rotation;
        Quaternion c0 = cam.localRotation;

        for (float t = 0; t < 1f; t += Time.deltaTime / moveTime)
        {
            float s = Mathf.SmoothStep(0, 1, t);
            player.position = Vector3.Lerp(p0, targetPos, s);
            player.rotation = Quaternion.Slerp(r0, targetRot, s);
            cam.localRotation = Quaternion.Slerp(c0, targetCam, s);
            yield return null;
        }

        player.SetPositionAndRotation(targetPos, targetRot);
        cam.localRotation = targetCam;
        yaw = 0;
        pitch = lookAngle;

        // Let the player keep interacting while seated, and show the stand-up hint
        if (PlayerInteract.instance)
        {
            PlayerInteract.instance.seated = true;
            PlayerInteract.instance.seatedHint = standPrompt;
        }

        seated = true;
        busy = false;
    }

    void Update()
    {
        if (!seated || busy) return;

        // Look around while seated (limited turning)
        Vector2 d = Mouse.current.delta.ReadValue() * mouseSensitivity;
        yaw = Mathf.Clamp(yaw + d.x, -lookRange, lookRange);
        pitch = Mathf.Clamp(pitch - d.y, minLookAngle, maxLookAngle);
        player.rotation = Quaternion.Euler(0, baseYaw + yaw, 0);
        cam.localRotation = Quaternion.Euler(pitch, 0, 0);

        // Space to stand up (E is free for interacting)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            StartCoroutine(StandUp());
    }

    IEnumerator StandUp()
    {
        busy = true;
        seated = false;

        if (PlayerInteract.instance)
        {
            PlayerInteract.instance.seated = false;
            PlayerInteract.instance.seatedHint = "";
        }

        Vector3 p0 = player.position;
        Quaternion c0 = cam.localRotation;

        for (float t = 0; t < 1f; t += Time.deltaTime / moveTime)
        {
            float s = Mathf.SmoothStep(0, 1, t);
            player.position = Vector3.Lerp(p0, standPos, s);
            cam.localRotation = Quaternion.Slerp(c0, Quaternion.identity, s);
            yield return null;
        }

        player.position = standPos;
        cam.localRotation = Quaternion.identity;

        cc.enabled = true;
        move.enabled = true;
        busy = false;
    }
}