using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;


public class S_TransitionManager : MonoBehaviour
{
    public static S_TransitionManager instance;

    private CanvasGroup canvasGroup;

    [SerializeField]
    private RectTransform rectChild;

    [SerializeField]
    private float openSpeed = 2f;

    [SerializeField]
    private float closeSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        canvasGroup = GetComponent<CanvasGroup>();

        rectChild.anchoredPosition = new Vector2(0, 0);

        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToLevel(string levelName)
    {
        StartCoroutine(LoadLevel(levelName));
    }

    public void GoToLevelWithMission(string levelName, int missionIndex)
    {
        StartCoroutine(LoadLevel(levelName, missionIndex));
        // Add logic to load the specific mission here
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartStage());
    }

    private IEnumerator LoadLevel(string levelName, int missionIndex = -1)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * openSpeed;
            yield return null;
        }

        SceneManager.LoadScene(levelName);
        if (missionIndex != -1)
        {
            // Load specific mission here
        }

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * closeSpeed;
            yield return null;
        }
    }

    private IEnumerator RestartStage()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * openSpeed;
            yield return null;
        }

        S_LevelManager.instance.ResetLevel();

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * closeSpeed;
            yield return null;
        }
    }
}
