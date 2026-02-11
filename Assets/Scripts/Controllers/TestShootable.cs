using System.Collections;
using UnityEngine;

public class TestShootable : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float bounceScale = 1.3f;
    [SerializeField] private float bounceDuration = 0.2f;

    [Header("Dialogue")]
    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private bool randomDialogue;
    [SerializeField] private int dialogueLineIndex = -1;

    private Material _materialInstance;
    private Vector3 _originalScale;
    private Color _originalEmission;
    private float _currentHealth;
    private bool _hasDied;

    [ColorUsage(false, true)]
    [SerializeField] private Color hurtEmission = new Color(0f, 0f, 5f, 1f); // HDR blue

    private void Awake()
    {
        _originalScale = transform.localScale;
        _currentHealth = maxHealth;

        var renderer = GetComponent<Renderer>();
        if(renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        _materialInstance = new Material(renderer.sharedMaterial);
        renderer.material = _materialInstance;
        _originalEmission = _materialInstance.GetColor("_Emission");
    }

    public void TakeDamage(float amount)
    {
        print($"{gameObject.name} took {amount} damage!");

        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        float healthPercent = _currentHealth / maxHealth;

        Color emission = Color.Lerp(hurtEmission, _originalEmission, healthPercent);
        _materialInstance.SetColor("_Emission", emission);

        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ScaleBounce());
    }

    private void Die()
    {
        if (_hasDied) return;
        _hasDied = true;
        if (dialogueSO != null && DialogueManager.Instance != null)
            DialogueManager.Instance.PlayDialogue(dialogueSO, randomDialogue, dialogueLineIndex);
    }

    private IEnumerator ScaleBounce()
    {
        Vector3 targetScale = _originalScale * bounceScale;
        float halfDuration = bounceDuration / 2f;

        // Scale up
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
            yield return null;
        }

        transform.localScale = _originalScale;
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
            Destroy(_materialInstance);
    }
}