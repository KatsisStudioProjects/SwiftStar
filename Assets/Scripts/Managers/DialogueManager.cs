using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterImage;

    [Header("Settings")]
    [SerializeField] private float characterDelay = 0.05f;
    [SerializeField] private float lineDelay = .5f;

    private Coroutine _typewriterCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayDialogue(DialogueSO dialogueSO, bool random = false, int lineIndex = -1)
    {
        if (dialogueSO == null || dialogueSO.dialogueLines == null || dialogueSO.dialogueLines.Length == 0)
        {
            Debug.LogWarning("DialogueManager: No DialogueSO assigned or it has no lines.");
            return;
        }

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        if (random)
        {
            int index = Random.Range(0, dialogueSO.dialogueLines.Length);
            _typewriterCoroutine = StartCoroutine(PlayLine(dialogueSO, index));
        }
        else if (lineIndex >= 0 && lineIndex < dialogueSO.dialogueLines.Length)
        {
            _typewriterCoroutine = StartCoroutine(PlayLine(dialogueSO, lineIndex));
        }
        else
        {
            _typewriterCoroutine = StartCoroutine(PlayAllLines(dialogueSO));
        }
    }

    private IEnumerator PlayAllLines(DialogueSO dialogueSO)
    {
        dialoguePanel.SetActive(true);

        for (int l = 0; l < dialogueSO.dialogueLines.Length; l++)
        {
            yield return PlayLineContent(dialogueSO, l);

            if (l < dialogueSO.dialogueLines.Length - 1)
                yield return new WaitForSeconds(lineDelay);
        }

        _typewriterCoroutine = null;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator PlayLine(DialogueSO dialogueSO, int index)
    {
        dialoguePanel.SetActive(true);

        yield return PlayLineContent(dialogueSO, index);
        yield return new WaitForSeconds(lineDelay);

        _typewriterCoroutine = null;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator PlayLineContent(DialogueSO dialogueSO, int index)
    {
        dialogueText.text = string.Empty;

        string line = dialogueSO.dialogueLines[index];
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            characterImage.sprite = (i % 2 == 0) ? dialogueSO.spriteA : dialogueSO.spriteB;
            yield return new WaitForSeconds(characterDelay);
        }
    }
}