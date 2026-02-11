using UnityEngine;

public class GiantLadyHitboxController : MonoBehaviour, IDamageable
{
    [SerializeField] private bool destroyOnDefeat = false;
    [SerializeField] private GameObject[] clothingGameObjects;
    [SerializeField] private float totalHealth = 300f;
    [SerializeField] private GiantLadyHitboxController[] hitboxesToLock;
    [SerializeField] private LadyController ladyController;
    [SerializeField] private GameObject darkSphere;

    public bool isSphereLocked = true;
    public bool IsLocked = false;
    private float startingPieceHealth;
    private float currentPieceHealth;
    private int currentClothingIndex = 0;
    private bool isDestroyed = false;

    void Awake()
    {
        if(isSphereLocked && darkSphere) darkSphere.SetActive(true);
        startingPieceHealth = totalHealth > 0f ? totalHealth : 1f;
        currentPieceHealth = startingPieceHealth;

        if (clothingGameObjects != null && clothingGameObjects.Length > 0)
        {
            for (int i = 0; i < clothingGameObjects.Length; i++)
            {
                var go = clothingGameObjects[i];
                if (go) go.SetActive(i == currentClothingIndex);
            }
        }
    }
    private void Start()
    {
        if (hitboxesToLock == null || hitboxesToLock.Length == 0) return;
        foreach (var hitbox in hitboxesToLock)
        {
            if (hitbox)
            {
                hitbox.IsLocked = true;
                hitbox.gameObject.SetActive(false);
            }
        }
    }
    public void UnlockSphere() => darkSphere.SetActive(false);
    private void HandleDeath()
    {
        if (hitboxesToLock != null)
        {
            foreach (var hitbox in hitboxesToLock)
            {
                if (hitbox)
                {
                    hitbox.gameObject.SetActive(true);
                    hitbox.IsLocked = false;
                }
            }
        }

        if (destroyOnDefeat)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
    public void TakeDamage(float amount)
    {
        if (ladyController.IsCensored)return;
        if (IsLocked) return;
        if (isDestroyed) return;
        if (amount <= 0f) return;

        if (clothingGameObjects == null || clothingGameObjects.Length == 0)
        {
            HandleDeath();
            return;
        }

        float remainingDamage = amount;

        // Apply damage and allow spillover across multiple clothing pieces
        while (remainingDamage > 0f && currentClothingIndex < clothingGameObjects.Length)
        {
            currentPieceHealth -= remainingDamage;

            if (currentPieceHealth > 0f)
            {
                // Damage consumed by current piece
                remainingDamage = 0f;
            }
            else
            {
                // This piece is depleted. Turn it off.
                var currentGO = clothingGameObjects[currentClothingIndex];
                if (currentGO) currentGO.SetActive(false);

                // Calculate leftover damage to carry to next piece
                remainingDamage = -currentPieceHealth;

                // Move to next piece
                currentClothingIndex++;

                if (currentClothingIndex >= clothingGameObjects.Length)
                {
                    // No more pieces
                    HandleDeath();
                    return;
                }

                // Activate next piece and reset its health
                var nextGO = clothingGameObjects[currentClothingIndex];
                if (nextGO) nextGO.SetActive(true);
                currentPieceHealth = startingPieceHealth;
            }
        }
    }
}