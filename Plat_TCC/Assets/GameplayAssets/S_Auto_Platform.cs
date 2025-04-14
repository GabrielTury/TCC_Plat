using UnityEngine;
using System.Collections;

public class S_Auto_Platform : MonoBehaviour, IActivatableObjects
{
    public Transform target;
    public float duration = 2;
    public bool moved = false;
    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(Move(target.position, duration));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        StartCoroutine(Move(target.position, duration));
        Debug.Log("Move");
    }


    public void MoveToPoint()
    {

    }

    IEnumerator Move(Vector3 destination, float time)
    {
        float elapsed;
        while (true)
        {
            elapsed = 0; //resets time for trip forward

            while (elapsed < time)
            {
                transform.position = Vector3.Lerp(startPosition, destination, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = destination;
            
            elapsed = 0; //resets time for trip back

            while (elapsed < time)
            {
                transform.position = Vector3.Lerp(destination, startPosition, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = startPosition;
        } 
    }

    public void ToggleButtonInteraction()
    {
        if (!moved)
        {
            Activate();
        }
    }
}
