using UnityEngine;
using System.Collections;

public class S_Moving_Platform : MonoBehaviour, IActivatableObjects
{
    public Transform target;
    public float duration = 2;

    Vector3 waypoint1;
    Vector3 waypoint2;

    [SerializeField] private AudioClip stoneMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoint1 = transform.position;
        waypoint2 = target.position;
    }

    public void Activate()
    {
        StartCoroutine(Move(duration));
    }

    IEnumerator Move(float time)
    {
        float elapsed = 0;

        while (elapsed < time)
        {
            transform.position = Vector3.Lerp(waypoint1, waypoint2, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = waypoint2;
        SwapDirections();
    }

    public void ToggleButtonInteraction()
    {
            Activate();
            AudioManager.instance.PlaySFX(stoneMove);
    }

    void SwapDirections()
    {
        Vector3 temp = waypoint1;
        waypoint1 = waypoint2;
        waypoint2 = temp;   
    }
}
