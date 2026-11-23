using UnityEngine;

public class TerminalScreen : MonoBehaviour
{
    [Header("Normal (power on)")]
    public Texture onlineTexture;            // TV_Hologram
    public Texture onlineEmission;           // TV_Hologram_Emission
    [TextArea(2, 4)]
    public string onlineMessage = "Resident Terminal: ID 33016222 · Planet Benhara · All systems online";

    [Header("Power cut")]
    public Texture offlineTexture;           // TV_Hologram_Offline
    public Texture offlineEmission;          // TV_Hologram_Offline_Emission
    [TextArea(2, 4)]
    public string offlineMessage = "WARNING: Power failure. Find 5 energy crystals and restart the generator.";

    Material mat;
    MessagePopup popup;
    bool showingOffline;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        popup = GetComponent<MessagePopup>();
        Apply(SignFlicker.powerDown);
    }

    void Update()
    {
        // Same switch the signs use: true after the power cut, false once powered
        if (SignFlicker.powerDown != showingOffline) Apply(SignFlicker.powerDown);
    }

    void Apply(bool offline)
    {
        showingOffline = offline;
        mat.SetTexture("_BaseMap", offline ? offlineTexture : onlineTexture);
        mat.SetTexture("_EmissionMap", offline ? offlineEmission : onlineEmission);
        if (popup) popup.message = offline ? offlineMessage : onlineMessage;
    }
}