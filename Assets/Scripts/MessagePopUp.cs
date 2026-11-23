using UnityEngine;

public class MessagePopup : MonoBehaviour
{
    // Same show time for every sign
    public const float showTime = 1.75f;

    [TextArea(2, 5)]
    public string message = "Write the sign's text here";

    public void Show()
    {
        GameManager.instance.ShowTimed(message, showTime);
    }
}