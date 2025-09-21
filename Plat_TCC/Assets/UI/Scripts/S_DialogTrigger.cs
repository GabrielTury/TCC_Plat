using UnityEngine;

public class S_DialogTrigger : MonoBehaviour
{
    // In the inspector, select a string to specify the dialog type "single", "multiple", or "predefined"

    private S_DialogSystem dialogManager;

    public enum DialogType
    {
        Single,
        Multiple,
        Predefined
    }

    [SerializeField]
    public DialogType dialogType;

    [Header("Single Dialog Settings")]
    [SerializeField]
    private S_DialogSystem.Sentence singleSentence;

    [Header("Multiple Dialog Settings")]
    [SerializeField]
    private S_DialogSystem.Sentence[] multipleSentences;

    [Header("Predefined Dialog Settings")]
    [SerializeField]
    private SO_PredefinedDialog predefinedDialog;

    [SerializeField]
    private bool fireOnlyOnce = true;

    private bool hasSeen = false;

    private bool isInHitbox = false;

    void Start()
    {
        dialogManager = S_DialogSystem.instance;
    }

    private void FireDialog()
    {
        if (dialogManager == null) return;
        if (dialogType == DialogType.Single)
        {
            dialogManager.RaiseSingleSentence(singleSentence);
        }
        else if (dialogType == DialogType.Multiple)
        {
            dialogManager.RaiseMultipleSentence(multipleSentences);
        }
        else if (dialogType == DialogType.Predefined)
        {
            dialogManager.RaisePredefinedDialog(predefinedDialog);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fireOnlyOnce && hasSeen) return;
            if (isInHitbox) return;
            if (dialogManager.IsDialogActive()) return;
            FireDialog();
            isInHitbox = true;
            hasSeen = true;
            Debug.Log("Dialog Triggered at object: " + this.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInHitbox = false;
        }
    }
}
