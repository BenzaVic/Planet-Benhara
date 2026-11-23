using UnityEngine;
using UnityEngine.Rendering;

public class AutoDoor : MonoBehaviour
{
    public float openDistance = 2f;
    public float fadeSpeed = 4f;

    [Header("Sound")]
    public AudioClip whooshSound;
    [Range(0f, 1f)] public float volume = 0.7f;

    Transform player;
    Renderer rend;
    Collider col;
    Material mat;
    GameObject shadowBlocker;
    float alpha = 1f;
    bool wasNear = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        mat = rend.material;

        // The fading door itself doesn't need to cast shadows
        rend.shadowCastingMode = ShadowCastingMode.Off;

        // Create an invisible copy that only blocks light
        shadowBlocker = new GameObject("DoorShadowBlocker");
        shadowBlocker.transform.SetParent(transform, false);
        shadowBlocker.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer sr = shadowBlocker.AddComponent<MeshRenderer>();
        sr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        sr.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }

    void Update()
    {
        // Flat distance from the player to the middle of the door
        Vector3 a = player.position; a.y = 0;
        Vector3 b = rend.bounds.center; b.y = 0;
        bool near = Vector3.Distance(a, b) < openDistance;

        // Whoosh when the door starts opening or closing
        if (near != wasNear)
        {
            SoundFX.Play3D(whooshSound, rend.bounds.center, volume, 0.05f);
            wasNear = near;
        }

        // Fade in or out
        float target = near ? 0f : 1f;
        alpha = Mathf.MoveTowards(alpha, target, fadeSpeed * Time.deltaTime);

        Color c = mat.GetColor("_BaseColor");
        c.a = alpha;
        mat.SetColor("_BaseColor", c);

        // Solid and blocking light only when (nearly) closed
        if (col) col.enabled = alpha > 0.9f;
        rend.enabled = alpha > 0.01f;
        shadowBlocker.SetActive(alpha > 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Renderer r = GetComponent<Renderer>();
        if (!r) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(r.bounds.center, openDistance);
    }
}