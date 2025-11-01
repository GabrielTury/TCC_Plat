using TMPro;
using UnityEngine;

public class S_SimpleLocalize : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    private string dialogTextEN = "English";

    [SerializeField]
    [TextArea]
    private string dialogTextBR = "Brazilian";
    void Start()
    {
        if (S_SaveManager.instance == null) return;

        S_SaveManager.SettingsData settingsData = S_SaveManager.instance.GetSettingsData();

        Debug.LogWarning(settingsData.language.ToString());

        if (settingsData.language == "en")
        {
            GetComponent<TextMeshProUGUI>().text = dialogTextEN;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = dialogTextBR;
        }
    }
}
