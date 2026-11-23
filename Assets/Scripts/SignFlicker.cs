using UnityEngine;
using System.Collections;

public class SignFlicker : MonoBehaviour
{
    // Set by the Generator: true after the power cut, false once the dome is powered
    public static bool powerDown = false;

    [Header("While the power is down")]
    public float offBrightness = 0.03f;                  // how dim the emission is when "off" (0 = none)
    public Vector2 waitBetween = new Vector2(3f, 8f);    // random wait in the dark between flickers
    public Vector2Int flickerCount = new Vector2Int(1, 4); // how many quick flickers each time
    public Vector2 flickerBrightness = new Vector2(0.4f, 1f); // how bright each flicker gets

    Material[] mats;
    Color[] baseGlow;
    float brightness = 1f;

    void Start()
    {
        // Remember each material's emission colour (works if the sign has several materials)
        Renderer rend = GetComponent<Renderer>();
        mats = rend.materials;
        baseGlow = new Color[mats.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].HasProperty("_EmissionColor"))
            {
                mats[i].EnableKeyword("_EMISSION");
                baseGlow[i] = mats[i].GetColor("_EmissionColor");
            }
        }

        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            // Power on: emission fully on
            if (!powerDown)
            {
                brightness = Mathf.MoveTowards(brightness, 1f, Time.deltaTime * 2f);
                Apply();
                yield return null;
                continue;
            }

            // Power down: dark
            brightness = offBrightness;
            Apply();

            // Wait a random time in the dark
            float wait = Random.Range(waitBetween.x, waitBetween.y);
            for (float w = 0; w < wait && powerDown; w += Time.deltaTime) yield return null;
            if (!powerDown) continue;

            // Quick flickers, then straight back to dark
            int count = Random.Range(flickerCount.x, flickerCount.y + 1);
            for (int i = 0; i < count; i++)
            {
                brightness = Random.Range(flickerBrightness.x, flickerBrightness.y);
                Apply();
                yield return new WaitForSeconds(Random.Range(0.04f, 0.12f));

                brightness = offBrightness;
                Apply();
                yield return new WaitForSeconds(Random.Range(0.06f, 0.2f));
            }
        }
    }

    void Apply()
    {
        for (int i = 0; i < mats.Length; i++)
            if (mats[i].HasProperty("_EmissionColor"))
                mats[i].SetColor("_EmissionColor", baseGlow[i] * brightness);
    }
}