using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

// Define the custom editor tool specifically for SO_MissionInfo
public class MissionObjectTagger : EditorWindow
{
    #region Visuals
    private ObjectField missionInfoField;
    private TextField tagField;
    private PopupField<string> targetArrayField;
    private Button populateButton;
    private Button clearButton;
    private Label statusLabel;
    #endregion

    private SO_MissionInfo targetMissionInfo;
    private string targetTag = "Insert Tag"; // Default tag
    private string[] arrayOptions = new string[] { "Mission Objects", "Mission Collectibles" };
    private string selectedArray = "Mission Objects"; // Default array choice

    [MenuItem("Tools/Mission Creator")]
    public static void ShowWindow()
    {
        MissionObjectTagger window = GetWindow<MissionObjectTagger>();
        window.titleContent = new GUIContent("Mission Creator");
        window.minSize = new Vector2(400, 300);
    }

    public void CreateGUI()
    {
        // Set up the root visual element
        VisualElement root = rootVisualElement;

        // Create a VisualElement for the header
        var headerContainer = new VisualElement();
        headerContainer.style.marginBottom = 10;
        headerContainer.style.marginTop = 10;

        // Add a title
        var titleLabel = new Label("Mission Craetor Tool");
        titleLabel.style.fontSize = 18;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerContainer.Add(titleLabel);

        // Add a description
        var descriptionLabel = new Label("This tool automates the process of creating levels by automatically assigning active objects to desired mission");
        descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
        headerContainer.Add(descriptionLabel);

        root.Add(headerContainer);

        // Create a container for the fields
        var fieldsContainer = new VisualElement();
        fieldsContainer.style.marginTop = 15;

        // Create the ScriptableObject field
        missionInfoField = new ObjectField("Mission Info")
        {
            objectType = typeof(SO_MissionInfo),
            allowSceneObjects = false
        };
        missionInfoField.RegisterValueChangedCallback(evt =>
        {
            targetMissionInfo = evt.newValue as SO_MissionInfo;
            UpdateStatusLabel();
        });
        fieldsContainer.Add(missionInfoField);

        // Create the target array selection field
        targetArrayField = new PopupField<string>("Target Array", arrayOptions.ToList(), 0);
        targetArrayField.RegisterValueChangedCallback(evt =>
        {
            selectedArray = evt.newValue;
            UpdateStatusLabel();
        });
        fieldsContainer.Add(targetArrayField);

        // Create the tag field
        tagField = new TextField("Target Tag");
        tagField.value = targetTag;
        tagField.RegisterValueChangedCallback(evt =>
        {
            targetTag = evt.newValue;
        });
        fieldsContainer.Add(tagField);

        root.Add(fieldsContainer);

        // Create a container for buttons
        var buttonsContainer = new VisualElement();
        buttonsContainer.style.flexDirection = FlexDirection.Row;
        buttonsContainer.style.justifyContent = Justify.SpaceBetween;
        buttonsContainer.style.marginTop = 20;

        // Create the populate button
        populateButton = new Button(PopulateGameObjects) { text = "Populate from Scene" };
        populateButton.style.flexGrow = 1;
        populateButton.style.marginRight = 5;
        buttonsContainer.Add(populateButton);

        // Create the clear button
        clearButton = new Button(ClearGameObjects) { text = "Clear Objects" };
        clearButton.style.flexGrow = 1;
        clearButton.style.marginLeft = 5;
        buttonsContainer.Add(clearButton);

        root.Add(buttonsContainer);

        // Add a status section
        var statusContainer = new VisualElement();
        statusContainer.style.marginTop = 15;
        statusContainer.style.marginBottom = 10;

        // Add status label
        statusLabel = new Label("No Mission Info selected");
        statusLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        statusContainer.Add(statusLabel);

        root.Add(statusContainer);
    }

    private void UpdateStatusLabel()
    {
        if (targetMissionInfo == null)
        {
            statusLabel.text = "No Mission Info selected";
            return;
        }

        int count = 0;

        if (selectedArray == "Mission Objects")
        {
            count = targetMissionInfo.missionObjectIds?.Length ?? 0;
            statusLabel.text = $"Mission Objects array has {count} GameObjects";
        }
        else if (selectedArray == "Mission Collectibles")
        {
            count = targetMissionInfo.missionCollectibleIds?.Length ?? 0;
            statusLabel.text = $"Mission Collectibles array has {count} GameObjects";
        }
    }

    private void PopulateGameObjects()
    {
        if (targetMissionInfo == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Mission Info ScriptableObject first.", "OK");
            return;
        }

        // Get all GameObjects with the specified tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        if (taggedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Warning", $"No GameObjects with tag '{targetTag}' found in the scene.", "OK");
            return;
        }

        // Record the object for undo
        Undo.RecordObject(targetMissionInfo, "Populate Mission Objects");

