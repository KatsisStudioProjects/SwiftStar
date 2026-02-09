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
                target.TakeDamage(damage);
        }

        DrawTracer(start, end);
    }

    void DrawTracer(Vector3 start, Vector3 end)
    {
        tracer.SetPosition(0, tracer.transform.position);
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
