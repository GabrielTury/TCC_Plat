using Unity.VisualScripting;
using UnityEngine;

public class S_RotatingGroundPlatform : MonoBehaviour
{
    [SerializeField]
    private float angleSpeed = 2;
    void FixedUpdate()
    {
        transform.Rotate(0, angleSpeed * 1.8f / Mathf.PI, 0);
    }
}
