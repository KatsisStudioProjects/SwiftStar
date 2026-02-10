using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private MeshRenderer spawnPlane;

    [Header("Assignments")]
    [SerializeField] private Transform playerTransform;

    public Transform PlayerTransform => playerTransform;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnEnemy();
        }
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnEnemy()
    {
        Bounds bounds = spawnPlane.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 spawnPosition = new Vector3(x, y, z);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}