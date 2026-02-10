using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LadyController : MonoBehaviour
{
    private Transform _player;

    private void Update()
    {
        _player = EnemyManager.Instance.PlayerTransform;

        if (_player != null)
        {
            transform.LookAt(_player);
        }
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
