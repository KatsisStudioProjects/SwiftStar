using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;

    [Header("Look")]
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float maxPitch = 89f;

    Vector2 moveInput;
    Vector2 lookInput;

    float yaw;
    float pitch;
    bool autoForward;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            autoForward = !autoForward;
    }

    void HandleLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMovement()
    {
        Vector3 right = transform.right;
        Vector3 up = Vector3.up;
        Vector3 forward = transform.forward;

        Vector3 move =
            right * moveInput.x +  
            up * moveInput.y;

        if (autoForward)
            move += forward;

        float speed = autoForward ? sprintSpeed : moveSpeed;

        transform.position += move * speed * Time.deltaTime;
    }

}
