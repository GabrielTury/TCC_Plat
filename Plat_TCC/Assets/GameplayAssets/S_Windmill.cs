using UnityEngine;

public class S_Windmill : MonoBehaviour
{
    [SerializeField]
    private Transform[] platformList;

    [SerializeField]
    private float angle = 0;

    [SerializeField]
    private float angleSpeed = 2;

    [SerializeField]
    private float distance = 25;

    [SerializeField]
    private Transform center;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int index = 1;

        foreach(Transform t in platformList)
        {
            float baseAngle = index * Mathf.PI/2;
            Vector3 pos;
            pos.x = transform.position.x + distance * Mathf.Cos(angle + baseAngle);
            pos.y = transform.position.y + distance * Mathf.Sin(angle + baseAngle);
            pos.z = transform.position.z;

            t.position = pos;

            index++;
        }

        angle = angle + angleSpeed/100;

        center.Rotate(0,0, angleSpeed*1.8f/Mathf.PI);
    }
}
