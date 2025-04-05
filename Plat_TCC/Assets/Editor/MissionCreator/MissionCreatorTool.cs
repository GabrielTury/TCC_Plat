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
    private ObjectField missionInfoField;
    private TextField tagField;
    private PopupField<string> targetArrayField;
    private Button populateButton;
    private Button clearButton;
    private Label statusLabel;

    private SO_MissionInfo targetMissionInfo;
    private string targetTag = "Player"; // Default tag
    private string[] arrayOptions = new string[] { "Mission Objects", "Mission Coins" };
    private string selectedArray = "Mission Objects"; // Default array choice

    [MenuItem("Tools/Mission Object Tagger")]
    public static void ShowWindow()
    {
        MissionObjectTagger window = GetWindow<MissionObjectTagger>();
        window.titleContent = new GUIContent("Mission Object Tagger");
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
        var titleLabel = new Label("Mission Object Tagger Tool");
        titleLabel.style.fontSize = 18;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        headerContainer.Add(titleLabel);

        // Add a description
        var descriptionLabel = new Label("This tool helps you populate arrays in SO_MissionInfo with GameObjects from your scene based on a tag.");
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
            count = targetMissionInfo.missionObjects?.Length ?? 0;
            statusLabel.text = $"Mission Objects array has {count} GameObjects";
        }
        else if (selectedArray == "Mission Coins")
        {
            count = targetMissionInfo.missionCoins?.Length ?? 0;
            statusLabel.text = $"Mission Coins array has {count} GameObjects";
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
            // Create a new array of the correct size
            GameObject[] newArray = new GameObject[taggedObjects.Length];

            // Copy each reference directly
            for (int i = 0; i < taggedObjects.Length; i++)
            {
                newArray[i] = taggedObjects[i];
            }

            // Assign the array
            targetMissionInfo.missionObjects = newArray;
        }
        else // Mission Coins
        {
            // Create a new array of the correct size
            GameObject[] newArray = new GameObject[taggedObjects.Length];

            // Copy each reference directly
            for (int i = 0; i < taggedObjects.Length; i++)
            {
                newArray[i] = taggedObjects[i];
            }

            // Assign using the property
            targetMissionInfo.missionCoins = newArray;
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
            targetMissionInfo.missionObjects = new GameObject[0];
        }
        else // Mission Coins
        {
            targetMissionInfo.missionCoins = new GameObject[0];
        }

        // Use EditorUtility.SetDirty to mark the asset as modified
        EditorUtility.SetDirty(targetMissionInfo);

        // Save all assets to disk
        AssetDatabase.SaveAssets();

        UpdateStatusLabel();

        EditorUtility.DisplayDialog("Success", $"{selectedArray} have been cleared.", "OK");
    }
}