using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private Transform _player;

    private void Awake()
    {
        Invoke("DestroySelf", .8f);
    }

    private void Update()
    {
        _player = EnemyManager.Instance.PlayerTransform;

        if (_player != null)
        {
            transform.LookAt(_player);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}