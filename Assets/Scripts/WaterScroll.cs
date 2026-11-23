using UnityEngine;

public class WaterScroll : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.02f, 0.01f);
    Material mat;

    void Start() { mat = GetComponent<Renderer>().material; }

    void Update()
    {
        mat.SetTextureOffset("_BaseMap", speed * Time.time);
        mat.SetTextureOffset("_BumpMap", speed * Time.time);
    }
}