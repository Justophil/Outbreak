using System;
using System.Collections;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;

    public class Equipment : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Header("Firing")]

        [Tooltip("Is this weapon automatic? If yes, then holding down the firing button will continuously fire.")]
        [SerializeField] 
        private bool automatic;
        
        [Tooltip("How fast the projectiles are.")]
        [SerializeField]
        private float projectileImpulse = 400.0f;

        [Tooltip("Amount of shots this weapon can shoot in a minute. It determines how fast the weapon shoots.")]
        [SerializeField] 
        private float rateOfFire = 0.1f;

        [Tooltip("Mask of things recognized when firing.")]
        [SerializeField]
        private LayerMask mask;

        [Tooltip("Maximum distance at which this weapon can fire accurately. Shots beyond this distance will not use linetracing for accuracy.")]
        [SerializeField]
        private float maximumDistance = 500.0f;

        [Header("Animation")]

        [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
        [SerializeField]
        private Transform socketEjection;

        [Header("Resources")]

        [Tooltip("Casing Prefab.")]
        [SerializeField]
        private GameObject prefabCasing;
        
        [Tooltip("Projectile Prefab. This is the prefab spawned when the weapon shoots.")]
        [SerializeField]
        private GameObject prefabProjectile;
        
        [Tooltip("The AnimatorController a player character needs to use while wielding this weapon.")]
        [SerializeField] 
        public RuntimeAnimatorController controller;

        [Tooltip("Weapon Body Texture.")]
        [SerializeField]
        private Sprite spriteBody;

        #endregion
        
        
        private Animator animator;
        private int ammunitionCurrent;
        
        private Transform _camera;

        private GameObject muzzleSocket;
        private MuzzleBehaviour muzzleBehaviour;
        
        private ParticleSystem particles;

        [Tooltip("Firing Particles.")]
        [SerializeField]
        private GameObject prefabFlashParticles;

        public GameObject bulletHole;


        
        
        private void Awake()
        {
            //Get Animator.
            animator = GetComponent<Animator>();
            muzzleSocket = GameObject.Find("SOCKET_Muzzle");
            GameObject playerCamera = GameObject.Find("PlayerCamera");
            
            if (playerCamera != null)
            {
                _camera = playerCamera.transform;
                Debug.Log("PlayerCamera set");
            }
            else
            {
                Debug.LogError("PlayerCamera not found in the scene!");
            }
        }
        protected void Start()
        {

        }

        private void Update()
        {
            // Debug.Log("Camera Position: " + _camera.position);

            // Debug.DrawRay(_camera.position, _camera.forward * 50f, Color.red);
            int flashParticlesCount = 5;

            
            if(particles != null)
                particles.Emit(flashParticlesCount);
        }
        
        public int GetAmmunitionCurrent() => ammunitionCurrent;
        
        public bool IsAutomatic() => automatic;
        public float GetRateOfFire() => rateOfFire;
        
        // public bool IsFull() => ammunitionCurrent == magazineBehaviour.GetAmmunitionTotal();
        public bool HasAmmunition() => ammunitionCurrent > 0;

        public RuntimeAnimatorController GetAnimatorController() => controller;



        public void Reload()
        {
            //Play Reload Animation.
            animator.Play(HasAmmunition() ? "Reload" : "Reload Empty", 0, 0.0f);
        }

        public void Fire(float spreadMultiplier = 1.0f)
        {
            Debug.Log("Pew Pew Pew!");
            Debug.DrawRay(_camera.position, _camera.forward.normalized * 100, Color.red);
            Quaternion rotation = Quaternion.LookRotation(_camera.forward * 1000.0f - muzzleSocket.transform.position);

            
            animator.Play("Fire", 0, 0.0f);
            
            // GameObject projectile = Instantiate(prefabProjectile, muzzleSocket.transform.position, rotation);

            //Add velocity to the projectile.
            // projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileImpulse;   
            
            
            
            RaycastHit hit;
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, maximumDistance, mask))
            {
                Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                StartCoroutine(BulletHit(hit));

            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }

        public IEnumerator RapidFire()
        {
            if (automatic)
            {
                while (true)
                {
                    Fire();
                    yield return new WaitForSeconds(1 / GetRateOfFire());   
                }
            }
            else
            {
                Fire();
                yield return null;
            }
        }
        
        IEnumerator BulletHit(RaycastHit hit)
        {
            GameObject bulletHolePoint = Instantiate(
                bulletHole,
                hit.point + (hit.normal * 0.01f),
                Quaternion.FromToRotation(Vector3.up, hit.normal)
            );

            Material bulletHoleMaterial = bulletHolePoint.GetComponent<Renderer>().material;

            float currentAlpha = 1.0f;

            while (currentAlpha > 0f)
            {
                currentAlpha -= Time.deltaTime * 0.1f; 
                Color newColor = bulletHoleMaterial.color;
                newColor.a = currentAlpha;
                bulletHoleMaterial.color = newColor;

                yield return null;
            }
            
            Destroy(bulletHolePoint);
        }


        public void FillAmmunition(int amount)
        {

        }

        public void EjectCasing()
        {

        }

    }
