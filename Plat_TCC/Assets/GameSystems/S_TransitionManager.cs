using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class S_TransitionManager : MonoBehaviour
{
    public static S_TransitionManager instance;

    private CanvasGroup canvasGroup;

    [SerializeField]
    private RectTransform rectChild;

    [SerializeField]
    private RawImage scrollerImage;

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
        // scroll the raw image

        float offsetY = Time.time * 0.5f; // Adjust the speed by changing the multiplier
        float offsetX = Time.time * 0.2f;
        scrollerImage.uvRect = new Rect(offsetX, -offsetY, 9, 9);
    }

    public void GoToLevel(string levelName)
    {
        Debug.LogWarning("Loading level without mission is ?. Level name: " + levelName);
        StartCoroutine(LoadLevel(levelName, 0, null));
    }

    public void GoToLevelWithMission(string levelName, int missionIndex, SO_WorldInfo worldInfo, SO_MissionUIInfo missionInfo)
    {
        StartCoroutine(LoadLevel(levelName, missionIndex, worldInfo, missionInfo));
        // Add logic to load the specific mission here
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartStage());
    }

    private IEnumerator LoadLevel(string levelName, int missionIndex = -1, SO_WorldInfo worldInfo = null, SO_MissionUIInfo missionInfo = null)
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

        if (S_DialogSystem.instance != null)
            S_DialogSystem.instance.ResetAndStopDialogueInstantly();

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

        S_LevelManager.instance.currentTimeChallengeInSeconds = missionInfo != null ? missionInfo.timeLimitInSeconds : 0;
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
