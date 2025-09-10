using UnityEngine;

public class S_WindmillBg : MonoBehaviour
{
    [SerializeField]
    private float angle = 0;

    [SerializeField]
    private float angleSpeed = 2;

    // Update is called once per frame
    void FixedUpdate()
    {
        angle = angle + angleSpeed / 100;

        transform.Rotate(0, 0, angleSpeed * 1.8f / Mathf.PI);
    }
}
