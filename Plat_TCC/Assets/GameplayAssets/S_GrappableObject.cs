using UnityEngine;

public class S_GrappableObject : MonoBehaviour
{
    [SerializeField]
    private Canvas grapplingCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Make the grapplingCanvas orbit around the object
        //grapplingCanvas.transform.RotateAround(transform.position, Vector3.up, orbitSpeed * Time.deltaTime);//the position must always be the closest to the player in certain radius

        // Make the grapplingCanvas always face the camera
        grapplingCanvas.transform.LookAt(Camera.main.transform.position);
    }
}
