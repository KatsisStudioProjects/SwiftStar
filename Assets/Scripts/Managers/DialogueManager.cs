using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterImage;

    [Header("Data")]
    [SerializeField] private DialogueSO dialogueSO;

    [Header("Settings")]
    [SerializeField] private float characterDelay = 0.05f;
    [SerializeField] private float delayBetweenLines = 1.5f;

    private Coroutine _typewriterCoroutine;

    [ContextMenu("Debug Dialogue")]
    public void DebugDialogue()
    {
        if (dialogueSO == null || dialogueSO.dialogueLines == null || dialogueSO.dialogueLines.Length == 0)
        {
            Debug.LogWarning("DialogueManager: No DialogueSO assigned or it has no lines.");
            return;
        }

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _typewriterCoroutine = StartCoroutine(PlayAllDialogues());
    }

    private IEnumerator PlayAllDialogues()
    {
        dialoguePanel.SetActive(true);

        for (int l = 0; l < dialogueSO.dialogueLines.Length; l++)
        {
            dialogueText.text = string.Empty;

            string line = dialogueSO.dialogueLines[l];
            for (int i = 0; i < line.Length; i++)
            {
                dialogueText.text += line[i];
                characterImage.sprite = (i % 2 == 0) ? dialogueSO.spriteA : dialogueSO.spriteB;
                yield return new WaitForSeconds(characterDelay);
            }

            if (l < dialogueSO.dialogueLines.Length - 1)
                yield return new WaitForSeconds(delayBetweenLines);
        }

        _typewriterCoroutine = null;
        dialoguePanel.SetActive(false);
    }
}