using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 playerVelocity;
    private ControllerColliderHit _contact;

    public bool isOnGround = false;

    public float walkSpeed = 5;
    public float runSpeed = 8;

    public float minFall = -1.5f;
    private float _vertSpeed;

    public float jumpHeight = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float rotSpeed = 1.0f;

    private bool _isJumping = false;
    private bool _canJump = true;
    private const float JumpCooldown = 0.2f;
    private float _jumpCooldownTimer = 0f;

    private CharacterController _controller;
    private Camera _playerCamera;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _vertSpeed = minFall;

        _playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!_canJump)
        {
            _jumpCooldownTimer += Time.deltaTime;

            if (_jumpCooldownTimer >= JumpCooldown)
            {
                _canJump = true;
                _jumpCooldownTimer = 0f;
            }
        }

        if (_canJump && Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
            _canJump = false;
        }

        ProcessMovement();
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 cameraRotation = _playerCamera.transform.rotation.eulerAngles;
        cameraRotation.x -= mouseY;
        cameraRotation.y += mouseX;

        _playerCamera.transform.rotation = Quaternion.Euler(cameraRotation);
    }

    private void ProcessMovement()
    {
        float speed = GetMovementSpeed();
        Vector3 movement = CalculateMovementVector();

        RotateCharacter(movement);
        HandleJump();
        HandleGravity();

        ApplyMovementToController(movement);
        // _controller.GetComponent<Animator>().SetBool("IsGrounded", isOnGround);
    }

    private Vector3 CalculateMovementVector()
    {
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        Vector3 right = _controller.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        Vector3 movement = (right * horInput) + (forward * vertInput);
        movement *= GetMovementSpeed();
        movement = Vector3.ClampMagnitude(movement, GetMovementSpeed());

        return movement;
    }

    private void RotateCharacter(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (_controller.isGrounded && _isJumping)
        {
            _vertSpeed = jumpHeight;
            _isJumping = false;
        }
        else if (!_controller.isGrounded)
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;
            _vertSpeed = Mathf.Max(_vertSpeed, terminalVelocity);
        }
    }

    private void HandleGravity()
    {
        RaycastHit hit;
        float raycastDistance = _controller.height * 0.6f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            float check = (_controller.height + _controller.radius) / 1.9f;
            isOnGround = hit.distance <= check;
        }
        else
        {
            isOnGround = false;
        }
    }

    private void ApplyMovementToController(Vector3 movement)
    {
        movement.y = _vertSpeed;
        movement *= Time.deltaTime;
        _controller.Move(movement);
    }

    private float GetMovementSpeed()
    {
        return Input.GetButton("Fire3") ? runSpeed : walkSpeed;
    }
}
