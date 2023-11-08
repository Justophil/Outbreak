using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;

namespace Outbreak
{ 
    
    [RequireComponent(typeof(CharacterKinematics))]
    public class GunController : MonoBehaviour
    {
        // Weapon variables
        public Weapon[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon;
        private Transform gunTransform;
        private Vector3 initialGunPosition;
        private Vector3 targetGunPosition;
        private int currentIndex;

        // Camera and Character Controller
        private CharacterController _controller;
        private Camera _playerCamera;

        // FOV settings
        private float initialFOV;
        private float adsFOV = 30.0f;

        // Rotation variables
        private Vector3 currentRotation;
        private Vector3 targetRotation;

        // Hipfire recoil
        [SerializeField] private float recoilX;
        [SerializeField] private float recoilY;
        [SerializeField] private float recoilZ;

        // Movement settings
        [SerializeField] private float snappiness;
        [SerializeField] private float returnSpeed;
        private float smoothness = 5.0f;

        private bool isAiming = false;
        
        /// <summary>
        /// /////////////
        ///
        ///
        ///
        /// 
        /// </summary>
        #region FIELDS SERIALIZED

		[Header("Inventory")]
		
		[Tooltip("Inventory.")]
		[SerializeField]
		private InventoryBehaviour inventory;

		[Header("Cameras")]

		[Tooltip("Normal Camera.")]
		[SerializeField]
		private Camera cameraWorld;

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

		#endregion

		#region FIELDS

		/// <summary>
		/// True if the character is aiming.
		/// </summary>
		private bool aiming;
		/// <summary>
		/// True if the character is running.
		/// </summary>
		private bool running;
		/// <summary>
		/// True if the character has its weapon holstered.
		/// </summary>
		private bool holstered;
		
		/// <summary>
		/// Last Time.time at which we shot.
		/// </summary>
		private float lastShotTime;
		
		/// <summary>
		/// Overlay Layer Index. Useful for playing things like firing animations.
		/// </summary>
		private int layerOverlay;
		/// <summary>
		/// Holster Layer Index. Used to play holster animations.
		/// </summary>
		private int layerHolster;
		/// <summary>
		/// Actions Layer Index. Used to play actions like reloading.
		/// </summary>
		private int layerActions;

		/// <summary>
		/// Character Kinematics. Handles all the IK stuff.
		/// </summary>
		private CharacterKinematics characterKinematics;
		
		/// <summary>
		/// The currently equipped weapon.
		/// </summary>
		private WeaponBehaviour equippedWeapon;
		/// <summary>
		/// The equipped weapon's attachment manager.
		/// </summary>
		private WeaponAttachmentManagerBehaviour weaponAttachmentManager;
		
		/// <summary>
		/// The scope equipped on the character's weapon.
		/// </summary>
		private ScopeBehaviour equippedWeaponScope;
		/// <summary>
		/// The magazine equipped on the character's weapon.
		/// </summary>
		private MagazineBehaviour equippedWeaponMagazine;
		
		/// <summary>
		/// True if the character is reloading.
		/// </summary>
		private bool reloading;
		
		/// <summary>
		/// True if the character is inspecting its weapon.
		/// </summary>
		private bool inspecting;

		/// <summary>
		/// True if the character is in the middle of holstering a weapon.
		/// </summary>
		private bool holstering;

		/// <summary>
		/// Look Axis Values.
		/// </summary>
		private Vector2 axisLook;
		/// <summary>
		/// Look Axis Values.
		/// </summary>
		private Vector2 axisMovement;
		
		/// <summary>
		/// True if the player is holding the aiming button.
		/// </summary>
		private bool holdingButtonAim;
		/// <summary>
		/// True if the player is holding the running button.
		/// </summary>
		private bool holdingButtonRun;
		/// <summary>
		/// True if the player is holding the firing button.
		/// </summary>
		private bool holdingButtonFire;

		/// <summary>
		/// If true, the tutorial text should be visible on screen.
		/// </summary>
		private bool tutorialTextVisible;

		/// <summary>
		/// True if the game cursor is locked! Used when pressing "Escape" to allow developers to more easily access the editor.
		/// </summary>
		private bool cursorLocked;

		#endregion

		#region CONSTANTS

		/// <summary>
		/// Aiming Alpha Value.
		/// </summary>
		private static readonly int HashAimingAlpha = Animator.StringToHash("Aiming");

		/// <summary>
		/// Hashed "Movement".
		/// </summary>
		private static readonly int HashMovement = Animator.StringToHash("Movement");

		#endregion
		
		/// <summary>
		/// ////////
		///
		///
		///
		///
		/// 
		/// </summary>
        
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            _playerCamera = GetComponentInChildren<Camera>();
            // RecoilScript = transform.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
            // gunTransform = transform.GetChild(0).GetChild(0);
            initialFOV = _playerCamera.fieldOfView;
            



            // initialGunPosition = gunTransform.localPosition;
            // targetGunPosition = initialGunPosition;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Equipping Item 1...");
                Equip(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Equipping Item 2...");
                Equip(1);
            }

            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedTime);
            
            
            
            if (currentWeapon != null)
            {
                gunTransform.localRotation = Quaternion.Euler(currentRotation);
                HandleGunMovement();
            
                // Toggle ADS
                if (Input.GetMouseButtonDown(1))
                {
                    ToggleADS(true);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    ToggleADS(false);
                }    
            }
        }

        void Equip(int p_int)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            currentIndex = p_int;
            GameObject t_newEquipment = Instantiate(loadout[p_int].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newEquipment;
            // Assign the gun's transform to gunTransform
            gunTransform = t_newEquipment.transform;
        }

        public void RecoilFire()
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }

        private void ToggleADS(bool isAiming)
        {
            // Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
            Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);

            
            if (isAiming)
            {
                // Transition to ADS
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);
                // StartCoroutine(SetADS(_playerCamera.fieldOfView, adsFOV, 0.2f));
                // targetGunPosition = Vector3.zero; // Adjust gun position for ADS
                Debug.Log(t_anchor.position);
            }
            else
            {
                // Transition out of ADS
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);
                Debug.Log(t_anchor.position);

                // StartCoroutine(SetADS(_playerCamera.fieldOfView, initialFOV, 0.2f));
                // targetGunPosition = initialGunPosition; // Reset gun position for hip fire
            }
            
            this.isAiming = isAiming;
        }

        private IEnumerator SetADS(float startFOV, float targetFOV, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                _playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _playerCamera.fieldOfView = targetFOV;
        }


        private void HandleGunMovement()
        {
            float smoothness = 5.0f;
      

            //Fire Gun
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Firing...");
                // RecoilScript.RecoilFire();
                            
            }
            
            //ADS if Right Click is pressed
            if (Input.GetMouseButtonDown(1))
            {
                targetGunPosition.x = 0;
                
                StartCoroutine(SetADS(_playerCamera.fieldOfView, adsFOV, 0.2f));  // Adjust the duration as needed

            }
        
            // Check if Right Click is released to Hip Fire
            if (Input.GetMouseButtonUp(1))
            {
                targetGunPosition = initialGunPosition;
                StartCoroutine(SetADS(_playerCamera.fieldOfView, initialFOV, 0.2f));  // Adjust the duration as needed

            }

            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetGunPosition, Time.deltaTime * smoothness);    }
    }
    
}
