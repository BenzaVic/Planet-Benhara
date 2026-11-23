using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float speedInWater = 0.4f;    // 0.4 = 40% of normal walking speed
    [Range(0.1f, 1f)]
    public float lookInWater = 0.6f;     // 0.6 = 60% of normal mouse sensitivity

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement move = other.GetComponent<PlayerMovement>();
        if (move)
        {
            move.speedMultiplier = speedInWater;
            move.lookMultiplier = lookInWater;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement move = other.GetComponent<PlayerMovement>();
        if (move)
        {
            move.speedMultiplier = 1f;
            move.lookMultiplier = 1f;
        }
    }
}