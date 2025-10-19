using UnityEngine;

public class S_AutoEnableSlow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindFirstObjectByType<S_PlayerMovement>().EnableSlowedMovement(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
