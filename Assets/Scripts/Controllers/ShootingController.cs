using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Camera cam;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 25f;
    [SerializeField] float fireRate = 10f;

    [Header("Tracer")]
    [SerializeField] LineRenderer tracer;
    [SerializeField] float tracerDuration = 0.05f;
    [SerializeField] Transform tracerStartPoint;

    [Header("Explosion")]
    [SerializeField] GameObject explosionPrefab;

    bool isFiring;
    float nextFireTime;

    public void OnFire(InputAction.CallbackContext ctx)
    {
        isFiring = ctx.performed;
    }

    void Update()
    {
        if (isFiring)
            Fire();
    }

    void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + 1f / fireRate;

        Vector3 start = cam.transform.position;
        Vector3 direction = cam.transform.forward;
        Vector3 end = start + direction * range;

        if (Physics.Raycast(start, direction, out RaycastHit hit, range))
        {
            end = hit.point;

            if (hit.collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                SpawnExplosion(hit.point);
            }
        }

        DrawTracer(start, end);
    }

    void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * 0.8f;

        SpriteRenderer sr = explosion.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.blue;
    }

    void DrawTracer(Vector3 start, Vector3 end)
    {
        tracer.SetPosition(0, tracerStartPoint.position);
        tracer.SetPosition(1, end);
        tracer.enabled = true;

        CancelInvoke(nameof(HideTracer));
        Invoke(nameof(HideTracer), tracerDuration);
    }

    void HideTracer()
    {
        tracer.enabled = false;
    }
}
