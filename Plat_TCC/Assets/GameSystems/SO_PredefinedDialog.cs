using UnityEngine;

[CreateAssetMenu(fileName = "PredefinedDialog", menuName = "Dialog/PredefinedDialog")]
public class SO_PredefinedDialog : ScriptableObject
{
    public S_DialogSystem.Sentence[] sentences;
}
