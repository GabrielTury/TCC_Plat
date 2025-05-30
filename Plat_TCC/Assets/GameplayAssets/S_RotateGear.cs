using UnityEngine;
using static UnityEditor.PlayerSettings;

public class S_RotateGear : MonoBehaviour
{
    [SerializeField]
    private float rotate_speed, spin_speed, float_speed, height;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotate_speed);
        transform.Rotate(0, Time.deltaTime * spin_speed, 0,Space.World);

        float float_height = Mathf.Sin(Time.time * float_speed) * height / 10;
        transform.position = new Vector3(transform.position.x, transform.position.y + float_height, transform.position.z);

    }
}
