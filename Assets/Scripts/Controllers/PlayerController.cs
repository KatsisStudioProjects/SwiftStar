using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float rollMoveMultiplier = 2f;
    [SerializeField] float maxForwardSpeed = 50f;
    [Header("Look")]
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float maxPitch = 89f;

    [Header("Barrel Roll (Visual Only)")]
    [SerializeField] float rollDuration = 0.35f;
    [SerializeField] GameObject mesh;

    [Header("Assignments")]
    [SerializeField] TextMeshProUGUI speedText;

    Vector2 moveInput;
    Vector2 lookInput;

    float yaw;
    float pitch;
    float roll;

    bool autoForward;
    bool isRolling;
    float rollTimer;

    Vector2 scroll;
    float forwardSpeed;
    void Start()
    {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
        roll = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleBarrelRoll();
        HandleMovement();
        HandleScrollAdditive();
        
        speedText.text = autoForward ? $"Speed: {(autoForward ? sprintSpeed + forwardSpeed : moveSpeed):0.0}" : "Press SHIFT to go forward";
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

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isRolling)
            StartBarrelRoll();
    }
    public void OnScroll(InputAction.CallbackContext ctx)
    {
        scroll = ctx.ReadValue<Vector2>();
        print($"Scroll: {scroll.y}");
    }

    void HandleLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    void StartBarrelRoll()
    {
        isRolling = true;
        rollTimer = 0f;
    }

    void HandleBarrelRoll()
    {
        if (!isRolling)
        {
            roll = 0f;
            ApplyMeshRotation();
            return;
        }

        rollTimer += Time.deltaTime;
        float t = rollTimer / rollDuration;

        // Guaranteed full 360
        roll = Mathf.Lerp(0f, 360f, t);

        if (t >= 1f)
        {
            isRolling = false;
            roll = 0f;
        }

        ApplyMeshRotation();
    }

    void ApplyMeshRotation()
    {
        mesh.transform.localRotation = Quaternion.Euler(0f, 0f, roll);
    }

    void HandleScrollAdditive()
    {
        forwardSpeed = Mathf.Clamp(forwardSpeed + scroll.y, 0, maxForwardSpeed);
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

        float speed = autoForward ? sprintSpeed + forwardSpeed : moveSpeed;

        if (isRolling)
            speed *= rollMoveMultiplier;

        transform.position += move * speed * Time.deltaTime;
    }
}
