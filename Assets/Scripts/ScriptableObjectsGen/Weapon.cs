using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outbreak
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Weapon : ScriptableObject
    {
        public string name;

        public string firerate;
        public float aimSpeed;

        public GameObject prefab;
   
    }
    
}
