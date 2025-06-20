using UnityEngine;

public class S_ButtonActivator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objects;

    private Transform pressedButtonTransform;

    private Vector3 pressedButtonPosition;

    private int timer = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pressedButtonTransform = this.gameObject.transform;
        pressedButtonPosition = pressedButtonTransform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= 1;
        }
    }

    private void ToggleButtonInteraction()
    {
        foreach (GameObject obj in objects)
        {
            IActivatableObjects activatable = obj.GetComponent<IActivatableObjects>();
            if (activatable != null)
            {
                activatable.ToggleButtonInteraction();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer <= 0)
        {
            timer = 5;
            ToggleButtonInteraction();
            pressedButtonTransform.position = new Vector3(pressedButtonPosition.x, pressedButtonPosition.y - 0.2f, pressedButtonPosition.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressedButtonTransform.position = new Vector3(pressedButtonPosition.x, pressedButtonPosition.y, pressedButtonPosition.z);
    }
}
