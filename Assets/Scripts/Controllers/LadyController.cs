using UnityEngine;

public class LadyController : MonoBehaviour
{
    private void Update()
    {
        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}