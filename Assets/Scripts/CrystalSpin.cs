using UnityEngine;

public class CrystalSpin : MonoBehaviour
{
    public float spinSpeed = 60f;
    public float floatHeight = 0.05f;   // smaller bob so it stays in the socket
    public float bobSpeed = 2f;

    Vector3 startPos;
    float offset;

    void Start()
    {
        startPos = transform.position;
        offset = Random.Range(0f, 6f);   // so they don't all bob in sync
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed + offset) * floatHeight;
    }
}