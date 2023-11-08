using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Transform gunTransform;
    private float initialFOV;
    private float adsFOV = 30.0f;

    private Vector3 initialGunPosition;
    private Vector3 targetGunPosition;
    private CharacterController _controller;
    private Camera _playerCamera;
    
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    
    //Hipfire recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    
    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;



    
    [SerializeField] private float smoothness = 5.0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _playerCamera = GetComponentInChildren<Camera>();
        
        gunTransform = transform.GetChild(0).GetChild(0);
        initialFOV = _playerCamera.fieldOfView;

        initialGunPosition = gunTransform.localPosition;
        targetGunPosition = initialGunPosition;
    }

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedTime);
        gunTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    
}