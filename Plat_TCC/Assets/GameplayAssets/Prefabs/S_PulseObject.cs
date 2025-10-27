using System;
using UnityEngine;

public class S_PulseObject : MonoBehaviour
{
    private float scale;
    public float width;
    public float speed;
    public float size;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scale = size * (1 + MathF.Sin(speed * Time.time)/width);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
