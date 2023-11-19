using System;
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
        private int roundsPerMinutes = 200;

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
        //
        // [Header("Audio Clips Holster")]
        //
        // [Tooltip("Holster Audio Clip.")]
        // [SerializeField]
        // private AudioClip audioClipHolster;
        //
        // [Tooltip("Unholster Audio Clip.")]
        // [SerializeField]
        // private AudioClip audioClipUnholster;
        //
        // [Header("Audio Clips Reloads")]
        //
        // [Tooltip("Reload Audio Clip.")]
        // [SerializeField]
        // private AudioClip audioClipReload;
        //
        // [Tooltip("Reload Empty Audio Clip.")]
        // [SerializeField]
        // private AudioClip audioClipReloadEmpty;
        //
        // [Header("Audio Clips Other")]
        //
        // [Tooltip("AudioClip played when this weapon is fired without any ammunition.")]
        // [SerializeField]
        // private AudioClip audioClipFireEmpty;

        #endregion
        
        
        private Animator animator;
        private int ammunitionCurrent;
        
        private Transform _camera;



        
        
        protected void Awake()
        {
            //Get Animator.
            animator = GetComponent<Animator>();
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
            
            

        }

        public Animator GetAnimator() => animator;
        
        public Sprite GetSpriteBody() => spriteBody;

        // public AudioClip GetAudioClipHolster() => audioClipHolster;
        // public AudioClip GetAudioClipUnholster() => audioClipUnholster;
        //
        // public AudioClip GetAudioClipReload() => audioClipReload;
        // public AudioClip GetAudioClipReloadEmpty() => audioClipReloadEmpty;
        //
        // public AudioClip GetAudioClipFireEmpty() => audioClipFireEmpty;
        
        // public AudioClip GetAudioClipFire() => muzzleBehaviour.GetAudioClipFire();
        
        public int GetAmmunitionCurrent() => ammunitionCurrent;
        
        public bool IsAutomatic() => automatic;
        public float GetRateOfFire() => roundsPerMinutes;
        
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
            
            RaycastHit hit;
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, maximumDistance, mask))
            {
                Debug.Log("Ray hit: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }


        public void FillAmmunition(int amount)
        {

        }

        public void EjectCasing()
        {

        }

    }
