using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class look : MonoBehaviour
{
  
   
   
        [SerializeField] private Transform target;
        void Update()

        {
        // transform.LookAt(target);
        transform.LookAt(2 * transform.position - target.position);
    }

    
}
