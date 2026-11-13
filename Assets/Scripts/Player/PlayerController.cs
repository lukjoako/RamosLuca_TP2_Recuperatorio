using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -20f;

    [Header("Chequeo de Suelo (Raycast)")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.35f;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float speedMultiplier = 1f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        CheckGround();
        Move();
        JumpAndGravity();
    }

    private void CheckGround()
    {
        Vector3 origin = groundCheck != null ? groundCheck.position : transform.position;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    private void Move()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float horizontal = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                         - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
        float vertical   = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                         - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            transform.Rotate(0f, horizontal * turnSpeed * Time.deltaTime, 0f);
        }

        if (Mathf.Abs(vertical) > 0.01f)
        {
            Vector3 moveDir = transform.forward * vertical;
            controller.Move(moveDir * (moveSpeed * speedMultiplier) * Time.deltaTime);
        }
    }

    private void JumpAndGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = jumpForce;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.1f, 2f);
    }
}