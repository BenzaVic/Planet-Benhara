using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI counterText;
    public TextMeshProUGUI messageText;
    public int total = 5;

    [Header("Sound")]
    public AudioClip crystalPickupSound;

    int carrying = 0;
    int found = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Counter visible straight away
        if (counterText) counterText.gameObject.SetActive(true);
        UpdateUI();
        ClearMessage();
    }

    // Called by IntroSequence once the player is out of bed
    public void StartGame()
    {
        UpdateUI();
        ShowTimed("Power is low!\nFind 5 energy crystals and bring them to the generator\n<size=28>WASD move · Mouse look · Shift run · Space jump · E use</size>", 7f);
    }

    public void CollectCrystal()
    {
        carrying++;
        found++;
        UpdateUI();
        SoundFX.Play2D(crystalPickupSound, 0.8f);

        if (found == total)
        {
            CancelInvoke(nameof(ClearMessage));
            ShowMessage("All crystals found! Take them to the generator");
        }
        else ShowTimed("Crystal found!", 2f);
    }

    public int Carrying()
    {
        return carrying;
    }

    public void UseCrystals(int n)
    {
        carrying -= n;
        UpdateUI();
    }

    public void ShowMessage(string m)
    {
        if (messageText) messageText.text = m;
    }

    // Show a message that disappears by itself
    public void ShowTimed(string m, float seconds)
    {
        ShowMessage(m);
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), seconds);
    }

    void ClearMessage()
    {
        if (messageText) messageText.text = "";
    }

    void UpdateUI()
    {
        if (counterText) counterText.text = "Crystals found " + found + "/" + total + "   Carrying " + carrying;
    }
}