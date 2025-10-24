using UnityEngine;

public class S_Ventilator : MonoBehaviour, IActivatableObjects
{
    private bool isOnVentilator = true;
    private Rigidbody objRb;

    [SerializeField]
    private GameObject disabledVisualizer;

    [SerializeField]
    private GameObject fanObject;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float fanSpinSpeed;

    [SerializeField]
    private bool startDisabled;

    private Collider ownCollider;
    private S_PlayerMovement playerMove;

    private bool isBoxOnTop = false;

    private void Awake()
    {
        ownCollider = GetComponent<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startDisabled == true)
        {
            ToggleButtonInteraction();
        }
        playerMove = S_LevelManager.instance.playerMovement;

    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOnVentilator)
        {
            if (objRb != null)
            {
                if (disabledVisualizer.activeSelf & !isBoxOnTop)
                {
                    playerMove.AddPlayerForce(transform.parent.up * speed, ForceMode.Acceleration);
                    //objRb.AddForce(transform.parent.up * speed, ForceMode.Acceleration);
                }
            }
        }

        if (disabledVisualizer.activeSelf)
        {
            fanObject.transform.Rotate(0, Time.deltaTime * fanSpinSpeed, 0, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isOnVentilator = true;
        objRb = other.gameObject.GetComponentInParent<Rigidbody>();
        if (playerMove == null)
        {
            playerMove = FindFirstObjectByType<S_PlayerMovement>();
        }
        // if object has "box" in the name, consider it a box
        if (other.gameObject.name.ToLower().Contains("box"))
        {
            isBoxOnTop = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        isOnVentilator = false;
        objRb = null;
        if (other.gameObject.name.ToLower().Contains("box"))
        {
            isBoxOnTop = false;
        }
    }

    public void ToggleButtonInteraction()
    {
        this.gameObject.layer = (this.gameObject.layer == 6 ? 0 : 6);
        disabledVisualizer.SetActive(!disabledVisualizer.activeSelf);
        ownCollider.enabled = !ownCollider.enabled;
    }
}
