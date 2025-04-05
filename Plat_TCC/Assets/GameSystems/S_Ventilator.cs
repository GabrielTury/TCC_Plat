using UnityEngine;

public class S_Ventilator : MonoBehaviour, IActivatableObjects
{
    private bool isOnVentilator = true;
    private Rigidbody objRb;

    [SerializeField]
    private GameObject disabledVisualizer;

    [SerializeField]
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnVentilator)
        {
            if (objRb != null)
            {
                if (!disabledVisualizer.activeSelf)
                {
                    objRb.AddForce(transform.parent.up * speed, ForceMode.Acceleration);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isOnVentilator = true;
        objRb = other.gameObject.GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        isOnVentilator = false;
        objRb = null;
    }

    public void ToggleButtonInteraction()
    {
        this.gameObject.layer = (this.gameObject.layer == 6 ? 0 : 6);
        disabledVisualizer.SetActive(!disabledVisualizer.activeSelf);
    }
}
