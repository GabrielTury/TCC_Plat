using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Image speakerImage;

    [SerializeField]
    private TextMeshProUGUI dialogText;

    [SerializeField]
    private Canvas dialogCanvas;

    public struct Sentence
    {
        public Sprite speakerSprite;
        public string speakerName;
        public string sentenceText;
    }

    public struct Choice
    {
        public string[] choices;
    }

    public struct Dialog
    {
        public Sentence[] sentences;
    }

    public void RaiseSingleSentence(Sentence sentence)
    {

    }

    public void RaiseMultipleSentence(Sentence[] sentence)
    {

    }

    public void RaisePredefinedDialog(Dialog dialog)
    {

    }
}
