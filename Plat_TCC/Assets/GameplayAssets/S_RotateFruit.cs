using UnityEngine;

public class S_RotateFruit : MonoBehaviour
{
    [SerializeField]
    private float spin_speed, float_speed, height;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + height / 10, transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime > 0)
        {
            transform.Rotate(0, Time.deltaTime * spin_speed, 0, Space.World);

            float float_height = Mathf.Sin(Time.time * float_speed) * height / 10;
            transform.position = new Vector3(transform.position.x, transform.position.y + float_height, transform.position.z);
        }
    }
}
