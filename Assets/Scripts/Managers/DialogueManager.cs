using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterImage;

    [Header("Settings")]
    [SerializeField] private float characterDelay = 0.05f;
    [SerializeField] private float lineDelay = 0.5f;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialoguePanel != null && dialogueText != null)
            StartCoroutine(WarmUpDialogueUI());
    }

    // One-frame warm up so TMP / Canvas create their resources up front.
    private IEnumerator WarmUpDialogueUI()
    {
        bool wasActive = dialoguePanel.activeSelf;
        dialoguePanel.SetActive(true);

        dialogueText.text = " ";
        dialogueText.ForceMeshUpdate();

        yield return null;

        dialogueText.text = string.Empty;
        dialoguePanel.SetActive(wasActive);
    }

    public void PlayDialogue(DialogueSO dialogueSO, bool random = false, int lineIndex = -1)
    {
        if (dialogueSO == null || dialogueSO.dialogueLines == null || dialogueSO.dialogueLines.Length == 0)
        {
            Debug.LogWarning("DialogueManager: No DialogueSO assigned or it has no lines.");
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Preload sprite textures to avoid stalls
        if (dialogueSO.spriteA != null)
            _ = dialogueSO.spriteA.texture;
        if (dialogueSO.spriteB != null)
            _ = dialogueSO.spriteB.texture;

        if (random)
        {
            int index = Random.Range(0, dialogueSO.dialogueLines.Length);
            typingCoroutine = StartCoroutine(PlaySingleLine(dialogueSO, index));
        }
        else if (lineIndex >= 0 && lineIndex < dialogueSO.dialogueLines.Length)
        {
            typingCoroutine = StartCoroutine(PlaySingleLine(dialogueSO, lineIndex));
        }
        else
        {
            typingCoroutine = StartCoroutine(PlayAllLines(dialogueSO));
        }
    }

    private IEnumerator PlayAllLines(DialogueSO dialogueSO)
    {
        dialoguePanel.SetActive(true);

        for (int i = 0; i < dialogueSO.dialogueLines.Length; i++)
        {
            yield return TypeLine(dialogueSO, i);

            if (i < dialogueSO.dialogueLines.Length - 1)
                yield return new WaitForSeconds(lineDelay);
        }

        typingCoroutine = null;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator PlaySingleLine(DialogueSO dialogueSO, int index)
    {
        dialoguePanel.SetActive(true);

        yield return TypeLine(dialogueSO, index);
        yield return new WaitForSeconds(lineDelay);

        typingCoroutine = null;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeLine(DialogueSO dialogueSO, int index)
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