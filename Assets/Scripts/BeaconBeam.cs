using UnityEngine;

public class BeaconBeam : MonoBehaviour
{
    public float height = 80f;
    public float width = 10f;
    public float growTime = 1.5f;

    [Header("Pulse (gentle, never closes)")]
    public float pulseSpeed = 15f;
    [Range(0f, 0.3f)]
    public float pulseAmount = 0.3f;

    public Light beaconLight;
    public float lightIntensity = 20f;

    bool active = false;
    float t = 0f;

    void Start()
    {
        transform.localScale = new Vector3(width, 0f, width);
        if (beaconLight) beaconLight.intensity = 0f;
    }

    public void Activate()
    {
        active = true;
    }

    void Update()
    {
        if (!active) return;

        t = Mathf.Min(1f, t + Time.deltaTime / growTime);

        // Shoot up, then hold full height
        float grow = 1f - Mathf.Pow(1f - t, 3f);

        // Slightly wider while firing up, settles to normal
        float flare = 1f + Mathf.Sin(t * Mathf.PI) * 0.4f;

        // Gentle pulse between (1 - pulseAmount) and 1
        float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float pulse = Mathf.Lerp(1f - pulseAmount, 1f, wave);

        float w = width * pulse * flare;
        transform.localScale = new Vector3(w, height * grow / 2f, w);

        if (beaconLight) beaconLight.intensity = lightIntensity * grow * pulse;
    }
}