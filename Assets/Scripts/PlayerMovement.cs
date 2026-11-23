using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cam;
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.2f;
    public float gravity = -15f;
    public float mouseSensitivity = 0.1f;

    // Changed by things like water (1 = normal)
    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public float lookMultiplier = 1f;

    CharacterController cc;
    float pitch;
    float yVel;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // When control comes back (after sitting / intro), continue from the camera's current angle
    void OnEnable()
    {
        if (cam)
        {
            float x = cam.localEulerAngles.x;
            if (x > 180f) x -= 360f;
            pitch = x;
        }
        yVel = 0f;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (kb.escapeKey.wasPressedThisFrame) Cursor.lockState = CursorLockMode.None;
        if (mouse.leftButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 d = mouse.delta.ReadValue() * mouseSensitivity * lookMultiplier;
            transform.Rotate(0, d.x, 0);
            pitch = Mathf.Clamp(pitch - d.y, -85f, 85f);
            cam.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        float x = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);
        float z = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);
        float speed = (kb.leftShiftKey.isPressed ? runSpeed : walkSpeed) * speedMultiplier;
        Vector3 move = (transform.right * x + transform.forward * z).normalized * speed;

        if (cc.isGrounded && yVel < 0) yVel = -2f;
        if (cc.isGrounded && kb.spaceKey.wasPressedThisFrame) yVel = Mathf.Sqrt(jumpHeight * speedMultiplier * -2f * gravity);

        yVel += gravity * Time.deltaTime;
        move.y = yVel;
        cc.Move(move * Time.deltaTime);
    }
}