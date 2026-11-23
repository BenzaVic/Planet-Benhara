using UnityEngine;

public class GlowAnimator : MonoBehaviour
{
    [Header("Colour")]
    public bool useMaterialColour = true;          // use the Emission colour already set on the material
    [ColorUsage(false, true)]
    public Color glowColour = new Color(0.75f, 0f, 0.58f) * 15f;  // used if the box above is unticked

    [Header("Scrolling")]
    public bool scrollTexture = true;              // untick for orbs / round objects
    public bool scrollSideways = false;
    public float idleScroll = 0.05f;
    public float poweredScroll = 0.6f;

    [Header("Brightness")]
    public float idleBrightness = 0.15f;           // 15% of full glow before power
    public float poweredBrightness = 1f;           // 100% = exactly how the material looks

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.3f;
    public bool randomTiming = true;               // stops every orb pulsing in sync

    Material mat;
    Color baseGlow;
    Vector2 offset;
    bool powered = false;
    float brightness;
    float timeOffset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        baseGlow = useMaterialColour ? mat.GetColor("_EmissionColor") : glowColour;
        brightness = idleBrightness;
        timeOffset = randomTiming ? Random.Range(0f, 10f) : 0f;
    }

    void Update()
    {
        // Scroll the texture (only if ticked)
        if (scrollTexture)
        {
            float scroll = powered ? poweredScroll : idleScroll;
            if (scrollSideways) offset.x += scroll * Time.deltaTime;
            else offset.y += scroll * Time.deltaTime;
            mat.SetTextureOffset("_BaseMap", offset);
            mat.SetTextureOffset("_EmissionMap", offset);
        }

        // Smoothly move towards the target brightness
        float target = powered ? poweredBrightness : idleBrightness;
        brightness = Mathf.MoveTowards(brightness, target, Time.deltaTime);

        // Pulse
        float pulse = 1f + Mathf.Sin((Time.time + timeOffset) * pulseSpeed) * pulseAmount;

        mat.SetColor("_EmissionColor", baseGlow * brightness * pulse);
    }

    public void SetPowered(bool on)
    {
        powered = on;
    }
}