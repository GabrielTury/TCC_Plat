using UnityEngine;
using System.Collections;

public class S_Moving_Platform : MonoBehaviour, IActivatableObjects
{
    public Transform target;
    public float duration = 2;
    public bool moved = false;

    [SerializeField] private AudioClip stoneMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        Vector3 startPosition = transform.position;
        float elapsed = 0;

        while (elapsed < time)
        {
            transform.position = Vector3.Lerp(startPosition, destination, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
    }

    public void ToggleButtonInteraction()
    {
        if (!moved)
        {
            Activate();
            AudioManager.instance.PlaySFX(stoneMove);
            moved = true;
        }
    }
}
