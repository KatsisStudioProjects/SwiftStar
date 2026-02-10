using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 90f;

    [Header("Engage")]
    [SerializeField] private float engageRange = 40f;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float overshoot = 15f;
    [SerializeField] private float spread = 5f;

    [Header("Shooting")]
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private float shootDamage = 10f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float inaccuracy = 5f;

    [Header("VFX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform cometPoint;
    [SerializeField] private LineRenderer tracer;
    [SerializeField] private Transform tracerStartPoint;

    private Vector3 _targetPoint;
    private bool _hasTarget;
    private bool _hasFacedTarget;
    private float _nextFireTime;
    private float _nextCometTime;

    private Transform Player => EnemyManager.Instance.PlayerTransform;

    private void Update()
    {
        Fly();
        TryShoot();
        EmitCometTrail();
    }

    #region Movement

    private void Fly()
    {
        if (NeedsNewTarget())
            PickTarget();

        Vector3 dir = (_targetPoint - transform.position).normalized;
        Quaternion goal = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private bool NeedsNewTarget()
    {
        if (!_hasTarget) return true;

        Vector3 toTarget = (_targetPoint - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toTarget);

        if (!_hasFacedTarget && dot > 0f)
            _hasFacedTarget = true;

        bool passed = _hasFacedTarget && dot < 0f;
        bool tooClose = Vector3.Distance(transform.position, Player.position) < minDistance;

        return passed || tooClose || Vector3.Distance(transform.position, _targetPoint) < 3f;
    }

    private void PickTarget()
    {
        Vector3 toPlayer = Player.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector3 dir = dist > 0.01f ? toPlayer / dist : transform.forward;

        if (dist < minDistance)
            dir = -dir;

        float proximity = Mathf.InverseLerp(engageRange, 0f, dist);
        float scale = Mathf.Lerp(1f, 0.15f, proximity);

        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        float s = spread * scale;
        Vector3 offset = perp * Random.Range(-s, s) + Vector3.up * Random.Range(-s, s);

        _targetPoint = Player.position + offset + dir * (overshoot * scale);
        _hasTarget = true;
        _hasFacedTarget = false;
    }

    #endregion

    #region Shooting

    private void TryShoot()
    {
        if (Vector3.Distance(transform.position, Player.position) > shootRange) return;
        if (Time.time < _nextFireTime) return;

        _nextFireTime = Time.time + 1f / fireRate;

        Vector3 start = transform.position;
        Vector3 noise = RandomOffset(inaccuracy);
        Vector3 dir = (transform.forward * shootRange + noise).normalized;
        Vector3 end = start + dir * shootRange;

        if (Physics.Raycast(start, dir, out RaycastHit hit, shootRange))
        {
            end = hit.point;
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(shootDamage);
                SpawnExplosion(hit.point, 0.8f, Color.red);
            }
        }

        DrawTracer(start, end);
    }

    #endregion

    #region VFX

    private void EmitCometTrail()
    {
        if (Time.time < _nextCometTime) return;
        _nextCometTime = Time.time + 0.2f;

        var clone = Instantiate(explosionPrefab, cometPoint.position, Quaternion.identity);
        if (clone != null) clone.transform.localScale = Vector3.one * 0.7f;
    }

    private void SpawnExplosion(Vector3 position, float scale, Color? color = null)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * scale;

        if (color.HasValue)
        {
            SpriteRenderer sr = explosion.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = color.Value;
        }
    }

    private void DrawTracer(Vector3 start, Vector3 end)
    {
        if (tracer == null) return;

        tracer.SetPosition(0, tracerStartPoint != null ? tracerStartPoint.position : start);
        tracer.SetPosition(1, end);
        tracer.enabled = true;

        CancelInvoke(nameof(HideTracer));
        Invoke(nameof(HideTracer), 0.05f);
    }

    private void HideTracer() => tracer.enabled = false;

    #endregion

    #region Damage

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            for (int i = 0; i < 4; i++)
                SpawnExplosion(transform.position + RandomOffset(3f), 2f);

            Destroy(gameObject);
        }
    }

    #endregion

    private static Vector3 RandomOffset(float range) =>
        new(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
}