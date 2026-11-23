using UnityEngine;

public class Crystal : MonoBehaviour
{
    public float spinSpeed = 60f;
    public float floatHeight = 0.15f;
    Vector3 startPos;

    void Start() { startPos = transform.position; }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);   // spin
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * 2f) * floatHeight; // bob
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.CollectCrystal();
            Destroy(gameObject);
        }
    }
}