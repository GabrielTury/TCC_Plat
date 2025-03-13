using UnityEngine;
using HF = S_HelperFunctions;

public class S_SettingsManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform settingsPanel;

    private IMenuCaller previousMenuScript;

    public static S_SettingsManager instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    public void OpenSettings(IMenuCaller originMenu)
    {
        StartCoroutine(HF.SmoothRectMove(settingsPanel, new Vector2(0, 0), 0.5f));
        previousMenuScript = originMenu;
    }

    private void CloseSettings()
    {
        previousMenuScript?.ResumeOperation();
    }
}
