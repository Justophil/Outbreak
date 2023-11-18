using System;
using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using Outbreak;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Determines how smooth the locomotion blendspace is.")]
    [SerializeField]
    private float dampTimeLocomotion = 0.15f;

    [Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
    [SerializeField]
    private float dampTimeAiming = 0.3f;

    [Header("Animation Procedural")]
    [Tooltip("Character Animator.")]
    [SerializeField]
    private Animator characterAnimator;
    
    [Header("Inventory")]
		
    [Tooltip("Inventory.")]
    [SerializeField]
    private InventoryBehaviour inventory;
    
    private Equipment equippedWeapon;
    private WeaponAttachmentManagerBehaviour weaponAttachmentManager;
    private ScopeBehaviour equippedWeaponScope;
    private MagazineBehaviour equippedWeaponMagazine;

    
    /// <summary>
    /// /////////
    /// </summary>
    public bool CanMove { get; private set; } = true;
    private float initialFOV;
    private float adsFOV = 30.0f;
    public Transform gunTransform;
    public Vector3 playerVelocity;
    private ControllerColliderHit _contact;
    [SerializeField] private bool canUseHeadbob = true;
    private bool _isSprinting = false;
    public bool isGrounded = false;

    public float walkSpeed = 5;
    public float runSpeed = 8;

    public float minFall = -1.5f;
    private float _vertSpeed;

    public float jumpHeight = 15.0f;
    public float rotSpeed = 1.0f;

    private bool _isJumping = false;
    private bool _canJump = true;


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

    private Rigidbody _controller;
    private Camera _playerCamera;
    private CharacterKinematics characterKinematics;
    private GunController _gunController;
    private Vector3 originalPosition;

    public float groundDistance = 0.85f;
    public LayerMask groundLayer;



    private void Awake()
    {
        _controller = GetComponent<Rigidbody>();
        _vertSpeed = minFall;

        _playerCamera = GetComponentInChildren<Camera>();
        initialFOV = _playerCamera.fieldOfView;
        
        characterKinematics = GetComponent<CharacterKinematics>();
        _gunController = GetComponent<GunController>();
        // //Initialize Inventory.
        // inventory.Init();
        
        //Refresh
        RefreshWeaponSetup();


        initialCameraPosition = _playerCamera.transform.localPosition; // Store the original camera position
        
        // gunTransform = transform.GetChild(0).GetChild(0);
        // initialGunPosition = gunTransform.localPosition;
        // targetGunPosition = initialGunPosition;
        
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.position;

    }
    /// <summary>
    /// 
    /// </summary>
    private void RefreshWeaponSetup()
    {
        //Weapon equip check
        if ((equippedWeapon = _gunController.GetEquipped()) == null)
            return;

        // weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
        // if (weaponAttachmentManager == null) 
        //     return;
			     
        equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();

        equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
    }
/// <summary>
/// 
/// </summary>

    private void Update()
    {
        if (CanMove)
        {
            if (canUseHeadbob)
            {
                HandleHeadbob();
            }
            
            ProcessMovement();
            UpdateCameraRotation();
        }
        
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position + new Vector3(0,1f, 0), Vector3.down, out hit, groundDistance, groundLayer);

        Color rayColor = grounded ? Color.green : Color.red;
        Debug.DrawRay(transform.position, Vector3.down * groundDistance, rayColor);
    }

    
    
    void LateUpdate()
    {
        // if (equippedWeapon == null)
        //     return;
        //
        // if (equippedWeaponScope == null)
        //     return;
			     //
                 
        //Make sure that we have a kinematics component!
        // if(characterKinematics != null)
        // {
        //     //Compute.
        //     characterKinematics.Compute();
        // }
    }

    private void UpdateCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX -= mouseY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeedX, 0);

        // gunTransform.rotation = _playerCamera.transform.rotation;

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
        // RotateCharacter();
        // HandleJump();
        // HandleGravity();
        ApplyMovementToController();
        // Debug.Log(IsGrounded());
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            HandleJump();
        }

    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.down, out hit, groundDistance, groundLayer);
    
        // Debug information
        if (grounded)
        {
            Debug.Log("Ground hit at: " + hit.point);
        }
        else
        {
            Debug.Log("Not grounded");
        }

        
        return grounded;
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
            // gunTransform.rotation = transform.rotation;
        }
    }

    private void HandleJump()
    {
            _controller.AddForce(new Vector3(0,jumpHeight,0), ForceMode.Impulse);
    }
    

    private void HandleHeadbob()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;

        if (isMoving && isGrounded)
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
        float speed = GetMovementSpeed();
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        _controller.velocity = new Vector3(moveDirection.x, _controller.velocity.y, moveDirection.z);
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
