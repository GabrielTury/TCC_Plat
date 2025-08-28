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
            int baseAngle = index * 90;
            Vector3 pos;
            pos.x = transform.localPosition.x + distance * Mathf.Cos(angle + baseAngle);
            pos.y = transform.localPosition.y + distance * Mathf.Sin(angle + baseAngle);
            pos.z = transform.localPosition.z;

            t.localPosition = pos;

            index++;
        }

        angle = angle + angleSpeed;
    }
}
