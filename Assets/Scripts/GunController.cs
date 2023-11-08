using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outbreak
{ 
    
    public class GunController : MonoBehaviour
    {
        public Weapon[] loadout;
        public Transform weaponParent;
        private GameObject currentWeapon;
        private Transform gunTransform;
        private float initialFOV;
        private float adsFOV = 30.0f;
        private Vector3 initialGunPosition;
        private Vector3 targetGunPosition;
        private CharacterController _controller;
        private Camera _playerCamera;

        private int currentIndex;

        // Rotations
        private Vector3 currentRotation;
        private Vector3 targetRotation;

        // Hipfire recoil
        [SerializeField] private float recoilX;
        [SerializeField] private float recoilY;
        [SerializeField] private float recoilZ;

        // Settings
        [SerializeField] private float snappiness;
        [SerializeField] private float returnSpeed;

        private float smoothness = 5.0f;

        private bool isAiming = false;

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
