using System;
using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Outbreak
{ 
    
    [RequireComponent(typeof(CharacterKinematics))]
    public class GunController : MonoBehaviour
    {
        // Weapon variables
        public Equipment[] loadout;
        public Equipment equipped;
        
        
        public Transform weaponParent;
        private GameObject currentWeapon;
        private Transform gunTransform;
        private Vector3 initialGunPosition;
        private Vector3 targetGunPosition;
        private int currentIndex = -1;

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
        
        float nextFire;

        
        /// <summary>
        /// /////////////
        ///
        ///
        ///
        /// 
        /// </summary>
       
		
		[Header("Animation Procedural")]
		
		[Tooltip("Character Animator.")]
		[SerializeField]
		private Animator characterAnimator;
		private int layerOverlay;
		private int layerHolster;
		
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
            initialFOV = _playerCamera.fieldOfView;
            loadout = GetComponentsInChildren<Equipment>(true);
            
            foreach (Equipment item in loadout)
	            item.gameObject.SetActive(false);


            int startingItem = 0;

            //Equip.
            Equip(startingItem);

            Debug.Log("Loadout count: " + loadout.Length);
            Debug.Log("Equipped weapon: " + equipped.gameObject.name);

            // initialGunPosition = gunTransform.localPosition;
            // targetGunPosition = initialGunPosition;
        }

		private void Start()
		{
			layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");
			characterAnimator.GetLayerIndex("Layer Holster");
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

            if (Input.GetMouseButtonDown(0))
            {
	            if (equipped.IsAutomatic())
	            {
		            Debug.Log("Rapid Fire");
		            InvokeRepeating("Fire", 0.01f,equipped.GetRateOfFire()/*equipped.GetRateOfFire() * (1/60)*/);
	            }
	            else
	            {
		            Debug.Log("Fire");

		            Fire();   
	            }
            }

            if (Input.GetMouseButtonUp(0))
            {
	            CancelInvoke();
            }
        
            // targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            // currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedTime);
            //
            
            
            if (equipped != null)
            {
                // // gunTransform.localRotation = Quaternion.Euler(currentRotation);
                // HandleGunMovement();
                // //
                // // // Toggle ADS
                if (Input.GetMouseButtonDown(1))
                {
	                Debug.Log("Aiming");
                    ToggleADS(true);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    ToggleADS(false);
                }    
            }
        }

        Equipment Equip(int index)
        {
	        if (loadout == null)
		        return equipped;
            
	        if (index > loadout.Length - 1)
		        return equipped;
	        
	        if (currentIndex == index)
		        return equipped;
            
	        //Disable the currently equipped weapon, if we have one.
	        if (equipped != null)
		        equipped.gameObject.SetActive(false);

	        
	        //layerHolster
	        layerHolster = characterAnimator.GetLayerIndex("Layer Holster");

	        
	        //Update index.
	        currentIndex = index;
	        //Update equipped.
	        equipped = loadout[currentIndex];
	        //Activate the newly-equipped weapon.
	        equipped.gameObject.SetActive(true);
	        characterAnimator.Play("Unholster", layerHolster, 0);

	        MonoBehaviour[] scriptsOnItem = equipped.GetComponents<MonoBehaviour>();
	        foreach (var script in scriptsOnItem)
	        {
		        script.enabled = true;
	        }
			RefreshWeaponSetup();
	        //Return.
	        return equipped;
        }
        
        public Equipment GetEquipped() => equipped;

        private void RefreshWeaponSetup()
        {
	        //Weapon equip check
	        if (equipped == null)
		        return;
	        characterAnimator.runtimeAnimatorController = equipped.GetAnimatorController();

	        // weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
	        // if (weaponAttachmentManager == null) 
	        //     return;
	        //
	        // equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
	        //
	        // equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
        }

        private void Fire()
        {
	        equipped.Fire();
	        characterAnimator.CrossFade("Fire", 0.05f, layerOverlay, 0);
        }
        
        public void RecoilFire()
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }

        private void ToggleADS(bool isAiming)
        {
	        characterAnimator.SetBool("Aim", isAiming);

	        
            // // Transform t_anchor = currentWeapon.transform.Find("Anchor");
            // Transform t_anchor = currentWeapon.transform.Find("Anchor");
            // Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
            // Transform t_state_hip = currentWeapon.transform.Find("States/Hip");
            //
            // t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);
            //
            //
            // if (isAiming)
            // {
            //     // Transition to ADS
            //     t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);
            //     // StartCoroutine(SetADS(_playerCamera.fieldOfView, adsFOV, 0.2f));
            //     // targetGunPosition = Vector3.zero; // Adjust gun position for ADS
            //     Debug.Log(t_anchor.position);
            // }
            // else
            // {
            //     // Transition out of ADS
            //     t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.fixedDeltaTime * loadout[currentIndex].aimSpeed);
            //     Debug.Log(t_anchor.position);
            //
            //     // StartCoroutine(SetADS(_playerCamera.fieldOfView, initialFOV, 0.2f));
            //     // targetGunPosition = initialGunPosition; // Reset gun position for hip fire
            // }
            //
            // this.isAiming = isAiming;
        }

        // private IEnumerator SetADS(float startFOV, float targetFOV, float duration)
        // {
        //     float elapsedTime = 0f;
        //
        //     while (elapsedTime < duration)
        //     {
        //         _playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }
        //
        //     _playerCamera.fieldOfView = targetFOV;
        // }


    // private void HandleGunMovement()
    // {
    //     float smoothness = 5.0f;
    //
    //
    //     //Fire Gun
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Debug.Log("Firing...");
    //         // RecoilScript.RecoilFire();
    //                     
    //     }
    //     
    //     // //ADS if Right Click is pressed
    //     // if (Input.GetMouseButtonDown(1))
    //     // {
    //     //     targetGunPosition.x = 0;
    //     //     
    //     //     StartCoroutine(SetADS(_playerCamera.fieldOfView, adsFOV, 0.2f));  // Adjust the duration as needed
    //     //
    //     // }
    //     //
    //     // // Check if Right Click is released to Hip Fire
    //     // if (Input.GetMouseButtonUp(1))
    //     // {
    //     //     targetGunPosition = initialGunPosition;
    //     //     StartCoroutine(SetADS(_playerCamera.fieldOfView, initialFOV, 0.2f));  // Adjust the duration as needed
    //     //
    //     // }
    //
    //     gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetGunPosition, Time.deltaTime * smoothness);    }
    }
    
}
