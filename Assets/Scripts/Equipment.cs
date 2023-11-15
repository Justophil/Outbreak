using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Transform _playerCamera;

    
    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = GetComponentInChildren<Camera>().transform;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
