using UnityEngine;
using System.Collections;

public class S_Auto_Platform : MonoBehaviour, IActivatableObjects
{
    [SerializeField]
    private Transform waypoint1;
    [SerializeField]
    private Transform waypoint2;

    public float duration = 2;
    public bool moved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = waypoint2.position;
        StartCoroutine(Move(waypoint1.position, waypoint2.position, duration));
    }

    public void Activate()
    {
        StartCoroutine(Move(waypoint2.position, waypoint1.position, duration));
    }

    IEnumerator Move(Vector3 start, Vector3 end, float time)
    {
        float elapsed = 0; //resets time for trip forward

        while (elapsed < time)
        {
            float percentage = elapsed / time;
            percentage = Mathf.SmoothStep(0,1,percentage); //smoothing at the ends
            transform.position = Vector3.Lerp(start, end, percentage);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //swaps direction
        transform.position = end;
        end = start;
        start = transform.position;

        yield return new WaitForSeconds(1f);
        StartCoroutine(Move(start, end, duration));
    }

    public void ToggleButtonInteraction()
    {
        if (!moved)
        {
            Activate();
        }
    }

    /*
    //Sets player as child to be carried with platform
    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent.SetParent(null);
    }*/
}
