using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    public Transform bedPoint;        // eyes position when lying in bed
    public Transform standPoint;      // on the floor next to the bed
    public Generator generator;
    public CanvasGroup fadeScreen;    // optional black screen
    public float lyingPitch = -75f;   // -90 = straight up at ceiling, 0 = straight ahead

    [Header("Start screen")]
    public GameObject startScreen;    // title screen shown before the intro (optional)

    [Header("Timing")]
    public float fadeInTime = 1.5f;
    public float beforePowerCut = 1.5f;  // lying in bed before the lights go
    public float afterPowerCut = 1.5f;   // lying in the dark before getting up
    public float getUpTime = 2f;
    public float beforeMessage = 0.5f;   // standing, before the prompt appears

    PlayerMovement move;
    CharacterController cc;
    Transform cam;

    void Start()
    {
        move = GetComponent<PlayerMovement>();
        cc = GetComponent<CharacterController>();
        cam = move.cam;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        // Freeze the player and lie them in bed
        move.enabled = false;
        cc.enabled = false;

        float camHeight = cam.localPosition.y;
        Vector3 lyingPos = bedPoint.position - Vector3.up * camHeight;
        Quaternion lyingRot = Quaternion.Euler(0, bedPoint.eulerAngles.y, 0);
        transform.SetPositionAndRotation(lyingPos, lyingRot);
        cam.localRotation = Quaternion.Euler(lyingPitch, 0, 0);

        if (fadeScreen) fadeScreen.alpha = 1;

        // Title screen: wait for Enter, Space or a click
        if (startScreen)
        {
            startScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            while (true)
            {
                Keyboard kb = Keyboard.current;
                if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame ||
                    Mouse.current.leftButton.wasPressedThisFrame) break;
                yield return null;
            }
            startScreen.SetActive(false);
            yield return null;   // so the same key press doesn't do anything else
        }

        // Fade in from black
        if (fadeScreen)
        {
            for (float t = 0; t < 1; t += Time.deltaTime / fadeInTime)
            {
                fadeScreen.alpha = 1 - t;
                yield return null;
            }
            fadeScreen.alpha = 0;
        }

        // Lying in bed, lights on
        yield return new WaitForSeconds(beforePowerCut);

        // Power cut while still in bed
        yield return StartCoroutine(generator.PowerFailure());

        // Lying in the dark for a moment
        yield return new WaitForSeconds(afterPowerCut);

        // Get out of bed
        Vector3 standPos = standPoint.position;
        Quaternion standRot = Quaternion.Euler(0, standPoint.eulerAngles.y, 0);
        for (float t = 0; t < 1; t += Time.deltaTime / getUpTime)
        {
            float s = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(lyingPos, standPos, s);
            transform.rotation = Quaternion.Slerp(lyingRot, standRot, s);
            cam.localRotation = Quaternion.Euler(Mathf.Lerp(lyingPitch, 0, s), 0, 0);
            yield return null;
        }
        transform.SetPositionAndRotation(standPos, standRot);
        cam.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(beforeMessage);

        // Hand control to the player and show the prompt
        cc.enabled = true;
        move.enabled = true;
        GameManager.instance.StartGame();
    }
}