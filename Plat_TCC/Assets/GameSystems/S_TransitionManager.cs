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
    private float openSpeed = 1f;

    [SerializeField]
    private float closeSpeed = 2f;

    [SerializeField]
    private SO_WorldInfo defaultWorldInfo;

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
        Debug.LogWarning("Loading level without mission is ?. Level name: " + levelName);
        StartCoroutine(LoadLevel(levelName, 0, null));
    }

    public void GoToLevelWithMission(string levelName, int missionIndex, SO_WorldInfo worldInfo)
    {
        StartCoroutine(LoadLevel(levelName, missionIndex, worldInfo));
        // Add logic to load the specific mission here
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartStage());
    }

    private IEnumerator LoadLevel(string levelName, int missionIndex = -1, SO_WorldInfo worldInfo = null)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 1f;
        float elapsedTime = 0;

        while (elapsedTime < openSpeed)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / openSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        Debug.Log("Attempting to load scene at LoadLevel()");
        SceneManager.LoadScene(levelName);
        Debug.Log("Scene load processed at LoadLevel()");

        Cursor.lockState = CursorLockMode.Locked;

        while (!SceneManager.GetSceneByName(levelName).isLoaded)
        {
            Debug.Log("Scene \"" + levelName + "\" has not loaded yet...");
            yield return null;
        }

        Debug.Log("Attempting to load mission");

        if (missionIndex != -1)
        {
            Debug.Log("Mission Index is not -1, proceeding");
            if (worldInfo == null)
            {
                worldInfo = defaultWorldInfo;
            }
            S_MissionManager.instance.SetWorldInfo(worldInfo);
            S_MissionManager.instance.StartMission((bool result) =>
            {
                if (result)
                {
                    Debug.Log("Mission loaded successfully.");
                    // Track collectibles in the new level
                    
                    S_PauseManager.instance.TrackCollectiblesInLevel(missionIndex, worldInfo);
                }
                else
                {
                    Debug.Log("Failed to load mission.");
                }
            }, missionIndex);
        }

        startAlpha = canvasGroup.alpha;
        endAlpha = 0;
        elapsedTime = 0;

        while (elapsedTime < closeSpeed)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / closeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        S_LevelManager.instance.PerformOnSceneLoad();
    }

    private IEnumerator RestartStage()
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 1f;
        float elapsedTime = 0;

        while (elapsedTime < openSpeed)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / openSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        S_LevelManager.instance.ResetLevel();

        yield return new WaitForSeconds(0.75f);

        startAlpha = canvasGroup.alpha;
        endAlpha = 0;
        elapsedTime = 0;

        while (elapsedTime < closeSpeed + 0.5f)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / (closeSpeed + 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