        // Handle direct reference assignment based on array type
        if (selectedArray == "Mission Objects")
        {
            #region Assing Id
            List<int> objectsIds = new List<int>();

            int objectIndex = 0;
            for (int i = 0; i < taggedObjects.Length; i++)
            {
                S_ObjectIdentifier scriptRef = taggedObjects[i].GetComponent<S_ObjectIdentifier>();
                if(scriptRef == null)
                    taggedObjects[i].AddComponent<S_ObjectIdentifier>();

                if (scriptRef != null)
                {
                    objectIndex++;

                    int resultId = -1;

                    int levelStartIndex = targetMissionInfo.name.IndexOf("_"); //where the levelId is Placed
                    int missionStartIndex = targetMissionInfo.name.IndexOf("-"); //where the missionId is placed
                    if (levelStartIndex != -1 && missionStartIndex != -1) //if both were found
                    {
                        int levelIdSize = (missionStartIndex - levelStartIndex) - 1;
                        if (int.TryParse(targetMissionInfo.name.Substring(levelStartIndex + 1, levelIdSize), out int levelIdInt))
                        {
                            resultId = levelIdInt * 100000;
                            targetMissionInfo.level = levelIdInt;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object, has incorrect level on its name, only numbers are allowed for level, check it again or report a bug in Discord", "OK");
                            return;
                        }

                        if (int.TryParse(targetMissionInfo.name.Substring(missionStartIndex + 1), out int missionIdInt))
                        {
                            resultId += missionIdInt * 1000;
                            targetMissionInfo.misisonIndex = missionIdInt;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object, has incorrect mission on its name, only numbers are allowed for mission, check it again or report a bug in Discord", "OK");
                            return;
                        }

                        resultId += objectIndex;

                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object has invalid name, check it again or report a bug in Discord", "OK");
                        return;
                    }
                    scriptRef.SetId(resultId);
                    objectsIds.Add(resultId);
                }
            }
            targetMissionInfo.missionObjectIds = objectsIds.ToArray();
#endregion
        }
        else // Mission Collectibles
        {
            // Create a new array of the correct size
            List<int> collectibleIds = new List<int>();

            int collectibleIndex = 0; //this will serve as index for objects with the collectible component
            // Copy each reference directly
            for (int i = 0; i < taggedObjects.Length; i++)
            {
                #region Assign ID
                S_Collectible scriptRef = taggedObjects[i].GetComponent<S_Collectible>();
                if (scriptRef != null)
                {
                    collectibleIndex++;

                    int resultId = -1;

                    int levelStartIndex = targetMissionInfo.name.IndexOf("_"); //where the levelId is Placed
                    int missionStartIndex = targetMissionInfo.name.IndexOf("-"); //where the missionId is placed
                    if (levelStartIndex != -1 && missionStartIndex != -1) //if both were found
                    {
                        int levelIdSize = (missionStartIndex - levelStartIndex) - 1;
                        if(int.TryParse(targetMissionInfo.name.Substring(levelStartIndex + 1, levelIdSize), out int levelIdInt))
                        {
                            resultId = levelIdInt * 100000;
                            targetMissionInfo.level = levelIdInt;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object, has incorrect level on its name, only numbers are allowed for level, check it again or report a bug in Discord", "OK");
                            return;
                        }

                        if (int.TryParse(targetMissionInfo.name.Substring(missionStartIndex + 1), out int missionIdInt))
                        {
                            resultId += missionIdInt * 1000;
                            targetMissionInfo.misisonIndex = missionIdInt;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object, has incorrect mission on its name, only numbers are allowed for mission, check it again or report a bug in Discord", "OK");
                            return;
                        }

                        resultId += collectibleIndex;

                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", $"Assigned Mission Info Object has invalid name, check it again or report a bug in Discord", "OK");
                        return;
                    }
                    scriptRef.SetId(resultId);
                    collectibleIds.Add(resultId);
                }
                #endregion

            }

            targetMissionInfo.missionCollectibleIds = collectibleIds.ToArray();
        }

        // Use EditorUtility.SetDirty to mark the asset as modified
        EditorUtility.SetDirty(targetMissionInfo);

        // Save all assets to disk
        AssetDatabase.SaveAssets();

        // Make sure scene is marked as dirty since we're referencing scene objects
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        UpdateStatusLabel();

        EditorUtility.DisplayDialog("Success", $"Found and assigned {taggedObjects.Length} GameObjects with tag '{targetTag}'.", "OK");
    }

    private void ClearGameObjects()
    {
        if (targetMissionInfo == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Mission Info ScriptableObject first.", "OK");
            return;
        }

        // Record the object for undo
        Undo.RecordObject(targetMissionInfo, "Clear Mission Objects");

        // Clear the array based on selection
        if (selectedArray == "Mission Objects")
        {
            targetMissionInfo.missionCollectibleIds = new int[0];
        }
        else // Mission Coins
        {
            targetMissionInfo.missionCollectibleIds = new int[0];
        }

        // Use EditorUtility.SetDirty to mark the asset as modified
        EditorUtility.SetDirty(targetMissionInfo);

        // Save all assets to disk
        AssetDatabase.SaveAssets();

        UpdateStatusLabel();

        EditorUtility.DisplayDialog("Success", $"{selectedArray} have been cleared.", "OK");
    }
}