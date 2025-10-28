using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class S_SimpleDialogBubble : MonoBehaviour
{
    [SerializeField]
    private Image bubbleIcon;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private Vector2 iconPosition;
    private Vector2 iconSize;
    private float canvasAlpha = 0f;

    private Transform playerTransform;

    [SerializeField]
    [TextArea]
    private string dialogText = "Hello, this is a simple dialog bubble!";

    [SerializeField]
    private Image dialogBoxImage;

    [SerializeField]
    private TextMeshProUGUI dialogTextBox;

    private Vector3 dialogBoxPosition;
    private Vector3 dialogBoxSize;

    [SerializeField]
    private InputSystem_Actions inputs;

    private int iconIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        dialogTextBox.text = dialogText;
    }

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        //GameEvents.UIInputMade += UpdateControllerIcons;
        InputSystem.onActionChange += UpdateControllerIcons;
        inputs.Enable();
    }

    private void OnDisable()
    {
        //GameEvents.UIInputMade -= UpdateControllerIcons;
        InputSystem.onActionChange -= UpdateControllerIcons;
        inputs.Disable();
    }

    private void UpdateControllerIcons(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var inputAction = (InputAction)obj;
            var lastControl = inputAction.activeControl;
            var lastDevice = lastControl.device;

            // Or you can check the device type directly
            if (lastDevice is Gamepad)
            {
                // Check for specific gamepad types
                if (lastDevice.description.manufacturer.Contains("Sony"))
                {
                    iconIndex = 2;
                }
                else if (lastDevice.description.manufacturer.Contains("Microsoft"))
                {
                    // Xbox controller
                    iconIndex = 1;
                }
                else
                {
                    // Generic gamepad
                    iconIndex = 0;
                }
            }
            else if (lastDevice is Keyboard)
            {
                iconIndex = 0;
            }
        }//
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        HandleBubbleIconExhibition();
        HandleTextBoxExhibition();
    }

    private void HandleBubbleIconExhibition()
    {
        float iconSine = Mathf.Sin(Time.time * 2) * 0.25f;

        if (bubbleIcon != null)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) < 15f && Vector3.Distance(playerTransform.position, transform.position) > 8f)
            {
                iconPosition = Vector2.Lerp(iconPosition, new Vector2(0, 2.5f + iconSine), 0.1f);
                canvasAlpha = Mathf.Lerp(canvasAlpha, 1f, 0.1f);
                iconSize = Vector2.Lerp(iconSize, new Vector2(1f, 1f), 0.1f);

            }
            else
            {
                if (Vector3.Distance(playerTransform.position, transform.position) < 8f)
                {
                    canvasAlpha = Mathf.Lerp(canvasAlpha, 1f, 0.1f);
                    iconPosition = Vector2.Lerp(iconPosition, new Vector2(0, 2.5f + iconSine), 0.1f);
                    iconSize = Vector2.Lerp(iconSize, new Vector2(0, 0), 0.1f);

                } else
                {
                    canvasAlpha = Mathf.Lerp(canvasAlpha, 0f, 0.1f);
                    iconPosition = Vector2.Lerp(iconPosition, new Vector2(0, 0), 0.1f);
                    iconSize = Vector2.Lerp(iconSize, new Vector2(0, 0), 0.1f);
                }
            }
        }
        if (bubbleIcon != null)
        {
            bubbleIcon.rectTransform.anchoredPosition = iconPosition;
            bubbleIcon.rectTransform.localScale = iconSize;
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = canvasAlpha;
        }

        if (Camera.main != null)
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0; // Keep the rotation on the Y axis
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void HandleTextBoxExhibition()
    {
        float iconSine = Mathf.Sin(Time.time * 2) * 0.25f;

        // Calculate dialog box width based on text length
        float textWidth = dialogTextBox.preferredWidth;
        float dialogBoxWidth = Mathf.Clamp(textWidth, 0, 300); // Set a maximum width for the dialog box
        dialogBoxImage.rectTransform.sizeDelta = new Vector3(dialogBoxWidth + 1, dialogBoxImage.rectTransform.sizeDelta.y);

        dialogBoxPosition = Vector3.Lerp(dialogBoxPosition, new Vector3(0, 2.5f + iconSine), 0.1f);

        if (Vector3.Distance(playerTransform.position, transform.position) < 8f)
        {
            dialogBoxSize = Vector3.Lerp(dialogBoxSize, new Vector3(-1f, 1f), 0.12f);

            // Replace all occurrences of "index=" in the dialog text with the current iconIndex value
            var matches = System.Text.RegularExpressions.Regex.Matches(dialogText, @"index=\d+");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string numberPart = match.Value.Substring(6); // Extract the number after "index="
                if (int.TryParse(numberPart, out int indexValue))
                {
                    dialogText = dialogText.Replace(match.Value, $"index={iconIndex}");
                }
            }

        }
        else
        {
            dialogBoxSize = Vector3.Lerp(dialogBoxSize, new Vector3(0, 1f), 0.25f);
        }

        dialogBoxSize = new Vector3(dialogBoxSize.x, dialogBoxSize.y, 1);

        dialogBoxImage.rectTransform.anchoredPosition = dialogBoxPosition;
        dialogBoxImage.rectTransform.localScale = dialogBoxSize;
    }
}
