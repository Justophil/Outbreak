using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private float initialFOV;
    private float adsFOV = 30.0f;
    public Transform gunTransform;
    private Vector3 initialGunPosition;
    private Vector3 targetGunPosition;
    public Vector3 playerVelocity;
    private ControllerColliderHit _contact;
    [SerializeField] private bool canUseHeadbob = true;
    private bool _isSprinting = false;
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

    [Header("Look Parameters")] 
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    [Header("Headbob Parameters")]
    [SerializeField] private float bobFrequency = 1.0f;   
    [SerializeField] private float bobAmount = 0.05f;      
    [SerializeField] private float sprintBobMultiplier = 1.5f; 

    private Vector3 initialCameraPosition;
    private float headbobTimer = 0;
    private bool isSprintingLastFrame = false;

    private CharacterController _controller;
    private Camera _playerCamera;
    

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _vertSpeed = minFall;

        _playerCamera = GetComponentInChildren<Camera>();
        initialFOV = _playerCamera.fieldOfView;

        initialCameraPosition = _playerCamera.transform.localPosition; // Store the original camera position
        
        // gunTransform = transform.GetChild(0).GetChild(0);
        // initialGunPosition = gunTransform.localPosition;
        // targetGunPosition = initialGunPosition;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (CanMove)
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
            
            if (canUseHeadbob)
            {
                HandleHeadbob();
            }

            ProcessMovement();
            UpdateCameraRotation();
        }
    }

    private void UpdateCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX -= mouseY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        _playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeedX, 0);

        gunTransform.rotation = _playerCamera.transform.rotation;


        // Vector3 cameraRotation = _playerCamera.transform.rotation.eulerAngles;
        // cameraRotation.x -= mouseY;
        // cameraRotation.y += mouseX;
        //
        // _playerCamera.transform.rotation = Quaternion.Euler(cameraRotation);
    }

    private void ProcessMovement()
    {
        float speed = GetMovementSpeed();
        CalculateMovementVector();
        RotateCharacter();
        HandleJump();
        HandleGravity();
        ApplyMovementToController();
    }

    private void CalculateMovementVector()
    {
        // currentInput = new Vector2(walkSpeed * Input.GetAxis("Horizontal"), walkSpeed * Input.GetAxis("Vertical"));
        // float moveDirectionY = moveDirection.y;
        // moveDirection =
        //     (transform.TransformDirection(Vector3.forward) * currentInput.x)
        //     + (transform.TransformDirection(Vector3.right) * currentInput.y);
        // moveDirection.y = moveDirectionY;

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        Vector3 right = _controller.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);
        
        moveDirection = (right * horInput) + (forward * vertInput);
        moveDirection *= GetMovementSpeed();
        moveDirection = Vector3.ClampMagnitude(moveDirection, GetMovementSpeed());
    }

    private void RotateCharacter()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion direction = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
            gunTransform.rotation = transform.rotation;
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

    private void HandleHeadbob()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;

        if (isMoving && _controller.isGrounded)
        {
            headbobTimer += Time.deltaTime * bobFrequency;

            float bobX = Mathf.Sin(headbobTimer) * bobAmount;
            float bobY = Mathf.Cos(headbobTimer * 2.0f) * bobAmount;

            Vector3 newCameraPosition = initialCameraPosition + new Vector3(bobX, bobY, 0);

            if (_isSprinting)
            {
                newCameraPosition.x *= sprintBobMultiplier;
                newCameraPosition.y *= sprintBobMultiplier;
            }

            _playerCamera.transform.localPosition = newCameraPosition;
        }
        else
        {
            _playerCamera.transform.localPosition = Vector3.Lerp(_playerCamera.transform.localPosition, initialCameraPosition, Time.deltaTime * 10.0f);
            headbobTimer = 0;
        }
    }
    
    private void ApplyMovementToController()
    {
        moveDirection.y = _vertSpeed;
        moveDirection *= Time.deltaTime;
        
        _controller.Move(moveDirection);
    }
    
    private float GetMovementSpeed()
    {
        if (Input.GetButton("Fire3"))
        {
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }
        return Input.GetButton("Fire3") ? runSpeed : walkSpeed;
    }
}
