using InfimaGames.LowPolyShooterPack;
using Outbreak;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
    
        [Header("Animation Procedural")]
        [Tooltip("Character Animator.")]
        [SerializeField]
        private Animator characterAnimator;
    
        private Equipment _equippedWeapon;
        private WeaponAttachmentManagerBehaviour _weaponAttachmentManager;
        private ScopeBehaviour _equippedWeaponScope;
        private MagazineBehaviour _equippedWeaponMagazine;

        private bool CanMove { get; set; } = true;
        private float initialFOV;
        private float adsFOV = 30.0f;
        private ControllerColliderHit _contact;
        [SerializeField] private bool canUseHeadbob = true;
        private bool _isSprinting = false;

        public float walkSpeed = 5;
        public float runSpeed = 8;

        public float minFall = -1.5f;
        private float _vertSpeed;

        public float jumpHeight = 15.0f;
        public float jumpCooldown = 0.25f;
        public float airMultiplier = 0.4f;
        private bool _readyToJump;
        private bool _exitingSlope;
        
        public float rotSpeed = 1.0f;

        [SerializeField] private bool canJump = true;


        [Header("Look Parameters")] 
        [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
        [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
        [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
        [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

        private Vector3 _moveDirection;
        private Vector2 _currentInput;

        private float rotationX = 0;

        [Header("Headbob Parameters")]
        [SerializeField] private float bobFrequency = 1.0f;   
        [SerializeField] private float bobAmount = 0.05f;      
        [SerializeField] private float sprintBobMultiplier = 1.5f; 

        private Vector3 _initialCameraPosition;
        private float _headbobTimer = 0;
        private bool _isSprintingLastFrame = false;

        private Camera _playerCamera;
        private CharacterKinematics _characterKinematics;
        private GunController _gunController;
        private Vector3 _originalPosition;

        public float groundDistance = 0.85f;
        public LayerMask groundLayer;
        public Transform player;
        public bool isGrounded;
        public float playerHeight;
        public float groundDrag;


        [Header("References")] 
        public Transform orientation;
        public Transform playerObj;
        private Rigidbody _rb;

        [Header("Sliding")] 
        public float maxSlideTime;

        public float slideForce;
        private float _slideTimer;
        public float slideYScale;
        private float _startYScale;

        private float _horizontalInput;
        private float _verticalInput;

        private bool _sliding;
        
        

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _vertSpeed = minFall;
            
            _readyToJump = true;
            
            _playerCamera = GetComponentInChildren<Camera>();
            initialFOV = _playerCamera.fieldOfView;
        
            _characterKinematics = GetComponent<CharacterKinematics>();
            _gunController = GetComponent<GunController>();
        
            _initialCameraPosition = _playerCamera.transform.localPosition;
            _originalPosition = transform.position;

        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            groundDistance = playerHeight * 0.5f + 0.2f;
            _startYScale = playerObj.localScale.y;
        }


        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (CanMove)
            {
                if (canUseHeadbob)
                {
                    // HandleHeadbob();
                }
            
                UpdateCameraRotation();
            }

        
            RaycastHit hit;
            isGrounded = GroundCheck.IsGrounded(transform, groundDistance, groundLayer);
            
            if (isGrounded)
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;
            
            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(transform.position, Vector3.down * groundDistance, rayColor);
        }


        private void FixedUpdate()
        {
            if (CanMove) 
                ProcessMovement();

            if (_sliding)
                HandleSlide();
        }


        void LateUpdate()
        {
            if (_equippedWeapon == null)
                return;
            
            // if (equippedWeaponScope == null)
            //     return;
                     
            if(_characterKinematics != null)
            {
                _characterKinematics.Compute();
            }
        }

        private void UpdateCameraRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotationX -= mouseY * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
            transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeedX, 0);
            player.rotation = Quaternion.Euler(rotationX, transform.eulerAngles.y, 0);
        }
        


        private void ProcessMovement()
        {
            float speed = GetMovementSpeed();
            CalculateMovementVector();
            ApplyMovementToController();
            if (Input.GetButtonDown("Jump") && isGrounded && _readyToJump)
            {
                _readyToJump = false;
                HandleJump();
                Invoke("ResetJump", jumpCooldown);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftControl) && (_verticalInput != 0 || _horizontalInput != 0))
            {
                Debug.Log("Sliding");
                StartSlide();
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) && _sliding)
            {
                StopSlide();
            }
        

        }
        
        
        private void CalculateMovementVector()
        {

            float horInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");
            Vector3 right = _rb.transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
        
            _moveDirection = (right * horInput) + (forward * vertInput);
            _moveDirection *= GetMovementSpeed();
            _moveDirection = Vector3.ClampMagnitude(_moveDirection, GetMovementSpeed());
        }

        private void HandleJump()
        {
            _exitingSlope = true;

            
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            _rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
        
        private void ResetJump()
        {
            _readyToJump = true;

            _exitingSlope = false;
        }

        private void HandleSlide()
        {
            Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
            _rb.AddForce(inputDirection.normalized * slideForce,ForceMode.Force);
            _slideTimer -= Time.deltaTime;
            
            if(_slideTimer <= 0)
                StopSlide();
        }

        private void StartSlide()
        {
            _sliding = true;
            playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            _slideTimer = maxSlideTime;
        }

        private void StopSlide()
        {
            _sliding = false;
            playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
        }
    
        private void HandleHeadbob()
        {
            // float horizontalInput = Input.GetAxis("Horizontal");
            // float verticalInput = Input.GetAxis("Vertical");
            bool isMoving = Mathf.Abs(_horizontalInput) > 0.1f || Mathf.Abs(_verticalInput) > 0.1f;

            if (isMoving && isGrounded)
            {
                _headbobTimer += Time.deltaTime * bobFrequency;

                float bobX = Mathf.Sin(_headbobTimer) * bobAmount;
                float bobY = Mathf.Cos(_headbobTimer * 2.0f) * bobAmount;

                Vector3 newCameraPosition = _initialCameraPosition + new Vector3(bobX, bobY, 0);

                if (_isSprinting)
                {
                    newCameraPosition.x *= sprintBobMultiplier;
                    newCameraPosition.y *= sprintBobMultiplier;
                }

                _playerCamera.transform.localPosition = newCameraPosition;
            }
            else
            {
                _playerCamera.transform.localPosition = Vector3.Lerp(_playerCamera.transform.localPosition, _initialCameraPosition, Time.deltaTime * 10.0f);
                _headbobTimer = 0;
            }
        }

        private void ApplyMovementToController()
        {
            _moveDirection.y = _vertSpeed;
            float speed = GetMovementSpeed();
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= speed;

            _rb.velocity = new Vector3(_moveDirection.x, _rb.velocity.y, _moveDirection.z);
        }

    
        private float GetMovementSpeed()
        {
            _isSprinting = Input.GetButton("Fire3");
            return Input.GetButton("Fire3") ? runSpeed : walkSpeed;
        }
    }
}
