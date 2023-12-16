using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
    {
        
        [Header("Firing")]
        [SerializeField] 
        private bool automatic;
        [SerializeField] 
        private float rateOfFire = 0.1f;
        [SerializeField] 
        private int clipSize = 500;
        
        [Tooltip("Mask of things recognized when firing.")]
        [SerializeField]
        private LayerMask mask;
        
        private float maximumDistance = 500.0f;
        public RuntimeAnimatorController animatorController;
        
        private Animator animator;
        private int ammunitionCurrent;
        private Transform _camera;
        private GameObject muzzleSocket;
        private ParticleSystem particles;
        public GameObject bulletHole;
        public int damage = 10;
        
        [SerializeField]
        private AudioSource gunshotAudioSource;
        [SerializeField]
        private AudioClip reloadSound;

        private float laserDuration = 0.05f;
        
        private LineRenderer _laserLine;
        
        Text ammo;
        
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
            _laserLine = GetComponent<LineRenderer>();
            ammo = GameObject.Find("Ammo").GetComponent<Text>();
            ammunitionCurrent = clipSize;
            gunshotAudioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            ammo.text = "Ammo: " + ammunitionCurrent;
        }
        
        public bool IsAutomatic() => automatic;
        public float GetRateOfFire() => rateOfFire;
        
        // public bool IsFull() => ammunitionCurrent == magazineBehaviour.GetAmmunitionTotal();
        public bool HasAmmunition() => ammunitionCurrent > 0;

        public RuntimeAnimatorController GetAnimatorController() => animatorController;
        
        public void Reload()
        {
            //Play Reload Animation
            if (HasAmmunition())
            {
                animator.Play("Reload", 0, 0.0f);
                ammunitionCurrent = clipSize;
            }
            else
            {
                animator.Play("Reload Empty", 0, 0.0f);
                ammunitionCurrent = clipSize;
            }
            gunshotAudioSource.PlayOneShot(reloadSound);
            animator.Play(HasAmmunition() ? "Reload" : "Reload Empty", 0, 0.0f);
        }

        public void Fire(float spreadMultiplier = 1.0f)
        {
            Debug.Log("Pew Pew Pew!");
            Debug.DrawRay(_camera.position, _camera.forward.normalized * 100, Color.red);
            Quaternion rotation = Quaternion.LookRotation(_camera.forward * 1000.0f - muzzleSocket.transform.position);

            
            

            
            // GameObject projectile = Instantiate(prefabProjectile, muzzleSocket.transform.position, rotation);

            //Add velocity to the projectile.
            // projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileImpulse;   
            
            
            
            RaycastHit hit;

            if (HasAmmunition())
            {
                animator.Play("Fire", 0, 0.0f);
                gunshotAudioSource.Play();
                ammunitionCurrent--;
                _laserLine.SetPosition(0, muzzleSocket.transform.position);
                if (Physics.Raycast(_camera.position, _camera.forward, out hit, maximumDistance, mask))
                {
                    Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                    _laserLine.SetPosition(1, hit.point);
                    ZombieStats zombieStats = hit.collider.gameObject.GetComponent<ZombieStats>();
                    if (zombieStats != null)
                    {
                        Debug.Log("Damage for: " + damage);
                        zombieStats.DecreaseHealth(damage);
                    }
                    if (!hit.collider.CompareTag("Zombie") || !hit.collider.CompareTag("BulletHole") || !hit.collider.CompareTag("Player"))
                    {
                        StartCoroutine(BulletHit(hit));
                    }
                }
                else
                {
                    _laserLine.SetPosition(1, (muzzleSocket.transform.position) + (_camera.forward * maximumDistance));

                    Debug.Log("Ray did not hit anything.");
                }

                StartCoroutine(ShootLaser());
            }
        }

        IEnumerator ShootLaser()
        {
            _laserLine.enabled = true;
            yield return new WaitForSeconds(laserDuration);
            _laserLine.enabled = false;
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
        

    }
