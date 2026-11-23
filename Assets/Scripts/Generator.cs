using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Generator : MonoBehaviour
{
    public GameObject[] socketCrystals;
    public Transform core;
    public GlowAnimator[] glowParts;
    public Light[] houseLights;
    public BeaconBeam beacon;
    public TextMeshProUGUI promptText;
    public float useDistance = 3f;
    public float dimAmount = 0.05f;      // how dark the lights get after the power cut (0 = off)
    public float messageTime = 5f;

    [Header("Sound")]
    public AudioSource hum;              // AudioSource on the generator with the hum loop
    public AudioClip powerDownSound;
    public AudioClip insertSound;
    public AudioClip powerUpSound;
    public float humOnVolume = 0.5f;
    public float humOffVolume = 0.08f;

    Transform player;
    float[] startIntensity;
    int inserted = 0;
    bool powered = false;
    float humTargetVolume, humTargetPitch;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        foreach (GameObject c in socketCrystals) c.SetActive(false);

        // Remember each light's real brightness (lights stay on until the power cut)
        startIntensity = new float[houseLights.Length];
        for (int i = 0; i < houseLights.Length; i++)
            startIntensity[i] = houseLights[i].intensity;

        SignFlicker.powerDown = false;   // signs steady during the intro

        // Hum running normally during the intro
        humTargetVolume = humOnVolume;
        humTargetPitch = 1f;
        if (hum)
        {
            hum.loop = true;
            hum.volume = humOnVolume;
            if (!hum.isPlaying) hum.Play();
        }

        if (promptText) promptText.text = "";
    }

    void Update()
    {
        // Smoothly move the hum towards its target volume and pitch
        if (hum)
        {
            hum.volume = Mathf.MoveTowards(hum.volume, humTargetVolume, Time.deltaTime * 0.5f);
            hum.pitch = Mathf.MoveTowards(hum.pitch, humTargetPitch, Time.deltaTime * 0.5f);
        }

        if (powered)
        {
            if (core) core.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            return;
        }

        Vector3 a = player.position; a.y = 0;
        Vector3 b = transform.position; b.y = 0;
        bool near = Vector3.Distance(a, b) < useDistance;
        int carrying = GameManager.instance.Carrying();

        if (near && carrying > 0)
        {
            if (promptText) promptText.text = "Press E to insert crystals (" + inserted + "/" + socketCrystals.Length + ")";

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                int used = 0;
                while (carrying > 0 && inserted < socketCrystals.Length)
                {
                    socketCrystals[inserted].SetActive(true);
                    inserted++;
                    carrying--;
                    used++;
                }
                GameManager.instance.UseCrystals(used);
                SoundFX.Play3D(insertSound, transform.position, 0.9f);

                if (inserted >= socketCrystals.Length) PowerOn();
            }
        }
        else if (promptText) promptText.text = "";
    }

    // Called by IntroSequence at the start of the game
    public IEnumerator PowerFailure()
    {
        SoundFX.Play2D(powerDownSound, 0.9f);

        for (int f = 0; f < 4; f++)
        {
            SetLights(dimAmount);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            SetLights(1f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        SetLights(dimAmount);

        // Hum winds down
        humTargetVolume = humOffVolume;
        humTargetPitch = 0.6f;

        SignFlicker.powerDown = true;    // signs start flickering
    }

    void PowerOn()
    {
        powered = true;
        if (promptText) promptText.text = "";

        SoundFX.Play2D(powerUpSound, 1f);

        // Hum winds back up, slightly stronger than before
        humTargetVolume = humOnVolume * 1.3f;
        humTargetPitch = 1.1f;

        foreach (GlowAnimator g in glowParts) g.SetPowered(true);

        SetLights(1f);

        SignFlicker.powerDown = false;   // signs back on for good

        if (beacon) beacon.Activate();

        // Game finished: hide the crystal counter
        if (GameManager.instance.counterText)
            GameManager.instance.counterText.gameObject.SetActive(false);

        GameManager.instance.ShowMessage("Generator Powered!\nThanks for Playing!");
        Invoke(nameof(HideMessage), messageTime);
    }

    void SetLights(float amount)
    {
        for (int i = 0; i < houseLights.Length; i++)
            houseLights[i].intensity = startIntensity[i] * amount;
    }

    void HideMessage()
    {
        GameManager.instance.ShowMessage("");
    }
}