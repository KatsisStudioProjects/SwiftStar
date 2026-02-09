using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public string[] dialogueLines;
    public Sprite spriteA;
    public Sprite spriteB;
}