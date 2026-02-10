using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float overshootDistance = 15f;
    [SerializeField] private float recalculateDistance = 3f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float lateralOffset = 5f;

    private Vector3 _targetPoint;
    private Vector3 _offset;
    private bool _hasTarget;
    private bool _hasFacedTarget;

    private void Update()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        Transform player = EnemyManager.Instance.PlayerTransform;

        Vector3 toTarget = _targetPoint - transform.position;
        float dot = Vector3.Dot(transform.forward, toTarget.normalized);

        // has faced the target at least once
        if (!_hasFacedTarget && dot > 0f)
            _hasFacedTarget = true;

        bool passedTarget = _hasFacedTarget && dot < 0f;

        if (!_hasTarget || passedTarget || Vector3.Distance(transform.position, _targetPoint) < recalculateDistance)
        {
            // Use cross prod to find a perpendicular direction to avoid crashing into player :p
            Vector3 dir = (player.position - transform.position).normalized;
            Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
            _offset = perp * Random.Range(-lateralOffset, lateralOffset)
                    + Vector3.up * Random.Range(-lateralOffset, lateralOffset);

            _targetPoint = player.position + _offset + dir * overshootDistance;
            _hasTarget = true;
            _hasFacedTarget = false;
        }

        Vector3 direction = (_targetPoint - transform.position).normalized;

        // Smoothly rotate toward the target instead of snapping
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Always move forward along the current facing direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {

    }
}