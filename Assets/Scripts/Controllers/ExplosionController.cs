using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private Transform _player;

    private void Awake()
    {
        _player = EnemyManager.Instance.PlayerTransform;
        Invoke("DestroySelf", .8f);
    }

    private void Update()
    {
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