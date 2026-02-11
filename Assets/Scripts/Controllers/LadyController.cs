using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LadyController : MonoBehaviour
{
    [SerializeField] private Material normalMat;
    [SerializeField] private Material censoredMat;
    public bool IsCensored = false;

    private List<SpriteRenderer> _spriteRenderers = new();
    private void Start()
    {
        _spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
        ToggleCensorship();
    }
    public void ToggleCensorship()
    {
        IsCensored = !IsCensored;
        foreach (var sr in _spriteRenderers)
        {
            sr.material = IsCensored ? censoredMat : normalMat;
        }
    }
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